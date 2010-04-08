
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Web;
using System.Web.Configuration;
using System.Net;
using System.Web.UI.WebControls;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using System.Security.Cryptography;

using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Search;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using System.IO;
using System.Text.RegularExpressions;


namespace avt.FastShot
{

    public class FastShotSettings
    {
        public int ModuleId = -1;
        public string TemplateType = "LightBox";
        public string Template = "LightBox/default";
        public int ThumbWidth = 0;
        public int ThumbHeight = 100;


        public void Load(int moduleId)
        {
            ModuleController modCtrl = new ModuleController();
            Hashtable modSettings = modCtrl.GetModuleSettings(moduleId);

            ModuleId = moduleId;

            if (modSettings.ContainsKey("template")) {
                Template = modSettings["template"].ToString();
            }

            if (Template.IndexOf("/") == -1) {
                Template = "LightBox/" + Template;
            }

            TemplateType = Template.Substring(0, Template.IndexOf("/"));

            if (modSettings.ContainsKey("thumb_width")) {
                try {
                    ThumbWidth = Convert.ToInt32(modSettings["thumb_width"]);
                } catch {
                    ThumbWidth = 0;
                }
            }

            if (modSettings.ContainsKey("thumb_height")) {
                try {
                    ThumbHeight = Convert.ToInt32(modSettings["thumb_height"]);
                } catch {
                    ThumbHeight = 0;
                }
            }
        }
    }

}

