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
    public partial class Main : PortalModuleBase, IMain
    {

        /////////////////////////////////////////////////////////////////////////////////
        // EVENT HANDLERS

        protected void Page_Init(object sender, EventArgs e)
        {
            //AJAX.RegisterScriptManager();

            // load css
            CDefault defaultPage = (CDefault)Page;
            FastShotSettings FsSettings = new FastShotSettings();
            FsSettings.Load(ModuleId);
            defaultPage.AddStyleSheet("FastShot." + FsSettings.Template, TemplateSourceDirectory + "/templates/" + FsSettings.Template + "/theme.css");

            RenderItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration)) {
                pnlSettings.Visible = true;
            } else {
                pnlSettings.Visible = false;
            }

        }

        public void RenderItems()
        {
            FastShotController fShotCtrl = new FastShotController();

            FastShotSettings FsSettings = new FastShotSettings();
            FsSettings.Load(ModuleId);

            bool isActivated = false;
            try {
                isActivated = fShotCtrl.IsActivated();
            } catch (System.Net.WebException) {
                isActivated = false;
            }

            List<ItemInfo> items = fShotCtrl.GetItems(ModuleId);

            StringBuilder strXML = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XmlWriter Writer = XmlWriter.Create(strXML, settings);

            Writer.WriteStartElement("fastshot");
            Writer.WriteElementString("root", TemplateSourceDirectory + "/");
            Writer.WriteElementString("mid", ModuleId.ToString());


            if (!isActivated) {
                Random rand = new Random();
                if (rand.Next(0, 20) == 1) {
                    items = new List<ItemInfo>();
                    items.Add(new ItemInfo() {
                        ImageUrl = "http://www.avatar-soft.ro/portals/0/product_logo/fastshot_large.png",
                        ThumbnailUrl = "http://www.avatar-soft.ro/portals/0/product_logo/fastshot_medium.png",
                        Title = "FastShot Demo",
                        Description = "This copy of FastShot is not activated! Visit <a href = 'http://www.avatar-soft.ro' style = 'font-weight: bold;'>www.avatar-soft.ro</a> to read more about FastShot.",
                        ThumbWidth = 115,
                        ThumbHeight = 110,
                        AutoGenerateThumb = false
                    });
                }
            }

            int maxThumbWidth = 0;
            int maxThumbHeight = 0;
            foreach (ItemInfo item in items) {
                if (item.ThumbWidth > maxThumbWidth) maxThumbWidth = item.ThumbWidth;
                if (item.ThumbHeight > maxThumbHeight) maxThumbHeight = item.ThumbHeight;
            }

            Writer.WriteElementString("max_thumb_width", maxThumbWidth.ToString());
            Writer.WriteElementString("max_thumb_height", maxThumbHeight.ToString());

            foreach (ItemInfo item in items) {
                Writer.WriteStartElement("img");
                Writer.WriteElementString("id", item.ItemId.ToString());
                Writer.WriteElementString("title", fShotCtrl.Tokenize(item.Title, ModuleConfiguration));
                Writer.WriteElementString("desc", fShotCtrl.Tokenize(item.Description, ModuleConfiguration));
                Writer.WriteElementString("thumburl", ResolveUrl(item.ThumbnailUrl));
                Writer.WriteElementString("imgurl", ResolveUrl(item.ImageUrl));
                Writer.WriteElementString("thumb_width", item.ThumbWidth.ToString());
                Writer.WriteElementString("thumb_height", item.ThumbHeight.ToString());
                Writer.WriteElementString("tplParams", item.TplParams);
                Writer.WriteEndElement(); //img
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
            itemContainer.InnerHtml += output.ToString();
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
            FastShotSettings FsSettings = new FastShotSettings();
            FsSettings.Load(ModuleId);

            CDefault defaultPage = (CDefault)Page;

            // doing this at another stage will break things work on IE
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("avt_jQuery_1_3_2")) {
                Page.ClientScript.RegisterClientScriptInclude("avt_jQuery_1_3_2_av3", TemplateSourceDirectory + "/js/jQuery-1.3.2.js?v=" + avt.FastShot.FastShotController.Build);
            }

            //if (!Page.ClientScript.IsClientScriptIncludeRegistered("avt_jQueryUi_1_7_2_av3")) {
            //    Page.ClientScript.RegisterClientScriptInclude("avt_jQueryUi_1_7_2_av3", TemplateSourceDirectory + "/js/jquery-ui-1.7.2.av3.js");
            //}

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("avtFastShot_1_5")) {
                Page.ClientScript.RegisterClientScriptInclude("avtFastShot_1_5", TemplateSourceDirectory + "/js/avt.FastShot-1.5.js?v=" + avt.FastShot.FastShotController.Build);
            }

            switch (FsSettings.TemplateType) {
                case "LightBox":
                    //if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQueryLightbox_av3")) {
                    Page.ClientScript.RegisterClientScriptInclude("jQueryLightbox_av3", TemplateSourceDirectory + "/js/jquery-lightbox/jquery.lightbox-av3.js?v=" + avt.FastShot.FastShotController.Build);
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "initLightbox", "avt.fs.initLightBox('" + TemplateSourceDirectory + "/');", true);
                        defaultPage.AddStyleSheet("skinLightbox", TemplateSourceDirectory + "/js/jquery-lightbox/css/lightbox.css");
                    //}
                    
                    break;
                case "SpaceGallery":
                    Page.ClientScript.RegisterClientScriptInclude("avtFsSpaceEye", TemplateSourceDirectory + "/js/SpaceGallery/js/eye.js?v=" + avt.FastShot.FastShotController.Build);
                    Page.ClientScript.RegisterClientScriptInclude("avtFsSpaceUtils", TemplateSourceDirectory + "/js/SpaceGallery/js/utils.js?v=" + avt.FastShot.FastShotController.Build);
                    Page.ClientScript.RegisterClientScriptInclude("avtFsSpaceGallery", TemplateSourceDirectory + "/js/SpaceGallery/js/spacegallery.js?v=" + avt.FastShot.FastShotController.Build);
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "initSpaceGallery", "avt.fs.initSpaceGallery('" + TemplateSourceDirectory + "/');", true);

                    defaultPage.AddStyleSheet("skinSpaceGallery", TemplateSourceDirectory + "/js/SpaceGallery/css/spacegallery.css");
                    break;
                case "Galleriffic":
                    Page.ClientScript.RegisterClientScriptInclude("avtFsGalleriffic", TemplateSourceDirectory + "/js/galleriffic-2.0/js/jquery.galleriffic.js?v=" + avt.FastShot.FastShotController.Build);
                    Page.ClientScript.RegisterClientScriptInclude("avtFsOpacityRollover", TemplateSourceDirectory + "/js/galleriffic-2.0/js/jquery.opacityrollover.js?v=" + avt.FastShot.FastShotController.Build);
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "initGalleriffic", "avt.fs.initGalleriffic('" + TemplateSourceDirectory + "/');", true);

                    //defaultPage.AddStyleSheet("skinGallerffic", TemplateSourceDirectory + "/js/galleriffic-2.0/css/galleriffic-5.css");
                    break;
                case "s3Slider":
                    Page.ClientScript.RegisterClientScriptInclude("avtFss3Slider", TemplateSourceDirectory + "/js/s3Slider/js/s3Slider.js?v=" + avt.FastShot.FastShotController.Build);
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "inits3Slider", "avt.fs.inits3Slider('" + TemplateSourceDirectory + "/');", true);
                    break;
            }

            

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "initGrid" + ModuleId.ToString(), "avt.fs.$(document).ready(function() { avt.fs.initGrid(avt.fs.$('#" + itemContainer.ClientID + "').find('.FastShot_grid')); });", true);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "avtFsInit", "avt.fs.$$.init({appPath:'" + Request.ApplicationPath + "', loaderIcon : '" + TemplateSourceDirectory + "/res/loader.gif'});", true);

            base.OnPreRender(e);
        }

    }

}
