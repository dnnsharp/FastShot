using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using System.Text.RegularExpressions;

using DotNetNuke;
using DotNetNuke.Framework;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace avt.FastShot
{
    public partial class Main : PortalModuleBase, IActionable, IMain
    {
        #region private member variables

        
        
        #endregion

        

        /////////////////////////////////////////////////////////////////////////////////
        // Module actions

        ModuleActionCollection IActionable.ModuleActions
        {
            get {
                ModuleActionCollection Actions = new ModuleActionCollection();
                return Actions;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////
        // EVENT HANDLERS

        protected void Page_Init(object sender, EventArgs e)
        {
            // load css
            CDefault defaultPage = (CDefault)Page;
            defaultPage.AddStyleSheet("FastShot_default", "~/" + TemplateSourceDirectory + "/templates/default/theme.css");

            ctlAddEdit.RenderProc = RenderItems;
            RenderItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "enableInput", "avt.Common.enableInput();", true);

            if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                pnlManage.Visible = true;
                btnAddNewItem.OnClientClick = "avt.FastShot.core.addEditItem('" + triggerAddEdit.UniqueID + "', ''); return false;";
                btnActivate.OnClientClick = ctlActivate.GetOpenLink();
                ctlAddEdit.PModuleId = ModuleId;
            }
        }

        public void RenderItems()
        {
            FastShotController fShotCtrl = new FastShotController();

            bool isActivated = false;
            try {
                isActivated = fShotCtrl.IsActivated(Server.MapPath(TemplateSourceDirectory + "\\activations.txt"));
            } catch (System.Net.WebException) {
                pnlErr.InnerHtml = "Unable to connect to Avatar Software servers. If this problem persisit please contact Avatar Software for support.";

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // TODO:
                isActivated = true;
            }

            if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                btnActivate.Visible = !isActivated;
            }
            
            ArrayList items = fShotCtrl.GetItems(ModuleId);

            StringBuilder strXML = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XmlWriter Writer = XmlWriter.Create(strXML, settings);

            Writer.WriteStartElement("fastshot");

            if (!isActivated) {
                if (items.Count > 2) {
                    items.RemoveRange(2, items.Count - 2);
                }
                //ItemInfo demoItem = new ItemInfo();
                //demoItem.ItemId = -1;
                //demoItem.ModuleId = ModuleId;
                ////demoItem.Title = "FastShot Demo";
                //demoItem.Description = "Visit http://products.avatar-soft.ro for more information about FastShot...";
                //demoItem.ThumbnailUrl = TemplateSourceDirectory + "/res/fastshot_medium.png";
                //demoItem.ImageUrl = "http://products.avatar-soft.ro";
                //items.Add(demoItem);
            }

            foreach (ItemInfo item in items) {
                Writer.WriteStartElement("img");
                Writer.WriteElementString("id", item.ItemId.ToString());
                Writer.WriteElementString("title", item.Title);
                Writer.WriteElementString("desc", item.Description);
                Writer.WriteElementString("thumburl", item.ThumbnailUrl);
                Writer.WriteElementString("imgurl", item.ImageUrl);

                if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                    PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                    Writer.WriteElementString("editurl", "javascript: avt.FastShot.core.addEditItem('" + triggerAddEdit.UniqueID + "', '" + item.ItemId.ToString() + "');");
                    Writer.WriteElementString("deleteurl", "javascript: avt.FastShot.core.deleteItem('" + triggerDelete.UniqueID + "', '" + item.ItemId.ToString() + "');");
                }

                Writer.WriteEndElement(); //img
            }


            if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                Writer.WriteStartElement("mng");
            //    Writer.WriteElementString("title", "New Image");
            //    Writer.WriteElementString("desc", "Click to enlarge");
            //    Writer.WriteElementString("thumburl", "/DesktopModules/avt.FastShot/templates/default/New_Image_HOV.png");
            //    Writer.WriteElementString("conf", "javascript: avt.FastShot.core.addEditItem('" + triggerAddEdit.UniqueID + "', '');");
                Writer.WriteEndElement(); //mng
            }

            Writer.WriteEndElement(); // fastshot
            Writer.Close();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXML.ToString());

            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(Server.MapPath(TemplateSourceDirectory + "/templates/default/main.xsl"));
            System.IO.StringWriter output = new System.IO.StringWriter();

            transform.Transform(doc, null, output);
            itemContainer.InnerHtml = "";
            itemContainer.InnerHtml += "<div style = 'float: left; margin: 20px;'><a href = 'http://products.avatar-soft.ro'><img border = '0' src = '" + TemplateSourceDirectory + "/res/fastshot_medium.png" + "' alt = 'Visit http://products.avatar-soft.ro for more information about FastShot...' title = 'Visit http://products.avatar-soft.ro for more information about FastShot...' /></a></div>"; 
            itemContainer.InnerHtml += output.ToString();

            
        }


        protected override void OnPreRender(EventArgs e)
        {
            // doing this at another stage will break things work on IE
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQuery")) {
                Page.ClientScript.RegisterClientScriptInclude("jQuery", TemplateSourceDirectory + "/js/jquery/jquery.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQueryUi")) {
                Page.ClientScript.RegisterClientScriptInclude("jQueryUi", TemplateSourceDirectory + "/js/jquery/ui/jquery.ui.all.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQueryLightbox")) {
                Page.ClientScript.RegisterClientScriptInclude("jQueryLightbox", TemplateSourceDirectory + "/js/jquery-lightbox/jquery.lightbox.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("avtFastShot")) {
                Page.ClientScript.RegisterClientScriptInclude("avtFastShot", TemplateSourceDirectory + "/js/avtFastShot.js");
            }

            CDefault defaultPage = (CDefault)Page;
            defaultPage.AddStyleSheet("skinLightbox", "~/" + TemplateSourceDirectory + "/js/jquery-lightbox/css/lightbox.css");

            base.OnPreRender(e);
        }


        protected void OnRender(object sender, EventArgs e)
        {
            RenderItems();
            //upnlRender.Update();
        }


        protected void OnShowAddEdit(object sender, EventArgs e)
        {
            ctlAddEdit.Visible = true;

            int itemId = -1;
            try {
                itemId = Convert.ToInt32(Request.Params["__EVENTARGUMENT"]);
            } catch {
                itemId = -1;
            }
            if (itemId > 0) {
                ctlAddEdit.EditItem(itemId);
            } else {
                ctlAddEdit.NewItem();
            }

            ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "enableInput", "avt.Common.enableInput();", true);
        }

        protected void OnDelete(object sender, EventArgs e)
        {
            int itemId;
            try {
                itemId = Convert.ToInt32(Request.Params["__EVENTARGUMENT"]);
            } catch (Exception) {
                ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "errorDelete", "alert('An error occured while deleting item. Please contact Avatar Software for support if the problem persist.');", true);
                return;
            }


            FastShotController fsCtrl = new FastShotController();
            ItemInfo itemInfo = fsCtrl.GetItemById(itemId);
            if (itemInfo == null) {
                ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "errorDelete", "alert('The item you\\'ve tried to delete doesn't exists.');", true);
                return;
            }

            fsCtrl.DeleteItem(itemId);
            
            // also, delete files from disk
            System.IO.File.Delete(Server.MapPath(itemInfo.ThumbnailUrl));
            System.IO.File.Delete(Server.MapPath(itemInfo.ImageUrl));
            

            RenderItems();
            upnlRender.Update();
            //ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "updateImages", "__doPostBack('" + triggerRender.UniqueID + "','');", true);
        }
        
        
    }

}
