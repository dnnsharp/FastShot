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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace avt.FastShot
{
    public partial class AddEditItem : PortalModuleBase
    {
        #region private member variables

        
        
        #endregion

        public delegate void CbRenderProc();

        public int PModuleId;
        public CbRenderProc RenderProc;

        /////////////////////////////////////////////////////////////////////////////////
        // Module actions

     


        /////////////////////////////////////////////////////////////////////////////////
        // EVENT HANDLERS

        protected void Page_Init(object sender, EventArgs e)
        {
            // force loading of the correct resource file
            
            this.LocalResourceFile = TemplateSourceDirectory + "/App_LocalResources/AddEditItem.ascx.resx";

            cmdUpdate.OnClientClick = "return avt.FastShot.core.onSaveItem('"+ cmdUpdate.UniqueID +"');";
            cmdCancel.OnClientClick = "jQuery('#newItem').dialog('destroy'); document.forms[0].reset(); Page_ValidationActive = false; return false;";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "enableInput", "avt.Common.enableInput();", true);
        }

        public void NewItem()
        {
            txtTitle.Text = "";
            txtDesc.Text = "";
            txtViewOrder.Text = "";
        }

        public void EditItem(int itemId)
        {
            FastShotController fShotCtrl = new FastShotController();
            ItemInfo item = fShotCtrl.GetItemById(itemId);

            txtTitle.Text = item.Title;
            txtDesc.Text = item.Description;
            txtViewOrder.Text = item.ViewOrder.ToString();

            sThumbName.InnerHtml = Regex.Match(item.ThumbnailUrl, "\\d+_([^/]+.[^/]+$)").Groups[1].Value + "<br /><i style = 'font-weight: normal; font-size: 10px;'>(leave blank to keep existing image)</i>";
            sImageName.InnerHtml = Regex.Match(item.ImageUrl, "\\d+_([^/]+.[^/]+$)").Groups[1].Value + "<br /><i style = 'font-weight: normal; font-size: 10px;'>(leave blank to keep existing image)</i>";

            hdnItemId.Value = itemId.ToString();
        }

        protected void OnSave(object sender, EventArgs e)
        {
            int viewOrder = -1;
            if (txtViewOrder.Text.Length > 0) {
                viewOrder = Convert.ToInt32(txtViewOrder.Text);
            }

            FastShotController fsCtrl = new FastShotController();

            int itemId = -1;
            try {
                itemId = Convert.ToInt32(hdnItemId.Value);
            } catch (Exception) {
                itemId = -1;
            }

            if (itemId > 0) {

                //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "a1", "alert('done" + hdnThumbFileName.Value + "');", true);
                ItemInfo existingItem = fsCtrl.GetItemById(itemId);

                string thumbUrl = hdnThumbFileName.Value.Length > 0 ? PortalSettings.HomeDirectory + "FastShot/" + PModuleId + "/thumbs/" + hdnThumbFileName.Value : "";
                string imageUrl = hdnImageFileName.Value.Length > 0 ? PortalSettings.HomeDirectory + "FastShot/" + PModuleId + "/images/" + hdnImageFileName.Value : "";

                if (thumbUrl.Length > 0) { // delete old thumbnail
                    System.IO.File.Delete(Server.MapPath(existingItem.ThumbnailUrl));
                }

                if (imageUrl.Length > 0) { // delete old thumbnail
                    System.IO.File.Delete(Server.MapPath(existingItem.ImageUrl));
                }

                fsCtrl.UpdateItem(itemId, PModuleId, txtTitle.Text, txtDesc.Text, thumbUrl, imageUrl, Convert.ToInt32(viewOrder));
            } else {
                fsCtrl.AddItem(PModuleId, txtTitle.Text, txtDesc.Text, PortalSettings.HomeDirectory + "FastShot/" + PModuleId + "/thumbs/" + hdnThumbFileName.Value, PortalSettings.HomeDirectory + "FastShot/" + PModuleId + "/images/" + hdnImageFileName.Value, Convert.ToInt32(viewOrder));
            }


            if (RenderProc != null) {
                RenderProc();
            }
            ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "closeConf", "jQuery('#newItem').dialog('destroy').find(':input').val('');", true);
            ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "reinitLighbox", "avt.FastShot.core.init();", true);
            
            //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "a21", "alert('done" + Parent.Parent.Parent.GetType() + "');", true);
            //((IMain)Parent.Parent.Parent).RenderItems();
            ((UpdatePanel)Parent.FindControl("upnlRender")).Update();
        }

        protected void OnCancel(object sender, EventArgs e)
        {
        }

        
       
        
    }

}
