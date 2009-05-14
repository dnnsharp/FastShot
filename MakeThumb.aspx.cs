using System;
using System.IO;
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
    public partial class MakeThumb : DotNetNuke.Framework.CDefault
    {
        private void Page_Load(object sender, System.EventArgs e)
        {

            // create an image object, using the filename we just retrieved
            System.Drawing.Image image; 
            try {
                image = System.Drawing.Image.FromFile(Server.MapPath(Server.UrlDecode(Request.QueryString["file"])));
            } catch {
                Response.Write("File not found");
                return;
            }

            int height = -1;
            int width = -1;

            try {
                height = Convert.ToInt32(Request.QueryString["height"]);
            } catch (Exception) {
                height = -1;
            }

            try {
                width = Convert.ToInt32(Request.QueryString["width"]);
            } catch (Exception) {
                width = -1;
            }

            if (height <= 0 && width <= 0) {
                height = 64;
                width = 64;
            } else if (height <= 0) {
                height = width * image.Height / image.Width;
            } else if (width <= 0) {
                width = height * image.Width / image.Height;
            }


            // create the actual thumbnail image
            System.Drawing.Image thumbnailImage = image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

            // make a memory stream to work with the image bytes
            MemoryStream imageStream = new MemoryStream();

            // put the image into the memory stream
            thumbnailImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            // make byte array the same size as the image
            byte[] imageContent = new Byte[imageStream.Length];

            // rewind the memory stream
            imageStream.Position = 0;

            // load the byte array with the image
            imageStream.Read(imageContent, 0, (int)imageStream.Length);

            // return byte array to caller with image type
            Response.ContentType = "image/jpeg";
            Response.BinaryWrite(imageContent);
        }

        public bool ThumbnailCallback()
        {
            return true;
        }
    }

}
