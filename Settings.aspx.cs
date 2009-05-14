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
    public partial class Settings : DotNetNuke.Framework.CDefault
    {
        private int PMod;
        private FastShotSettings FsSettings;

        /////////////////////////////////////////////////////////////////////////////////
        // Module actions

     


        /////////////////////////////////////////////////////////////////////////////////
        // EVENT HANDLERS

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
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

                    FsSettings = new FastShotSettings();
                    FsSettings.Load(PMod);

                } catch {
                    Response.Write("Access denied!");
                    Response.End();
                }
            } catch {
                Response.Write("Access denied!");
                Response.End();
            }

            // load templates
            foreach (string dir in System.IO.Directory.GetDirectories(Server.MapPath(TemplateSourceDirectory + "/templates/"))) {
                if (System.IO.Path.GetFileName(dir)[0] == '.') {
                    continue;
                }
                ddTemplate.Items.Add(new ListItem(System.IO.Path.GetFileName(dir), dir));
            }

            // load setings
            if (!Page.IsPostBack) {
                txtModuleId.Text = PMod.ToString();
                txtThumbHeight.Text = FsSettings.ThumbHeight.ToString();
                txtThumbWidth.Text = FsSettings.ThumbWidth.ToString();

                
                if (ddTemplate.Items.FindByText(FsSettings.Template) != null) {
                    ddTemplate.Items.FindByText(FsSettings.Template).Selected = true;
                }
            }
            //ScriptManager.RegisterStartupScript(upnlAddEdit, upnlAddEdit.GetType(), "enableInput", "avt.Common.enableInput();", true);
        }

        protected void OnSave(object sender, EventArgs e)
        {
            ModuleController modCtrl = new ModuleController();
            modCtrl.UpdateModuleSetting(PMod, "template", ddTemplate.SelectedItem.Text);
            modCtrl.UpdateModuleSetting(PMod, "thumb_width", txtThumbWidth.Text);
            modCtrl.UpdateModuleSetting(PMod, "thumb_height", txtThumbHeight.Text);
        }

        protected void OnCancel(object sender, EventArgs e)
        {
        }

    }

}
