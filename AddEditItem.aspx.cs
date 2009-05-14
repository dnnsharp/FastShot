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
    public partial class AddEditItem : DotNetNuke.Framework.CDefault
    {
        public delegate void CbRenderProc();

        private int PMod;
        public CbRenderProc RenderProc;

        /////////////////////////////////////////////////////////////////////////////////
        // Module actions

     


        /////////////////////////////////////////////////////////////////////////////////
        // EVENT HANDLERS

        protected void Page_Init(object sender, EventArgs e)
        {
            // TODO: check security

            // force loading of the correct resource file
            
            this.LocalResourceFile = TemplateSourceDirectory + "/App_LocalResources/AddEditItem.aspx.resx";

            


            //cmdUpdate.OnClientClick = "return avt.FastShot.core.onSaveItem('" + cmdUpdate.UniqueID + "', " + (hdnItemId.Value.Length > 0 ? "true" : "false") + ");";
            //cmdCancel.OnClientClick = "avt.FastShot.$('#newItem').dialog('destroy').find(':input').val(''); avt.FastShot.$('.ui-dialog-overlay').remove(); document.getElementById('" + sThumbName.ClientID + "').innerHTML = ''; document.getElementById('" + sImageName.ClientID + "').innerHTML = ''; Page_ValidationActive = false; return false;";

            //iUploadThumbFrame.Attributes["src"] = TemplateSourceDirectory + "/upload.aspx?moduleId="+ PMod + "&m=thumb";
            //iUploadThumbFrame.Attributes["onload"] = "avt.FastShot.core.thumbUploaded('" + hdnThumbFileName.ClientID + "');";

            //iUploadImageFrame.Attributes["src"] = TemplateSourceDirectory + "/upload.aspx?moduleId=" + PMod;
            //iUploadImageFrame.Attributes["onload"] = "avt.FastShot.core.imageUploaded('" + hdnImageFileName.ClientID + "');";


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PMod = Convert.ToInt32(Request.QueryString["pmod"]);

            // check that user has rights
            try {
                ModuleController modCtrl = new ModuleController();
                ModuleInfo ModuleConfiguration = modCtrl.GetModule(PMod, -1);

                if (ModuleConfiguration == null) {
                    throw new Exception();
                }

                if (!PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                    throw new Exception();
                }
            } catch {
                Response.Write("Access denied!");
                Response.End();
            }


            if (!Page.IsPostBack) {
                cbAutoGenerate.Checked = true;
                int itemId = -1;
                if (Request.QueryString["itemid"] != null) {
                    try {
                        itemId = Convert.ToInt32(Request.QueryString["itemid"]);
                        EditItem(itemId);
                    } catch {
                        Response.Write("Invalid Input!");
                        Response.End();
                    }
                }

                if (itemId == -1) {
                    NewItem();
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "initAutoGenerate", "onAutoGenerate(document.getElementById('"+ cbAutoGenerate.ClientID +"'));", true);
        }

        private void NewItem()
        {
            txtTitle.Text = "";
            txtDesc.Text = "";
            txtViewOrder.Text = "";
            hdnItemId.Value = "";

            //sThumbName.InnerHtml = "";
            //sImageName.InnerHtml = "";

            cmdUpdate.OnClientClick = "return avt.FastShot.core.onSaveItem('" + cmdUpdate.UniqueID + "', " + (hdnItemId.Value.Length > 0 ? "true" : "false") + ");";
        }

        private void EditItem(int itemId)
        {
            FastShotController fShotCtrl = new FastShotController();
            ItemInfo item = fShotCtrl.GetItemById(itemId);

            txtTitle.Text = item.Title;
            txtDesc.Text = item.Description;
            txtViewOrder.Text = item.ViewOrder.ToString();

            lblExistingImage.Text = System.IO.Path.GetFileName(item.ImageUrl);
            lblExistingImage.Text = lblExistingImage.Text.Substring(lblExistingImage.Text.IndexOf('_') + 1);
            lblExistingImage.Text = "image: <b>" + lblExistingImage.Text + "</b><br />";

            if (!String.IsNullOrEmpty(item.ThumbnailUrl)) {
                lblExistingThumb.Text = System.IO.Path.GetFileName(item.ThumbnailUrl);
                lblExistingThumb.Text = lblExistingThumb.Text.Substring(lblExistingThumb.Text.IndexOf('_') + 1);
                lblExistingThumb.Text = "thumb: <b>" + lblExistingThumb.Text + "</b><br />";
            }

            //sThumbName.InnerHtml = Regex.Match(item.ThumbnailUrl, "\\d+_([^/]+.[^/]+$)").Groups[1].Value + "<br /><i style = 'font-weight: normal; font-size: 10px;'>(leave blank to keep existing image)</i>";
            //sImageName.InnerHtml = Regex.Match(item.ImageUrl, "\\d+_([^/]+.[^/]+$)").Groups[1].Value + "<br /><i style = 'font-weight: normal; font-size: 10px;'>(leave blank to keep existing image)</i>";

            hdnItemId.Value = itemId.ToString();
            cbAutoGenerate.Checked = item.AutoGenerateThumb;

            lblReqImage.Visible = false;
            lblReqThumb.Visible = false;
            reqImage.Visible = false;
            reqThumb.Visible = false;

            cmdUpdate.OnClientClick = "return avt.FastShot.core.onSaveItem('" + cmdUpdate.UniqueID + "', " + (hdnItemId.Value.Length > 0 ? "true" : "false") + ");";
        }

        protected void OnSave(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "a1", "alert('done" + hdnThumbFileName.Value + "');", true);
            //return;

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

            string thumbUrl = CopyUploadedFile(flThumb, true);
            string imageUrl = CopyUploadedFile(flImage, false);


            if (itemId > 0) {

                //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "a1", "alert('done" + hdnThumbFileName.Value + "');", true);
                ItemInfo existingItem = fsCtrl.GetItemById(itemId);

                if (thumbUrl == null) {
                    thumbUrl = existingItem.ThumbnailUrl;
                } else {
                    try {
                        if (thumbUrl.Length > 0) { // delete old thumbnail
                            System.IO.File.Delete(Server.MapPath(existingItem.ThumbnailUrl));
                        }
                    } catch {
                    }
                }

                if (imageUrl == null) {
                    imageUrl = existingItem.ImageUrl;
                } else {
                    try {
                        if (imageUrl.Length > 0) { // delete old thumbnail
                            System.IO.File.Delete(Server.MapPath(existingItem.ImageUrl));
                        }
                    } catch {
                    }
                }

                //string imageFile = System.IO.Path.GetFileName(flImage.FileName);

                //string thumbUrl = hdnThumbFileName.Value.Length > 0 ? PortalSettings.HomeDirectory + "FastShot/" + PMod + "/thumbs/" + hdnThumbFileName.Value : "";
                //string imageUrl = hdnImageFileName.Value.Length > 0 ? PortalSettings.HomeDirectory + "FastShot/" + PMod + "/images/" + hdnImageFileName.Value : "";

                fsCtrl.UpdateItem(itemId, PMod, txtTitle.Text, txtDesc.Text, thumbUrl, imageUrl, Convert.ToInt32(viewOrder), cbAutoGenerate.Checked);
            } else {
                
                if (thumbUrl == null) {
                    thumbUrl = "";
                }

                if (imageUrl == null) {
                    imageUrl = ""; // TODO: could this be null?
                }

                fsCtrl.AddItem(PMod, txtTitle.Text, txtDesc.Text, thumbUrl, imageUrl, Convert.ToInt32(viewOrder), cbAutoGenerate.Checked);
            }


            //if (RenderProc != null) {
            //    RenderProc();
            //}
            //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "closeConf", "avt.FastShot.$('#newItem').dialog('close').find(':input').val(''); avt.FastShot.$('.ui-dialog-overlay').remove(); document.getElementById('" + sThumbName.ClientID + "').innerHTML = ''; document.getElementById('" + sImageName.ClientID + "').innerHTML = '';", true);
            //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "reinitLighbox", "avt.FastShot.core.init();", true);
            
            //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "a21", "alert('done" + Parent.Parent.Parent.GetType() + "');", true);
            //((IMain)Parent.Parent.Parent).RenderItems();
            //((UpdatePanel)Parent.FindControl("upnlRender")).Update();
            //this.Visible = false;
        }

        protected void OnCancel(object sender, EventArgs e)
        {
        }

        private string CopyUploadedFile(FileUpload fu, bool isThumb)
        {
            if (fu == null || fu.PostedFile == null || fu.PostedFile.ContentLength <= 0) {
                return null;
            }

            // copy files to their location
            string fastShotRoot = PortalSettings.HomeDirectoryMapPath + "\\FastShot";
            if (!System.IO.Directory.Exists(fastShotRoot)) {
                System.IO.Directory.CreateDirectory(fastShotRoot);
            }

            string fastShotRootModule = fastShotRoot + "\\" + PMod;
            if (!System.IO.Directory.Exists(fastShotRootModule)) {
                System.IO.Directory.CreateDirectory(fastShotRootModule);
            }

            string imagePath = fastShotRootModule + (isThumb ? "\\thumbs" : "\\images");

            if (!System.IO.Directory.Exists(imagePath)) {
                System.IO.Directory.CreateDirectory(imagePath);
            }

            string filename = System.IO.Path.GetFileName(fu.PostedFile.FileName);
            filename = DateTime.Now.ToFileTime().ToString() + "_" + filename;
            fu.PostedFile.SaveAs(imagePath + "\\" + filename);

            return PortalSettings.HomeDirectory + "FastShot/" + PMod.ToString() + (isThumb ? "/thumbs/" : "/images/") + filename;
        }

    }

}
