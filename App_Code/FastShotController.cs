
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


namespace avt.FastShot
{
    public interface IMain
    {
        void RenderItems();
    }

    public class FastShotSettings
    {
        public int ModuleId = -1;
        public string Template = "default";
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

            if (modSettings.ContainsKey("thumb_width")) {
                ThumbWidth = Convert.ToInt32(modSettings["thumb_width"]);
            }

            if (modSettings.ContainsKey("thumb_height")) {
                ThumbHeight = Convert.ToInt32(modSettings["thumb_height"]);
            }
        }
    }


    public class RegistrationCode
    {
        static Random rGen = new Random();

        string _RegCode;
        string _prodCode;
        string _variantCode;
        string _hashCheck;
        string _custPart;
        string _randPart;

        DateTime _dateExpire = Null.NullDate;

        public bool HasTimeBomb
        {
            get { return _dateExpire != Null.NullDate; }
        }

        public string ProductCode
        {
            get { return _prodCode; }
        }

        public string VariantCode
        {
            get { return _variantCode; }
        }

        public DateTime DateExpire
        {
            get { return _dateExpire; }
        }

        public bool IsExpired()
        {
            return HasTimeBomb && _dateExpire < DateTime.Now;
        }

        public RegistrationCode(string regCode)
        {
            _RegCode = regCode;

            // parse parts
            string[] parts = regCode.Split('-');
            int iPart = 0;
            _prodCode = parts[iPart++];
            _variantCode = parts[iPart++];
            if (parts.Length == 4) { // has timebomb
                DateTime centuryBegin = new DateTime(2001, 1, 1);
                _dateExpire = centuryBegin.AddDays(Convert.ToInt32(parts[iPart++]));
            }

            _hashCheck = parts[iPart].Substring(0, 20);
            _custPart = parts[iPart].Substring(21);
            _randPart = parts[iPart].Substring(29);
        }

        private RegistrationCode() // private constructor called via Generate
        {
        }


        private void Generate(string productCode, string variantCode, DateTime dateExpire, string customerID)
        {
            _RegCode = productCode + "-" + variantCode + "-";
            _prodCode = productCode;
            _variantCode = variantCode;

            // check time bomb
            if (dateExpire > DateTime.Now) {
                DateTime centuryBegin = new DateTime(2001, 1, 1);
                long elapsedTicks = dateExpire.Ticks - centuryBegin.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                _RegCode += elapsedSpan.Days.ToString("D5"); // days from century - 5 chars
                _RegCode += "-";

                _dateExpire = dateExpire;
            }

            // let's sign data
            string token = _RegCode + "GapuFR3cUBrefAnA";

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] hashBytes = sha1.ComputeHash(Encoding.Unicode.GetBytes(token));
            string hash = "";
            for (int i = 0; i < hashBytes.Length / 2; ++i) {
                hash += hashBytes[i].ToString("X2");
            }

            _RegCode += hash; // hash check - 20 chars
            _hashCheck = hash;

            // add some customer ID
            byte[] custBytes = sha1.ComputeHash(Encoding.Unicode.GetBytes(customerID));
            string custPart = "";
            for (int i = 0; i < 4; ++i) {
                custPart += custBytes[i].ToString("X2"); // 8 bytes
            }
            _RegCode += custPart;
            _custPart = custPart;

            // finally, add some random
            string randPart = rGen.Next(999999).ToString("D6");
            _RegCode += randPart;
            _randPart = randPart;
        }

        private void Generate(string productCode, string variantCode, int valabilityDays, string customerID)
        {
            DateTime dateExpire = DateTime.Now.AddDays(valabilityDays);
            Generate(productCode, variantCode, dateExpire, customerID);
        }

        public override string ToString()
        {
            return _RegCode;
        }

        public bool IsValid()
        {
            RegistrationCode regCode = new RegistrationCode();
            regCode.Generate(_prodCode, _variantCode, _dateExpire, "");

            //HttpContext.Current.Response.Write("R:" + ToString() + "<br />");
            //HttpContext.Current.Response.Write("Check:" + regCode.ToString() + "<br />");

            return regCode.ToString().Substring(0, regCode.ToString().Length - 14 /*custPart+randPart*/)
                 == ToString().Substring(0, ToString().Length - 14 /*custPart+randPart*/);
        }

        public string Activate(string host, bool bPrimary)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();

            string token = host + _RegCode + _randPart;
            if (!bPrimary && _variantCode == "PRTL") {
                token += DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId.ToString();
            }

            byte[] hashBytes = sha1.ComputeHash(Encoding.Unicode.GetBytes(token));
            string hash = "";
            for (int i = 0; i < hashBytes.Length; ++i) {
                hash += hashBytes[i].ToString("X2");
            }
            return hash;
        }

        public string IsActivationValid(string activationCode, bool bPrimary, bool isNew, string appPath, string baseActivationCode)
        {
            // let's ask the server about that
            try {
                string hostname = HttpContext.Current.Request.Url.Host;

                // remove www.
                if (hostname.IndexOf("www.") != -1) hostname = hostname.Substring(hostname.IndexOf("www.") + 4);
                if (hostname.IndexOf("dev.") != -1) hostname = hostname.Substring(hostname.IndexOf("dev.") + 4);
                if (hostname.IndexOf("staging.") != -1) hostname = hostname.Substring(hostname.IndexOf("staging.") + 8);

                Random randGen = new Random();
                string srvTempKey = FastShotController.GetInstallationKey() + randGen.Next().ToString();
                System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath(appPath + "/tmpkey.txt"), srvTempKey);

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(FastShotController.RegSrv + "?cmd=validate_activation&reg_code=" + _RegCode + "&activation_code=" + activationCode + "&primary=" + (bPrimary ? "true" : "false") + "&host=" + hostname + "&srv_key=" + srvTempKey + "&prtlid=" + DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId.ToString() + "&app_path=" + appPath + "&base_activation_code=" + baseActivationCode);
                httpRequest.Timeout = 120 * 1000;
                HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();

                // clear temp key
                System.IO.File.Delete(HttpContext.Current.Server.MapPath(appPath + "/tmpkey.txt"));

                System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                string responseText = reader.ReadToEnd();
                response.Close();

                if (!responseText.Contains("Success")) {
                    return null;
                }

                if (isNew) {
                    // this is a new activation, let's create new activation
                    activationCode = Activate(hostname, false);
                    DataProvider.Instance().AddActivation(activationCode, _RegCode, hostname, "FSHOT", false, baseActivationCode);
                }

                return activationCode;

            } catch {
                return null;
            }
        }
    }


    public class ActivationInfo
    {
        string _ActivationCode;
        string _RegistrationCode;
        string _Host;
        string _ProductCode;
        bool _IsPrimary;
        string _BaseActivationCode;

        public string ActivationCode
        {
            get { return _ActivationCode; }
            set { _ActivationCode = value; }
        }

        public string RegistrationCode
        {
            get { return _RegistrationCode; }
            set { _RegistrationCode = value; }
        }

        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }

        public string ProductCode
        {
            get { return _ProductCode; }
            set { _ProductCode = value; }
        }

        public bool IsPrimary
        {
            get { return _IsPrimary; }
            set { _IsPrimary = value; }
        }

        public string BaseActivationCode
        {
            get { return _BaseActivationCode; }
            set { _BaseActivationCode = value; }
        }

    }


    public class ItemInfo
    {
        int _ItemId;
        int _ModuleId;
        string _ItemTitle;
        string _ItemDescription;
        string _ThumbUrl;
        string _ImageUrl;
        int _ViewOrder;
        bool _AutoGenerateThumb;

        public int ItemId {
            get { return _ItemId; }
            set { _ItemId = value; }
        }

        public int ModuleId {
            get { return _ModuleId; }
            set { _ModuleId = value; }
        }

        public string Title {
            get { return _ItemTitle; }
            set { _ItemTitle = value; }
        }

        public string Description {
            get { return _ItemDescription; }
            set { _ItemDescription = value; }
        }

        public string ThumbnailUrl {
            get { return _ThumbUrl; }
            set { _ThumbUrl = value; }
        }

        public string ImageUrl {
            get { return _ImageUrl; }
            set { _ImageUrl = value; }
        }

        public int ViewOrder {
            get { return _ViewOrder; }
            set { _ViewOrder = value; }
        }

        public bool AutoGenerateThumb {
            get { return _AutoGenerateThumb; }
            set { _AutoGenerateThumb = value; }
        }

    }
    

    public class FastShotController
    {
        static public string RegSrv = "http://www.avatar-soft.ro/RegCoreApi2.aspx";
        //static public string RegSrv = "http://devx.avt.2am.ro:8080/RegCoreApi.aspx";
        static public string BuyLink = "http://www.snowcovered.com/snowcovered2/Default.aspx?tabid=242&PackageID=14359&r=bf0821d1ea";
        static public string FastShotVersion = "1.2";
        static public string FastShotVersionAll = "1.2.0";

        public int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb)
        {
            return DataProvider.Instance().AddItem(moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb);
        }

        public void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb)
        {
            DataProvider.Instance().UpdateItem(itemId, moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb);
        }

        public ArrayList GetItems(int moduleId)
        {
            return DotNetNuke.Common.Utilities.CBO.FillCollection(DataProvider.Instance().GetItems(moduleId), typeof(ItemInfo));
        }

        public ItemInfo GetItemById(int itemId)
        {
            return (ItemInfo)DotNetNuke.Common.Utilities.CBO.FillObject(DataProvider.Instance().GetItemById(itemId), typeof(ItemInfo));
        }

        public void DeleteItem(int itemId)
        {
            DataProvider.Instance().DeleteItem(itemId);
        }






        private string GetActivationCode(string appPath)
        {
            string host = HttpContext.Current.Request.Url.Host;

            // remove www.
            if (host.IndexOf("www.") != -1) host = host.Substring(host.IndexOf("www.") + 4);
            if (host.IndexOf("dev.") != -1) host = host.Substring(host.IndexOf("dev.") + 4);
            if (host.IndexOf("staging.") != -1) host = host.Substring(host.IndexOf("staging.") + 8);

            List<ActivationInfo> activations = DotNetNuke.Common.Utilities.CBO.FillCollection<ActivationInfo>(
                DataProvider.Instance().GetActivations("FSHOT", host)
                );

            // validate all activations
            foreach (ActivationInfo actInfo in activations) {

                RegistrationCode regCode = new RegistrationCode(actInfo.RegistrationCode);

                // check valid registration code
                if (!regCode.IsValid()) { continue; }

                // check valid activation
                if (regCode.Activate(host, actInfo.IsPrimary) != actInfo.ActivationCode) { continue; }

                // this code appears to be valid for this domain
                // validate on server
                if (null != regCode.IsActivationValid(actInfo.ActivationCode, actInfo.IsPrimary, false, appPath, actInfo.BaseActivationCode)) {
                    return actInfo.ActivationCode;
                }
            }


            // we don't have an activation for this domain
            // check registration types, and act based on this

            List<ActivationInfo> allActivations = DotNetNuke.Common.Utilities.CBO.FillCollection<ActivationInfo>(
                DataProvider.Instance().GetAllActivations("FSHOT")
                );

            foreach (ActivationInfo actInfo in allActivations) {
                if (!actInfo.IsPrimary) { continue; }

                RegistrationCode regCode = new RegistrationCode(actInfo.RegistrationCode);

                // check valid registration codee
                if (!regCode.IsValid()) { continue; }

                switch (regCode.VariantCode) {
                    case "DOM":
                        break; // domain licenses do not propagate
                    case "XDOM":

                        // check if current host is a subdomain
                        if (host.Length > actInfo.Host.Length &&
                            host[host.IndexOf(actInfo.Host) - 1] == '.') {
                            // validate on server and save activation
                            string newXdomActCode = regCode.IsActivationValid(actInfo.ActivationCode, false, true, appPath, actInfo.ActivationCode);
                            if (null != newXdomActCode) {
                                return newXdomActCode;
                            }
                        }
                        break;

                    case "PRTL":
                        // are we on the same portal as this primary domain?

                        PortalAliasController paCtrl = new PortalAliasController();
                        int mainPortalId = -1;
                        if (paCtrl.GetPortalAliases()[actInfo.Host] != null) {
                            mainPortalId = paCtrl.GetPortalAliases()[actInfo.Host].PortalID;
                        }

                        if (mainPortalId == -1) { // TODO: Maybe something is wrong here
                            //Response.Write("Main Portal not Found");
                            break;
                        }

                        // now, check we're on the same portal
                        if (mainPortalId == PortalController.GetCurrentPortalSettings().PortalId) {
                            // check on server and save in db
                            string newPrtlActCode = regCode.IsActivationValid(actInfo.ActivationCode, false, true, appPath, actInfo.ActivationCode);
                            if (null != newPrtlActCode) {
                                return newPrtlActCode;
                            }
                        }

                        break;

                    case "SRV":
                        // are we on the same server as the primary domain?
                        // ofcourse we are, or at least we can't tell from the client side
                        // check on server and save in db
                        string newSrvActCode = regCode.IsActivationValid(actInfo.ActivationCode, false, true, appPath, actInfo.ActivationCode);
                        if (null != newSrvActCode) {
                            return newSrvActCode;
                        }
                        break;

                }
            }

            // no more things to check, we definetly don't have a license
            return null;
        }

        public bool IsActivated(string appPath)
        {
            string host = HttpContext.Current.Request.Url.Host;

            // remove www.
            if (host.IndexOf("www.") != -1) host = host.Substring(host.IndexOf("www.") + 4);
            if (host.IndexOf("dev.") != -1) host = host.Substring(host.IndexOf("dev.") + 4);
            if (host.IndexOf("staging.") != -1) host = host.Substring(host.IndexOf("staging.") + 8);

            if (host == "localhost") {
                return true; // we're FULL on localhost
            }

            // initialize caching
            if (HttpContext.Current.Application["FastShotActivation"] == null) {
                HttpContext.Current.Application["FastShotActivation"] = new Hashtable();
                HttpContext.Current.Application["FastShotActivationToken"] = new Hashtable();
            }

            // check if activation exists in cache
            if (((Hashtable)HttpContext.Current.Application["FastShotActivation"])[host] != null) {
                // also, we need to validate this
                if (((Hashtable)HttpContext.Current.Application["FastShotActivationToken"])[host].ToString() == EncryptCode(host + "FSTSX", ((Hashtable)HttpContext.Current.Application["FastShotActivation"])[host].ToString())) {
                    //HttpContext.Current.Response.Write("cached: " + ((Hashtable)HttpContext.Current.Application["NavXpActivation"])[host]);
                    return true;
                }
            }

            // we don't have it cached
            string actCode = GetActivationCode(appPath);
            if (actCode == null) {
                return false;
            }

            // let's cache this code
            ((Hashtable)HttpContext.Current.Application["FastShotActivation"])[host] = actCode;
            ((Hashtable)HttpContext.Current.Application["FastShotActivationToken"])[host] = EncryptCode(host + "FSTSX", ((Hashtable)HttpContext.Current.Application["FastShotActivation"])[host].ToString());

            return true;
        }


        public static string GetInstallationKey()
        {
            string installationKey = "";

            // get application key
            //installationKey += ((Guid)SqlHelper.ExecuteScalar(DotNetNuke.Common.Utilities.Config.GetConnectionString(), CommandType.Text, "SELECT ApplicationId FROM aspnet_Applications WHERE ApplicationName = 'DotNetNuke'")).ToString() + ":";
            installationKey += DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().HostSettings["GUID"].ToString() + ":";

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/Portals"));
            installationKey += dirInfo.CreationTime.Ticks.ToString();

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] hashBytes = sha1.ComputeHash(enc.GetBytes(installationKey));
            string hash = "";
            for (int i = 0; i < hashBytes.Length; ++i) {
                hash += hashBytes[i].ToString("X2");
            }

            return hash;
        }

        private string EncryptCode(string key, string code)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] hashBytes = sha1.ComputeHash(enc.GetBytes(key + code));
            string hash = "";
            for (int i = 0; i < hashBytes.Length / 2; ++i) {
                hash += hashBytes[i].ToString("X2");
            }
            return hash;
        }

    }
}