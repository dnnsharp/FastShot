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
using System.Text.RegularExpressions;

using DotNetNuke;
using DotNetNuke.Framework;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace avt.FastShot
{
    public partial class Upload : DotNetNuke.Framework.CDefault
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Files.Count == 1 && Request.Files[0].FileName.Length > 0) {

                int ModuleId = Convert.ToInt32(Request.Params["moduleId"]);

                // let's copy files to their location
                string fastShotRoot = PortalSettings.HomeDirectoryMapPath + "\\FastShot";
                if (!System.IO.Directory.Exists(fastShotRoot)) {
                    System.IO.Directory.CreateDirectory(fastShotRoot);
                }

                string fastShotRootModule = fastShotRoot + "\\" + ModuleId;
                if (!System.IO.Directory.Exists(fastShotRootModule)) {
                    System.IO.Directory.CreateDirectory(fastShotRootModule);
                }

                string imagePath;
                if (Request.Params["m"] == "thumb") {
                    imagePath = fastShotRootModule + "\\thumbs";
                } else {
                    imagePath = fastShotRootModule + "\\images";
                }

                if (!System.IO.Directory.Exists(imagePath)) {
                    System.IO.Directory.CreateDirectory(imagePath);
                }

                string filename = Request.Files[0].FileName;
                if (filename.IndexOf('\\') != -1) {
                    filename = filename.Substring(filename.LastIndexOf('\\') + 1);
                }
                filename = DateTime.Now.ToFileTime().ToString() + "_" + filename;
                Request.Files[0].SaveAs(imagePath + "\\" + filename);

                hdFilename.Value = filename;
                Response.Write("<span style = 'color: #00FF00; font-weight: bold;'>Success.</span>");
                pnlFile.Visible = false;
            } else {
                hdFilename.Value = "";
            }
        }
    }
}