using System;
using System.IO;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Net;
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
    public partial class MakeThumb : System.Web.UI.Page // DotNetNuke.Framework.CDefault
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            //System.Threading.Thread.Sleep(1000);

            // create an image object, using the filename we just retrieved
            System.Drawing.Image image;
            string file = null;
            try {
                if (Server.UrlDecode(Request.QueryString["file"]) == "http://www.avatar-soft.ro/portals/0/product_logo/fastshot_large.png") {
                    image = LoadImageFromURL(Server.UrlDecode(Request.QueryString["file"]));
                } else {
                    file = Server.UrlDecode(Request.QueryString["file"]);
                    try {
                        image = System.Drawing.Image.FromFile(Server.MapPath(file));
                        file = Server.MapPath(file);
                    } catch (Exception ex) {
                        // give it another chance
                        if (file[0] != '~') {
                            file = "~" + file;
                            image = System.Drawing.Image.FromFile(Server.MapPath(file));
                            file = Server.MapPath(file);
                        } else {
                            throw ex;
                        }
                    }
                }
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
            //Response.Write(width.ToString() + ":" + height.ToString());
            //return;

            // create the actual thumbnail image
            System.Drawing.Bitmap thumbnailImage = new System.Drawing.Bitmap(image, width, height);
            //System.Drawing.Image thumbnailImage = image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

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
            Response.Cache.SetCacheability(HttpCacheability.Public);
            if (file != null) {
                Response.AddFileDependency(file);
                Response.Cache.SetLastModifiedFromFileDependencies();
            }

            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.Now.AddDays(365));
            Response.ContentType = "image/jpeg";
            Response.BinaryWrite(imageContent);
        }

        //public bool ThumbnailCallback()
        //{
        //    return true;
        //}

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
    }

}
