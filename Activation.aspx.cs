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
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;


using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;


namespace avt.FastShot
{
    

    public partial class ActivationWnd : PageBase
    {
        public event EventHandler OnActivateSuccess
        {
            add { Events.AddHandler("Activate", value); }
            remove { Events.RemoveHandler("Activate", value); }
        }


        protected void Page_Init(Object Sender, EventArgs args)
        {
            //if (Request.QueryString["cmd"] != null && Request.QueryString["cmd"] == "srvkey") {
            //    // this needs to be put in by someone before the request
            //    Response.Write(Application["NavXP_SrvTmpKey"]);
            //    Response.End();
            //}

            if (Request.QueryString["cmd"] != null && Request.QueryString["cmd"] == "prtlid") {
                int portalId = PortalController.GetCurrentPortalSettings().PortalId;
                Response.Write(portalId.ToString());

                //SHA1 sha1 = new SHA1CryptoServiceProvider();
                //byte[] hashBytes = sha1.ComputeHash(Encoding.Unicode.GetBytes(portalId.ToString() + "Xfsk2"));
                //string hash = "";
                //for (int i = 0; i < hashBytes.Length / 2; ++i) {
                //    hash += hashBytes[i].ToString("X2");
                //}

                //Response.Write(portalId.ToString() + "-" + hash);
                Response.End();
            }

            if (Request.QueryString["cmd"] != null && Request.QueryString["cmd"] == "clear") {
                HttpContext.Current.Application["FastShotActivation"] = new Hashtable();
                HttpContext.Current.Application["FastShotActivationToken"] = new Hashtable();
                Response.End();
            }

            AJAX.RegisterScriptManager();
        }

        protected void Page_Load(Object Sender, EventArgs args)
        {
            AJAX.RegisterScriptManager();

            pnlActivate.Visible = true;
            pnlSuccess.Visible = false;

            // let's fill possible hostnames
            if (!Page.IsPostBack) {
                
            }

            if (Page.IsPostBack) {
                ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "load", "avt.fastshot.$$.frameLoaded(parent, window);", true);
            }
        }

        protected void OnNext(Object Sender, EventArgs args)
        {

            RegistrationCode regCode;
            try {
                regCode = new RegistrationCode(txtRegistrationCode.Text);
                if (!regCode.IsValid()) {
                    throw new Exception();
                }
            } catch {
                validateActivation.Text = "The registration code you supplied is invalid.";
                validateActivation.IsValid = false;
                return;
            }

            trDomains.Visible = true;
            //ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "init1", "alert('" + regCode.ProductCode + "');", true);

            switch (regCode.VariantCode) {
                case "DOM":
                    lblDomainTitle.Text = "Domain";
                    lblDomain.Visible = true;
                    FillHosts(false);
                    break;
                case "XDOM":
                    lblDomainTitle.Text = "Domain";
                    lblDomain.Visible = true;
                    FillHosts(false);
                    break;
                case "PRTL":
                    lblDomainTitle.Text = "Primary Domain";
                    lblPrimaryDomain.Visible = true;
                    FillHosts(true);
                    break;
                case "SRV":
                    lblDomainTitle.Text = "Primary Domain";
                    lblPrimaryDomain.Visible = true;
                    FillHosts(true);
                    break;
            }

            btnActivate.Visible = true;
            btnNext.Visible = false;
            //txtRegistrationCode.ReadOnly = true;
        }

        protected void OnActivate(Object Sender, EventArgs args)
        {
            if (txtRegistrationCode.Text.Length == 0) {
                validateActivation.IsValid = false;
                validateActivation.Text = "Please provide the registration code";
                return;
            }
            
            //ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "a1", "alert('a1');", true);
            PortalSettings cPortal = DotNetNuke.Common.Globals.GetPortalSettings();

            Random randGen = new Random();
            string srvTempKey = FastShotController.GetInstallationKey() + randGen.Next().ToString();
            System.IO.File.WriteAllText(Server.MapPath(TemplateSourceDirectory + "/tmpkey.txt"), srvTempKey);

            string postData = "";
            postData += "reg_code=" + txtRegistrationCode.Text;
            postData += "&version=" + FastShotController.FastShotVersion;
            postData += "&hostname=" + ddHosts.SelectedValue;
            postData += "&aliases=" + GetPortalAliases();
            postData += "&portal_id=" + PortalSettings.PortalId;
            postData += "&app_path=" + TemplateSourceDirectory;
            postData += "&server_key=" + srvTempKey;

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(FastShotController.RegSrv + "?cmd=activate");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = data.Length;
            httpRequest.Timeout = 120 * 1000;
            System.IO.Stream newStream = httpRequest.GetRequestStream();

            // Send the data.
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            string responseText = reader.ReadToEnd();
            response.Close();

            // clear temp key
            System.IO.File.Delete(Server.MapPath(TemplateSourceDirectory + "/tmpkey.txt"));

            if (!responseText.Contains("Success:")) {
                validateActivation.IsValid = false;
                validateActivation.Text = responseText;
                return;
            }

            DataProvider.Instance().AddActivation(responseText.Substring(9), txtRegistrationCode.Text, ddHosts.SelectedValue, "FSHOT", true, null);


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

            ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "avtRefreshMainScript", "window.avtRefreshMain = true;", true);
            ScriptManager.RegisterStartupScript(upnlActivate, upnlActivate.GetType(), "avtChangeToClose", "avt.fastshot.$(parent.document).find('.ui-dialog').find('[rel=cancel]').text('Close');", true);
        }



        #region Helpers

        private void FillHosts(bool bIncludeIpAddresses)
        {
            ddHosts.Items.Clear();

            PortalAliasController paCtrl = new PortalAliasController();
            foreach (DictionaryEntry de in paCtrl.GetPortalAliases()) {
                PortalAliasInfo paInfo = (PortalAliasInfo)de.Value;
                string httpAlias = paInfo.HTTPAlias;

                if (!bIncludeIpAddresses && Regex.Match(httpAlias, ".*\\d+\\.\\d+\\.\\d+\\.\\d+.*").Length > 0) {
                    continue; // this is IP based alias
                }
                // remove port, if exists
                if (!bIncludeIpAddresses && httpAlias.LastIndexOf(":") != -1) httpAlias = httpAlias.Substring(0, httpAlias.IndexOf(":"));

                // remove path
                if (httpAlias.LastIndexOf("/") != -1) httpAlias = httpAlias.Substring(0, httpAlias.IndexOf("/"));

                // remove www.
                if (httpAlias.IndexOf("www.") != -1) httpAlias = httpAlias.Substring(httpAlias.IndexOf("www.") + 4);
                if (httpAlias.IndexOf("dev.") != -1) httpAlias = httpAlias.Substring(httpAlias.IndexOf("dev.") + 4);
                if (httpAlias.IndexOf("staging.") != -1) httpAlias = httpAlias.Substring(httpAlias.IndexOf("staging.") + 8);

                if (httpAlias.IndexOf("localhost") == 0) {
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
        }

        private string GetPortalAliases()
        {
            List<string> aliases = new List<string>();
            PortalAliasController paCtrl = new PortalAliasController();
            foreach (DictionaryEntry de in paCtrl.GetPortalAliases()) {
                PortalAliasInfo paInfo = (PortalAliasInfo)de.Value;
                aliases.Add(paInfo.HTTPAlias);
            }

            return String.Join(";", aliases.ToArray());
        }


        #endregion

    }
}