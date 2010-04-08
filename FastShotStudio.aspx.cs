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
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Entities.Users;
using System.Net;
using DotNetNuke.Entities.Portals;
using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Services.Search;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using System.IO;

namespace avt.FastShot
{
    public partial class FastShotStudio : DotNetNuke.Framework.CDefault
    {
        protected PortalInfo _portal = null;

        protected void Page_Init(object sender, EventArgs e)
        {
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();


            if (!DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().IsInRole(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().AdministratorRoleName)) {
                Response.Write("Access denied!");
                try { Response.End(); } catch { }
                return;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // validate module
            // load module
            ModuleInfo _mod = null;
            try {
                ModuleController modCtrl = new ModuleController();
                int mid = Convert.ToInt32(Request.Params["mid"]);

                ArrayList mods = modCtrl.GetModuleTabs(mid);
                if (mods == null || mods.Count == 0) {
                    throw new Exception();
                }

                _mod = (ModuleInfo)mods[0];
                if (_mod == null || _mod.ModuleID <= 0) {
                    throw new Exception();
                }
            } catch (Exception ex) {
                _mod = null;
            }

            if (_mod == null) {
                Response.Write("Invalid Module!");
            } else {


                if (Request.Files != null && Request.Files.Count > 0) {
                    try {
                        string parentFolder = uploadPath.Value;
                        if (parentFolder.Length > 0 && parentFolder[parentFolder.Length - 1] != '\\')
                            parentFolder += '\\';

                        string path = PortalSettings.HomeDirectoryMapPath + parentFolder + Path.GetFileName(Request.Files[0].FileName);
                        if (File.Exists(path)) {
                            if (cbUploadOverwrite.Checked) {
                                File.Delete(path);
                            } else {
                                throw new Exception("A file with the same name already exists. ");
                            }
                        }

                        Request.Files[0].SaveAs(path);
                        Response.Write(uploadPath.Value);
                    } catch (Exception ex) {
                        Response.Write("Error uploading file (Server response: " + ex.Message + ")");
                    }

                    Response.End();
                }



                if (!Page.IsPostBack) {

                    try {
                        // load module settings
                        ModuleController modCtrl = new ModuleController();
                        int mid = Convert.ToInt32(Request.Params["mid"]);

                        ArrayList mods = modCtrl.GetModuleTabs(mid);
                        if (mods == null || mods.Count == 0) {
                            throw new Exception();
                        }

                        ModuleInfo mod = (ModuleInfo)mods[0];
                        if (mod == null || mod.ModuleID <= 0) {
                            throw new Exception();
                        }

                        FastShotSettings fsSettings = new FastShotSettings();
                        fsSettings.Load(mod.ModuleID);

                        // load templates
                        List<string> templates = new List<string>();
                        foreach (string dir in System.IO.Directory.GetDirectories(Server.MapPath(TemplateSourceDirectory + "/templates/"))) {
                            if (System.IO.Path.GetFileName(dir)[0] == '.') {
                                continue;
                            }
                            foreach (string subDir in System.IO.Directory.GetDirectories(dir)) {
                                if (System.IO.Path.GetFileName(subDir)[0] == '.') {
                                    continue;
                                }
                                templates.Add(JsonEncode(System.IO.Path.GetFileName(dir) + "/" + System.IO.Path.GetFileName(subDir)));
                            }
                        }

                        AJAX.RegisterScriptManager();
                        ScriptManager.RegisterStartupScript(this, GetType(), "initSettings", "avt.fs.settings = {mid: " + fsSettings.ModuleId.ToString() + ", title: '" + JsonEncode(mod.ModuleTitle) + "', template: '" + JsonEncode(fsSettings.Template) + "', thumb_w: " + fsSettings.ThumbWidth + ", thumb_h: " + fsSettings.ThumbHeight + "};", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "initTemplates", "avt.fs.templates = [\"" + string.Join("\",\"", templates.ToArray()) + "\"];", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "initReturnUrl", "avt.fs.returnUrl = \""+ DotNetNuke.Common.Globals.NavigateURL(mod.TabID) +"\";", true);
                        
                    } catch {
                        ScriptManager.RegisterStartupScript(this, GetType(), "initSettings", "avt.fs.settings = {mid: -1, title = 'Invalid Module'};", true);
                    }

                    FastShotController fsCtrl = new FastShotController();
                    if (fsCtrl.IsActivated()) {
                        btnActivate.Visible = false;
                    } else {
                        btnActivate.Attributes["onclick"] = "window.location='" + TemplateSourceDirectory + "/Activation.aspx?rurl=" + Server.UrlEncode(Request.Url.OriginalString) + "';";
                    }
                }
            }

            //AJAX.RegisterScriptManager();
            //SearchBoostController sbCtrl = new SearchBoostController();
            //PortalController portalCtrl = new PortalController();

            //// init instances
            //if (DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().IsSuperUser) {
            //    ScriptManager.RegisterStartupScript(this, GetType(), "initInstAll", "avt.sb.inst = " + GetInstAllJson() + "; avt.sb.inst.byPortal = {};", true);
            //    foreach (PortalInfo portaInfo in portalCtrl.GetPortals()) {
            //        ScriptManager.RegisterStartupScript(this, GetType(), "initInstP" + portaInfo.PortalID.ToString(), "avt.sb.inst.byPortal[" + portaInfo.PortalID.ToString() + "] = {caption: '"+ portaInfo.PortalName +" (Id: "+ portaInfo.PortalID.ToString() +")', data: " + GetInstJson(portaInfo.PortalID) + "};", true);
            //    }
            //} else {
            //    ScriptManager.RegisterStartupScript(this, GetType(), "initInstAll", "avt.sb.inst = " + GetInstAllJson(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId) + ";", true);
            //}

            //// init instances on current portal
            //ScriptManager.RegisterStartupScript(this, GetType(), "initInstC", "avt.sb.inst.cPortal = " + GetInstJson(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId) + ";", true);

            //// init custom rules
            //ScriptManager.RegisterStartupScript(this, GetType(), "initSearchTargets", "avt.sb.targets = " + GetRulesJson() + ";", true);

            //// set return url
            //if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.ToString()) && !Request.UrlReferrer.ToString().Contains("SearchBoostStudio.aspx") && !Request.UrlReferrer.ToString().Contains("/avt.SearchBoost/Activation.aspx")) {
            //    ScriptManager.RegisterStartupScript(this, GetType(), "initReturnUrl", "avt.sb.returnUrl = '" + Request.UrlReferrer.ToString() + "';", true);
            //} else {
            //    ScriptManager.RegisterStartupScript(this, GetType(), "initReturnUrl", "avt.sb.returnUrl = '" + HttpRuntime.AppDomainAppVirtualPath + "';", true);
            //}

            //// set if superuser
            //if (DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().IsSuperUser) {
            //    ScriptManager.RegisterStartupScript(this, GetType(), "initSuperUSer", "avt.sb.isSuperUser = true;", true);
            //}

            //// check if we need to load an instance on startup
            //if (!string.IsNullOrEmpty(Request.QueryString["iinst"])) {
            //    SearchInstance searchInst = sbCtrl.GetInstance(Request.QueryString["iinst"]);
            //    if (searchInst != null) {
            //        ScriptManager.RegisterStartupScript(this, GetType(), "initCurrentInstance", "avt.sb.current = '" + Request.QueryString["iinst"] + "';", true);
            //    }
            //}

            //if (sbCtrl.IsActivated()) {
            //    btnActivate.Visible = false;
            //} else {
            //    btnActivate.Attributes["onclick"] = "window.location='" + TemplateSourceDirectory + "/Activation.aspx?rurl=" + Server.UrlEncode(Request.Url.OriginalString) + "';";
            //}
        }


        private string JsonEncode(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            char c;
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            string t;

            //sb.Append('"');
            for (i = 0; i < len; i += 1) {
                c = s[i];
                if ((c == '\\') || (c == '"') || (c == '>') || (c == '\'')) {
                    sb.Append('\\');
                    sb.Append(c);
                } else if (c == '\b')
                    sb.Append("\\b");
                else if (c == '\t')
                    sb.Append("\\t");
                else if (c == '\n')
                    sb.Append("\\n");
                else if (c == '\f')
                    sb.Append("\\f");
                else if (c == '\r')
                    sb.Append("\\r");
                else {
                    if (c < ' ') {
                        //t = "000" + Integer.toHexString(c); 
                        string tmp = new string(c, 1);
                        t = "000" + int.Parse(tmp, System.Globalization.NumberStyles.HexNumber);
                        sb.Append("\\u" + t.Substring(t.Length - 4));
                    } else {
                        sb.Append(c);
                    }
                }
            }
            //sb.Append('"');
            return sb.ToString();
        }


        private string StripHtml(string htmlContent)
        {
            // apply the regular expression to strip tags
            return Regex.Replace(htmlContent, @"<(.|\n)*?>", string.Empty);
        }


    }
}