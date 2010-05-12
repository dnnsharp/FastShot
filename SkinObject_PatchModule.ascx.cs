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
    public partial class PatchModules : PortalModuleBase, IMain
    {
        public int PatchModuleId = -1;

        /////////////////////////////////////////////////////////////////////////////////
        // EVENT HANDLERS

        protected void Page_Init(object sender, EventArgs e)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CDefault p = (CDefault)HttpContext.Current.Handler;
            p.ClientScript.RegisterClientScriptInclude("avt_jQuery_1_3_2_av3", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/jQuery-1.3.2.js?v=" + avt.FastShot.FastShotController.Build);
            p.ClientScript.RegisterClientScriptInclude("avtFastShot_1_5", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/avt.FastShot-1.5.js?v=" + avt.FastShot.FastShotController.Build);
            p.ClientScript.RegisterClientScriptInclude("jQueryLightbox_av3", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/jquery-lightbox/jquery.lightbox-av3.js?v=" + avt.FastShot.FastShotController.Build);
            //p.ClientScript.RegisterClientScriptBlock(this.GetType(), "initLightbox", "avt.fs.initLightBox('" + HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot');", true);
            p.AddStyleSheet("skinLightbox", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/jquery-lightbox/css/lightbox.css");
        }

        public void RenderItems()
        {
            
        }

    }

}
