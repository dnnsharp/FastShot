
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

    public class ItemInfo
    {
        int _ItemId;
        int _ModuleId;
        string _ItemTitle;
        string _ItemDescription;
        string _ThumbUrl;
        string _ImageUrl;
        int _ViewOrder;

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
    }
    

    public class FastShotController
    {
        //static public string RegSrv = "http://products.avatar-soft.ro/RegCoreApi.aspx";
        static public string RegSrv = "http://devx.avt.2am.ro:8080/RegCoreApi.aspx";
        static public string BuyLink = "http://www.snowcovered.com/Snowcovered2/Default.aspx?tabid=242&PackageID=13310&r=bf0821d1ea";
        static public string FastShotVersion = "1.1";
        static public string FastShotVersionAll = "1.1.0";

        public int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder)
        {
            return DataProvider.Instance().AddItem(moduleId, title, description, thumbUrl, imageUrl, viewOrder);
        }

        public void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder)
        {
            DataProvider.Instance().UpdateItem(itemId, moduleId, title, description, thumbUrl, imageUrl, viewOrder);
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





        public bool IsActivated(string activationsFile)
        {
            if (HttpContext.Current.Request.Url.Host == "localhost") {
                return true; // we're FULL on localhost
            }

            if (HttpContext.Current.Application["FastShotActivation"] == null) {
                HttpContext.Current.Application["FastShotActivation"] = new Hashtable();
                HttpContext.Current.Application["FastShotActivationToken"] = new Hashtable();
            }

            PortalSettings portalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
            int portalId = portalSettings.PortalId;
            //HttpContext.Current.Response.Write("here2");
            if (((Hashtable)HttpContext.Current.Application["FastShotActivation"])[HttpContext.Current.Request.Url.Host] != null) {
                // validate if the current key stands
                return ValidateActivation();
            }

            if (!System.IO.File.Exists(activationsFile)) {
                return false;
            }

            //HttpContext.Current.Response.Write("here1");
            // let's load the keys from the disk and see if we have a valid one
            string[] activations = System.IO.File.ReadAllLines(activationsFile);
            //HttpContext.Current.Response.Write(activations.Length.ToString() + " activations <br />");

            for (int i = 0; i < activations.Length; i++) {
                if (activations[i].Contains("FSTACT-")) {
                    //HttpContext.Current.Response.Write("here3");
                    if (IsGoodActivation(activations[i])) {
                        ((Hashtable)HttpContext.Current.Application["FastShotActivation"])[HttpContext.Current.Request.Url.Host] = activations[i];
                        ((Hashtable)HttpContext.Current.Application["FastShotActivationToken"])[HttpContext.Current.Request.Url.Host] = EncryptCode("NXPUYFSX", activations[i]);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsGoodActivation(string activationCode)
        {
            //HttpContext.Current.Response.Write("here3");
            string checkCode = GenerateActivationKey(Convert.ToInt32(activationCode.Substring(7, 2), 16), DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId, GetInstallationKey(), HttpContext.Current.Request.Url.Host);
            if (activationCode.Substring(0, activationCode.IndexOf(':')) != checkCode) { return false; }

            // let's check it's type
            //int actType = Convert.ToInt32(activationCode.Substring(7, 2), 16);
            ////HttpContext.Current.Response.Write(actType.ToString());
            //if (actType == 0) { // we also need to validate the portal
            //    int portalId = Convert.ToInt32(activationCode.Substring(9, 4), 16) - 391;

            //    if (portalId != DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId) {
            //        return false;
            //    }
            //    //HttpContext.Current.Response.Write("portalId" + portalId + activationCode);
            //}

            // now, let's also validate on the server

            string postData = "activation_code=" + activationCode.Substring(0, activationCode.IndexOf(':'));
            postData += "&reg_code=" + activationCode.Substring(activationCode.IndexOf(':') + 1);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(RegSrv + "?cmd=validate_activation");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = data.Length;
            httpRequest.Timeout = 12 * 1000;
            System.IO.Stream newStream = httpRequest.GetRequestStream();

            // Send the data.
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            string responseText = reader.ReadToEnd();
            response.Close();

            //HttpContext.Current.Response.Write("RESPONSE:" + responseText);

            return responseText.Contains("Success");
        }

        private bool ValidateActivation()
        {
            if (HttpContext.Current.Application["FastShotActivation"] == null || HttpContext.Current.Application["FastShotActivationToken"] == null) {
                return false;
            }

            // let's check it's type
            int portalId = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId;
            string activationCode = ((Hashtable)HttpContext.Current.Application["FastShotActivation"])[HttpContext.Current.Request.Url.Host].ToString();
            //HttpContext.Current.Response.Write(activationCode.Substring(7, 2));
            //int actType = Convert.ToInt32(activationCode.Substring(7, 2), 16);
            //if (actType == 0) { // we also need to validate the portal
            //    int actPortalId = Convert.ToInt32(activationCode.Substring(9, 4), 16) - 391;

            //    if (portalId != actPortalId) {
            //        return false;
            //    }
            //}

            return ((Hashtable)HttpContext.Current.Application["FastShotActivationToken"])[HttpContext.Current.Request.Url.Host].ToString() == EncryptCode("NXPUYFSX", activationCode);
        }

        public static string GetInstallationKey()
        {
            string installationKey = "FSHOT:MACHINE_KEY:633647961863437500:";

            // get application key
            installationKey += ((Guid)SqlHelper.ExecuteScalar(DotNetNuke.Common.Utilities.Config.GetConnectionString(), CommandType.Text, "SELECT ApplicationId FROM aspnet_Applications WHERE ApplicationName = 'DotNetNuke'")).ToString() + ":";
            installationKey += DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().HostSettings["GUID"].ToString() + ":";

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/Portals"));
            installationKey += dirInfo.CreationTime.Ticks.ToString();

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] hashBytes = sha1.ComputeHash(enc.GetBytes(installationKey));
            string hash = "";
            for (int i = 0; i < hashBytes.Length / 2; ++i) {
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

        private string GenerateActivationKey(int installationType, int portalId, string installationKey, string hostname)
        {
            string activationKey = "FSTACT-";
            activationKey += installationType.ToString("X2");
            //if (installationType == 0) {
            //    activationKey += (portalId + 391).ToString("X4");
            //} else {
            //    activationKey += (7431).ToString("X4");
            //}

            // let's build security token
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            //string token = activationKey + installationKey + "SECURITY_TXG";
            string token = activationKey + "SECURITY_TXG" + hostname;
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] hashBytes = sha1.ComputeHash(enc.GetBytes(token));
            string hash = "";
            for (int i = 0; i < hashBytes.Length / 2; ++i) {
                hash += hashBytes[i].ToString("X2");
            }
            activationKey += hash; // hash check - 20 chars

            return activationKey;
        }

    }
}