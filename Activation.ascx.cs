using System;
using System.Web;
using System.Net;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;


using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;


namespace avt.FastShot
{
    public partial class Activation : PortalModuleBase
    {
        public event EventHandler OnActivateSuccess
        {
            add { Events.AddHandler("Activate", value); }
            remove { Events.RemoveHandler("Activate", value); }
        }

        public string GetOpenLink()
        {
            return "avt.Common.openActivation('#" + pnlActivateDlg.ClientID + "', '" + triggerActivate.UniqueID + "'); return false;";
        }


        protected void Page_Load(Object Sender, EventArgs args)
        {
        }


        protected void OnShowActivate(Object Sender, EventArgs args)
        {
            pnlActivate.Visible = true;
            pnlSuccess.Visible = false;

            // let's fill possible hostnames

            ddHosts.Items.Clear();

            PortalAliasController paCtrl = new PortalAliasController();
            foreach (DictionaryEntry de in paCtrl.GetPortalAliases()) {
                PortalAliasInfo paInfo = (PortalAliasInfo)de.Value;
                string httpAlias = paInfo.HTTPAlias;

                if (Regex.Match(httpAlias, ".*\\d+\\.\\d+\\.\\d+\\.\\d+.*").Length > 0) {
                    continue; // this is IP based alias
                }
                // remove port, if exists
                if (httpAlias.LastIndexOf(":") != -1) httpAlias = httpAlias.Substring(0, httpAlias.IndexOf(":"));
                // remove path
                if (httpAlias.LastIndexOf("/") != -1) httpAlias = httpAlias.Substring(0, httpAlias.IndexOf("/"));

                if (httpAlias == "localhost") {
                    continue;
                }

                if (ddHosts.Items.FindByText(httpAlias) != null) {
                    continue; // item already exists
                }

                ddHosts.Items.Add(new ListItem(httpAlias, httpAlias));
            }

            if (ddHosts.Items.Count == 0) {
                pnlActivate.Visible = false;
                pnlInvalidHost.Visible = true;
            }

            

            ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "enableInput", "avt.Common.enableInput();", true);
        }

        protected void OnActivate(Object Sender, EventArgs args)
        {
            ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "enableInput", "avt.Common.enableInput();", true);

            if (txtRegistrationCode.Text.Length == 0) {
                validateActivation.IsValid = false;
                validateActivation.Text = "Please provide the registration code";
                return;
            }

            string postData = "portal_id=" + PortalId.ToString();
            postData += "&install_key=" + FastShotController.GetInstallationKey();
            postData += "&reg_code=" + txtRegistrationCode.Text;
            postData += "&version=" + FastShotController.FastShotVersion;
            postData += "&hostname=" + ddHosts.SelectedValue;
            postData += "&aliases=" + GetPortalAliases();

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(FastShotController.RegSrv + "?cmd=activate");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = data.Length;
            httpRequest.Timeout = 20 * 1000;
            System.IO.Stream newStream = httpRequest.GetRequestStream();

            // Send the data.
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            string responseText = reader.ReadToEnd();
            response.Close();

            if (!responseText.Contains("Success:")) {
                validateActivation.IsValid = false;
                validateActivation.Text = responseText;
                return;
            }

            // it's a success, let's save activation code and show success screen
            string activations;
            try {
                activations = System.IO.File.ReadAllText(Server.MapPath(TemplateSourceDirectory + "\\activations.txt"));
            } catch (Exception) {
                activations = "";
            }

            activations += "\r\n" + responseText.Substring(9) + ":" + txtRegistrationCode.Text;
            System.IO.File.WriteAllText(Server.MapPath(TemplateSourceDirectory + "\\activations.txt"), activations);

            pnlActivate.Visible = false;
            pnlSuccess.Visible = true;
        }

        protected void OnCloseActivation(Object Sender, EventArgs args)
        {
            pnlActivate.Visible = false;
            pnlSuccess.Visible = false;

            if (Events["Activate"] != null) {
                ((EventHandler)Events["Activate"])(this, null);
            }

            ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "closeAct", "avt.Common.closeActivation('#" + pnlActivateDlg.ClientID + "'); avt.Common.enableInput();", true);
        }

        #region Helpers


        private string GetPortalAliases()
        {
            List<string> aliases = new List<string>();
            PortalAliasController paCtrl = new PortalAliasController();
            foreach (DictionaryEntry de in paCtrl.GetPortalAliases()) {
                PortalAliasInfo paInfo = (PortalAliasInfo) de.Value;
                aliases.Add(paInfo.HTTPAlias);
            }

            return String.Join(";", aliases.ToArray());
        }

        #endregion

    }
}