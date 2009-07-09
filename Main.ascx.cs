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
using System.Net;
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
            AJAX.RegisterScriptManager();

            // load css
            CDefault defaultPage = (CDefault)Page;
            defaultPage.AddStyleSheet("FastShot_default", TemplateSourceDirectory + "/templates/default/theme.css");

            //ctlAddEdit.RenderProc = RenderItems;
            RenderItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AJAX.RegisterScriptManager();
            ScriptManager.RegisterStartupScript(upnlRender, upnlRender.GetType(), "initLightbox", "avt.fastshot.init();", true);

            if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                pnlSettings.Visible = true;

                btnAddNewItem.OnClientClick = "avt.fastshot.$$.showDlg('" + TemplateSourceDirectory + "/AddEditItem.aspx?pmod=" + ModuleId.ToString() + "', { width: 450, height: 500, cssClass : 'FastShot_dlg', title : 'FastShot - Add/Edit Item', rightText : '" + FastShotController.FastShotVersionAll + "', postbackid_onsave : '" + triggerSave.UniqueID + "'}); return false;";
                btnSettings.OnClientClick = "avt.fastshot.$$.showDlg('" + TemplateSourceDirectory + "/Settings.aspx?pmod=" + ModuleId.ToString() + "', { width: 450, height: 500, cssClass : 'FastShot_dlg', title : 'FastShot - Settings', rightText : '" + FastShotController.FastShotVersionAll + "', postbackid_onsave : '" + triggerSaveSettings.UniqueID + "'}); return false;";
                btnSettingsNewWnd.OnClientClick = "window.open('" + TemplateSourceDirectory + "/Settings.aspx?pmod=" + ModuleId.ToString() + "&rurl=" + Server.UrlEncode(Request.RawUrl) + "', '_self', ''); return false;";
                btnActivate.OnClientClick = "avt.fastshot.$$.showDlg('" + TemplateSourceDirectory + "/Activation.aspx', { width: 550, height: 400, cssClass : 'FastShot_dlg', title : 'FastShot - Activation', rightText : '" + FastShotController.FastShotVersionAll + "', postbackid_onsave : '" + triggerRender.UniqueID + "', show_save : false, cancel_text : 'Close', refresh_onsave : true}); return false;";
                btnActivateNewWnd.OnClientClick = "window.open('" + TemplateSourceDirectory + "/Activation.aspx', '_blank', 'width=540'); return false;";
                btnAddNewItemNewWnd.OnClientClick = "window.open('" + TemplateSourceDirectory + "/AddEditItem.aspx?pmod=" + ModuleId.ToString() + "&rurl=" + Server.UrlEncode(Request.RawUrl) + "', '_self', ''); return false;";
                //triggerDelete.OnClientClick = "avt.fs.$('#" + upnlRender.ClientID + "').find('.fsOrderingChanged').html('Deleting, please wait...').show('slide', {direction: 'up'});";

                pnlDragOrdering.Visible = true;

                ScriptManager.RegisterStartupScript(upnlRender, upnlRender.GetType(), "initSortable" + ModuleId.ToString(), "avt.fastshot.$('#" + upnlRender.ClientID + "').find('.FastShot_grid').sortable({revert: true}); avt.fastshot.$('#" + upnlRender.ClientID + "').find('.FastShot_grid').find('li').bind('mouseup', function() { if (avt.fastshot.$(this).css('position') == 'absolute') { avt.fastshot.$('#" + fsOrderingChanged.ClientID + ":hidden').show('slide', {direction: 'up'}); avt.fastshot.$(this).find('a').children('').bind('click', function() {return false; }); } }).bind('mousedown', function() { avt.fastshot.$(this).find('a').children('').unbind('click'); });", true);
            } else {
                pnlSettings.Visible = false;
            }


            ScriptManager.RegisterStartupScript(upnlRender, upnlRender.GetType(), "initProdMenu" + ModuleId.ToString(), "initProductMenu" + ModuleId.ToString() + "();", true);
            ScriptManager.RegisterStartupScript(upnlRender, upnlRender.GetType(), "initGrid" + ModuleId.ToString(), "avt.fs.$(document).ready(function() { avt.fs.initGrid(avt.fs.$('#" + upnlRender.ClientID + "').find('.FastShot_grid')); });", true);
            ScriptManager.RegisterStartupScript(upnlRender, upnlRender.GetType(), "avtFsInit", "avt.fs.$$.init({appPath:'" + Request.ApplicationPath + "', loaderIcon : '" + TemplateSourceDirectory + "/res/loader.gif'});", true);
        }

        public void RenderItems()
        {
            FastShotController fShotCtrl = new FastShotController();

            FastShotSettings FsSettings = new FastShotSettings();
            FsSettings.Load(ModuleId);

            bool isActivated = false;
            try {
                //isActivated = fShotCtrl.IsActivated(Server.MapPath(TemplateSourceDirectory + "\\activations.txt"));
                isActivated = fShotCtrl.IsActivated(TemplateSourceDirectory);
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
                btnActivateNewWnd.Visible = !isActivated;
            }
            
            ArrayList items = fShotCtrl.GetItems(ModuleId);

            StringBuilder strXML = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XmlWriter Writer = XmlWriter.Create(strXML, settings);

            Writer.WriteStartElement("fastshot");
            Writer.WriteElementString("root", TemplateSourceDirectory + "/");
            Writer.WriteElementString("mid", ModuleId.ToString());
            
            if (!isActivated) {
                
                //if (items.Count > 2) {
                //    items.RemoveRange(2, items.Count - 2);
                //}

                // randomly replace an image with FastShot logo
                if (items.Count > 1) {
                    Random rand = new Random();

                    int iImgRepl = rand.Next(0, items.Count - 1);
                    ((ItemInfo)items[iImgRepl]).ImageUrl = "http://www.avatar-soft.ro/portals/0/product_logo/fastshot_large.png";
                    ((ItemInfo)items[iImgRepl]).Title = "FastShot Demo";
                    ((ItemInfo)items[iImgRepl]).Description = "This copy of FastShot is not activated! Visit <a href = 'http://www.avatar-soft.ro' style = 'font-weight: bold;'>www.avatar-soft.ro</a> to read more about FastShot.";

                    if (items.Count > 6) {
                        iImgRepl = rand.Next(0, items.Count - 1);
                        ((ItemInfo)items[iImgRepl]).ImageUrl = "http://www.avatar-soft.ro/portals/0/product_logo/fastshot_large.png";
                        ((ItemInfo)items[iImgRepl]).Title = "FastShot Demo";
                        ((ItemInfo)items[iImgRepl]).Description = "This copy of FastShot is not activated! Visit <a href = 'http://www.avatar-soft.ro' style = 'font-weight: bold;'>www.avatar-soft.ro</a> to read more about FastShot.";
                    }
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
                if (!item.AutoGenerateThumb && item.ThumbnailUrl != null && item.ThumbnailUrl.Length > 0) {
                    Writer.WriteElementString("thumburl", item.ThumbnailUrl);
                    System.Drawing.Image image;
                    try {
                        image = System.Drawing.Image.FromFile(Server.MapPath(item.ThumbnailUrl));
                        Writer.WriteElementString("thumb_width", image.Width.ToString());
                        Writer.WriteElementString("thumb_height", image.Height.ToString());
                    } catch {
                        Writer.WriteElementString("thumb_width", "10");
                        Writer.WriteElementString("thumb_height", "10");
                    }
                    
                } else {
                    string thumbUrl = TemplateSourceDirectory + "/MakeThumb.aspx?file=" + Server.UrlEncode(item.ImageUrl) + "&height=" + FsSettings.ThumbHeight.ToString() + "&width=" + FsSettings.ThumbWidth.ToString();
                    Writer.WriteElementString("thumburl", thumbUrl);
                    
                    try {
                    System.Drawing.Image image = LoadImageFromURL("http://" + Request.Url.Host + ":" + Request.Url.Port + thumbUrl);
                        Writer.WriteElementString("thumb_width", image.Width.ToString());
                        Writer.WriteElementString("thumb_height", image.Height.ToString());
                    } catch {
                        Writer.WriteElementString("thumb_width", "10");
                        Writer.WriteElementString("thumb_height", "10");
                    }
                }
                Writer.WriteElementString("imgurl", item.ImageUrl);

                if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                    PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                    Writer.WriteElementString("editurl", "javascript: avt.fastshot.$$.showDlg('"+TemplateSourceDirectory+"/AddEditItem.aspx?pmod=" + ModuleId.ToString() + "&itemid=" + item.ItemId.ToString() + "', { width: 450, height: 500, cssClass : 'FastShot_dlg', title : 'FastShot - Add/Edit Item', rightText : '" + FastShotController.FastShotVersionAll + "', postbackid_onsave : '" + triggerSave.UniqueID + "'});");
                    Writer.WriteElementString("editurl_comp", TemplateSourceDirectory + "/AddEditItem.aspx?pmod=" + ModuleId.ToString() + "&itemid=" + item.ItemId.ToString() + "&rurl=" + Server.UrlEncode(Request.RawUrl));
                    Writer.WriteElementString("deleteurl", "javascript: avt.fastshot.$$.confirm('Confirm Delete', 'Are you sure you want to delete this item?', '" + triggerDelete.UniqueID + "', '" + item.ItemId.ToString() + "', 'FastShot_dlg');");
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
            transform.Load(Server.MapPath(TemplateSourceDirectory + "/templates/"+ FsSettings.Template +"/main.xsl"));
            System.IO.StringWriter output = new System.IO.StringWriter();

            transform.Transform(doc, null, output);
            itemContainer.InnerHtml = "";
            //if (!isActivated)
            //    itemContainer.InnerHtml += "<div style = 'float: left; margin: 20px;'><a href = 'http://www.avatar-soft.ro'><img border = '0' src = '" + TemplateSourceDirectory + "/res/fastshot_medium.png" + "' class = 'pngFix' alt = 'Visit http://www.avatar-soft.ro for more information about FastShot...' title = 'Visit http://www.avatar-soft.ro for more information about FastShot...' /></a></div>"; 
            itemContainer.InnerHtml += output.ToString();

            btnActivate.Visible = !isActivated;
        }

        private System.Drawing.Image LoadImageFromURL(string URL)
        {
            const int BYTESTOREAD = 10000;
            WebRequest myRequest = WebRequest.Create(URL);
            WebResponse myResponse = myRequest.GetResponse();
            System.IO.Stream ReceiveStream = myResponse.GetResponseStream();
            System.IO.BinaryReader br = new System.IO.BinaryReader(ReceiveStream);
            System.IO.MemoryStream memstream = new System.IO.MemoryStream();
            byte[] bytebuffer = new byte[BYTESTOREAD];
            int BytesRead = br.Read(bytebuffer, 0, BYTESTOREAD);
            while (BytesRead > 0) {
                memstream.Write(bytebuffer, 0, BytesRead);
                BytesRead = br.Read(bytebuffer, 0, BYTESTOREAD);
            }

            return System.Drawing.Image.FromStream(memstream);
        }


        protected override void OnPreRender(EventArgs e)
        {
            // doing this at another stage will break things work on IE
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("avt_jQuery_1_3_2")) {
                Page.ClientScript.RegisterClientScriptInclude("avt_jQuery_1_3_2", TemplateSourceDirectory + "/js/jquery/jquery-1.3.2.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("avt_jQueryUi_1_6")) {
                Page.ClientScript.RegisterClientScriptInclude("avt_jQueryUi_1_6", TemplateSourceDirectory + "/js/jquery/jquery-ui-1.6.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("avt_core_1_2")) {
                Page.ClientScript.RegisterClientScriptInclude("avt_core_1_2", TemplateSourceDirectory + "/js/avt.core-1.2.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQueryLightbox")) {
                Page.ClientScript.RegisterClientScriptInclude("jQueryLightbox", TemplateSourceDirectory + "/js/jquery-lightbox/jquery.lightbox.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQuery.jGrowl-1.2.0")) {
                Page.ClientScript.RegisterClientScriptInclude("jQuery.jGrowl-1.2.0", TemplateSourceDirectory + "/js/jGrowl-1.2.0/jquery.jgrowl.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("avtFastShot")) {
                Page.ClientScript.RegisterClientScriptInclude("avtFastShot", TemplateSourceDirectory + "/js/avtFastShot.js");
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQuery.fg-menu")) {
                Page.ClientScript.RegisterClientScriptInclude("jQuery.fg-menu", TemplateSourceDirectory + "/js/fg-menu/fg-menu/fg.menu.js");
            }

            CDefault defaultPage = (CDefault)Page;
            defaultPage.AddStyleSheet("skinLightbox", TemplateSourceDirectory + "/js/jquery-lightbox/css/lightbox.css");
            defaultPage.AddStyleSheet("theme.jquery.ui", TemplateSourceDirectory + "/js/ui-themes/smoothness/jquery-ui-1.7.1.css");
            defaultPage.AddStyleSheet("skin.jGrowl-1.2.0", TemplateSourceDirectory + "/js/jGrowl-1.2.0/jquery.jgrowl.css");

            base.OnPreRender(e);
        }


        protected void OnSaveSuccess(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(upnlRender, upnlRender.GetType(), "addEditSuccess", "avt.fs.$(document).ready(function() {avt.fs.$.jGrowl('Image successfully updated!', {header: 'Add/Edit Succesfull', life: 5000})});", true);
            RenderItems();
        }

        protected void OnSaveSettingsSuccess(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(upnlRender, upnlRender.GetType(), "saveSuccess", "avt.fs.$(document).ready(function() {avt.fs.$.jGrowl('Settings successfully updated!', {header: 'Save Succesfull', life: 5000})});", true);
            RenderItems();
        }
        

        protected void OnRender(object sender, EventArgs e)
        {
            RenderItems();
            //upnlRender.Update();
        }

        protected void OnChangeOrder(object sender, EventArgs e)
        {
            FastShotController fsCtrl = new FastShotController();

            int orderIndex = 0; // TODO: recalc for pagination
            string[] itemIds = hdnItemOrder.Value.Split(new char[] { ',' });
            foreach (string strItemId in itemIds) {
                int itemId;
                try {
                    itemId = Convert.ToInt32(strItemId);
                } catch {
                    continue; // something went wrong?
                }

                ItemInfo itemInfo = fsCtrl.GetItemById(itemId);
                if (itemInfo == null) {
                    continue; // something else went wrong?
                }

                fsCtrl.UpdateItem(itemInfo.ItemId, ModuleId, itemInfo.Title, itemInfo.Description, itemInfo.ThumbnailUrl, itemInfo.ImageUrl, orderIndex,itemInfo.AutoGenerateThumb);
                orderIndex++;
            }
            RenderItems();

            ScriptManager.RegisterStartupScript(upnlConf, upnlRender.GetType(), "saveSuccess", "avt.fs.$(document).ready(function() {avt.fs.$.jGrowl('FastShot succesfully updated items order!', {header: 'Save Succesfull', life: 5000})});", true);
        }


        protected void OnShowAddEdit(object sender, EventArgs e)
        {
            //ctlAddEdit.Visible = true;

            int itemId = -1;
            try {
                itemId = Convert.ToInt32(Request.Params["__EVENTARGUMENT"]);
            } catch {
                itemId = -1;
            }
            if (itemId > 0) {
                //ctlAddEdit.EditItem(itemId);
            } else {
                //ctlAddEdit.NewItem();
            }

            //ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "enableInput", "avt.Common.enableInput();", true);
        }

        protected void OnDelete(object sender, EventArgs e)
        {
            int itemId;
            try {
                itemId = Convert.ToInt32(Request.Params["__EVENTARGUMENT"]);
            } catch (Exception) {
                //ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "errorDelete", "alert('An error occured while deleting item. Please contact Avatar Software for support if the problem persist.');", true);
                return;
            }


            FastShotController fsCtrl = new FastShotController();
            ItemInfo itemInfo = fsCtrl.GetItemById(itemId);
            if (itemInfo == null) {
                //ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "errorDelete", "alert('The item you\\'ve tried to delete doesn't exists.');", true);
                return;
            }

            fsCtrl.DeleteItem(itemId);
            
            // also, delete files from disk
            try {
                System.IO.File.Delete(Server.MapPath(itemInfo.ThumbnailUrl));
                System.IO.File.Delete(Server.MapPath(itemInfo.ImageUrl));
            } catch {
            }
            

            RenderItems();
            upnlRender.Update();
            
            ScriptManager.RegisterStartupScript(upnlConf, upnlConf.GetType(), "updateImages", "__doPostBack('" + triggerRender.UniqueID + "','');", true);
            ScriptManager.RegisterStartupScript(upnlConf, upnlRender.GetType(), "deleteSuccess", "avt.fs.$(document).ready(function() {avt.fs.$.jGrowl('FastShot succesfully deleted image <b>" + itemInfo.Title + "</b>!', {header: 'Delete Succesfull', life: 5000})});", true);
        }
        
        
    }

}
