
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

        int _ThumbWidth;
        int _ThumbHeight;
        int _ImageWidth;
        int _ImageHeight;
        long _FileTime;

        public ItemInfo()
        {
            ItemId = -1;
        }

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
            get {
                if (AutoGenerateThumb || string.IsNullOrEmpty(_ThumbUrl)) {
                    FastShotController fsCtrl = new FastShotController();
                    _ThumbUrl = fsCtrl.GenerateThumb(this);
                } else {
                    if (ItemId > 0 && (ThumbHeight <= 0 || ThumbWidth <= 0)) {
                        FastShotController ctrl = new FastShotController();
                        try {
                            ModuleController modCtrl = new ModuleController();
                            int portalId = ((ModuleInfo) modCtrl.GetModuleTabs(_ModuleId)[0]).PortalID;
                            PortalController portalCtrl = new PortalController();
                            PortalInfo portal = portalCtrl.GetPortal(portalId);
                            string url = _ThumbUrl;
                            if (url.IndexOf("http") != 0) {
                                url = HttpContext.Current.Request.MapPath(url);
                            }
                            System.Drawing.Image thumbImg = ctrl.LoadImageFromURL(url);
                            ctrl.UpdateItem(_ItemId, _ModuleId, _ItemTitle, _ItemDescription, _ThumbUrl, _ImageUrl, _ViewOrder, _AutoGenerateThumb, _ImageWidth, _ImageHeight, thumbImg.Width, thumbImg.Height, _FileTime);
                        } catch {
                            // set to -1 to flag error?
                            ctrl.UpdateItem(_ItemId, _ModuleId, _ItemTitle, _ItemDescription, _ThumbUrl, _ImageUrl, _ViewOrder, _AutoGenerateThumb, _ImageWidth, _ImageHeight, -1, -1, _FileTime);
                        }
                    }
                }
                return _ThumbUrl;
            }
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

        public int ThumbWidth
        {
            get { return _ThumbWidth; }
            set { _ThumbWidth = value; }
        }

        public int ThumbHeight
        {
            get { return _ThumbHeight; }
            set { _ThumbHeight = value; }
        }

        public int ImageWidth
        {
            get { return _ImageWidth; }
            set { _ImageWidth = value; }
        }

        public int ImageHeight
        {
            get { return _ImageHeight; }
            set { _ImageHeight = value; }
        }

        public long FileTime
        {
            get { return _FileTime; }
            set { _FileTime = value; }
        }
    }
    

    public class FastShotController
    {

        private string _objectQualifier;
        private string _databaseOwner;

        static public string RegSrv = "http://www.avatar-soft.ro/DesktopModules/avt.RegCore4/Api.aspx";

        static public string ProductCode = "FSHOT";
        static public string Version = "1.4";
        static public string VersionAll = "1.4.0";

        static public string DocSrv = RegSrv + "?cmd=doc&product=FastShot&version=" + Version;
        static public string BuyLink = RegSrv + "?cmd=buy&product=FastShot&version=" + Version;

        static public string ProductKey = "<RSAKeyValue><Modulus>zkBeCaywWL1J38zEmy7+0gysKNb0EwikvxRThOSvHOcLKD/qF4GIwldf+7LS3sFEFlnCGjRq+bOmWfYhogre2er+NWjmWXuCxaHvnYg9DH1KJyE0sGg5hzkaRLxZe54fnHYmwt/HP/frOY7rWut97ZQMBhItX0JNaMwGlYgVxGk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";



        public FastShotController()
        {
            // Read the configuration specific information for this provider
            DotNetNuke.Framework.Providers.ProviderConfiguration _providerConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data");
            DotNetNuke.Framework.Providers.Provider objProvider = (DotNetNuke.Framework.Providers.Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            //Get Connection string from web.config
            string _connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

            if (_connectionString == "") {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            string _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false) {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false) {
                _databaseOwner += ".";
            }
        }


        public AvtActivationDataSource GetActivationSrc()
        {
            return new AvtActivationDataSourceDb(DotNetNuke.Common.Utilities.Config.GetConnectionString(), _databaseOwner, _objectQualifier, "avtFastShot_Activations");
        }

        public AvtRegCoreClient GetRegCoreClient()
        {
            return AvtRegCoreClient.Get(RegSrv, ProductCode, GetActivationSrc(), false);
        }

        public bool IsActivated()
        {
            AvtRegCoreClient regClient = AvtRegCoreClient.Get(RegSrv, ProductCode, GetActivationSrc(), false);
            return regClient.IsActivated(ProductCode, Version, HttpContext.Current.Request.Url.Host);
        }




        public int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb)
        {
            return DataProvider.Instance().AddItem(moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb);
        }

        public void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb, int imageWidth, int imageHeight, int thumbWidth, int thumbHeight, long lastWriteTime)
        {
            DataProvider.Instance().UpdateItem(itemId, moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb, imageWidth, imageHeight, thumbWidth, thumbHeight, lastWriteTime);
        }

        public void UpdateItemOrder(int itemId, int viewOrder)
        {
            DataProvider.Instance().UpdateItemOrder(itemId, viewOrder);
        }

        public List<ItemInfo> GetItems(int moduleId)
        {
            return DotNetNuke.Common.Utilities.CBO.FillCollection <ItemInfo>(DataProvider.Instance().GetItems(moduleId));
        }

        public ItemInfo GetItemById(int itemId)
        {
            return (ItemInfo)DotNetNuke.Common.Utilities.CBO.FillObject(DataProvider.Instance().GetItemById(itemId), typeof(ItemInfo));
        }

        public void DeleteItem(int itemId)
        {
            DataProvider.Instance().DeleteItem(itemId);
        }

        private string GetThumbFolder()
        {
            string fastShotRoot = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath + "\\FastShot";
            if (!System.IO.Directory.Exists(fastShotRoot)) {
                System.IO.Directory.CreateDirectory(fastShotRoot);
            }

            fastShotRoot = fastShotRoot + "\\thumbs\\";
            if (!System.IO.Directory.Exists(fastShotRoot)) {
                System.IO.Directory.CreateDirectory(fastShotRoot);
            }

            return fastShotRoot;
        }

        public void ClearCache()
        {
            HttpRuntime.Cache.Remove("FastShot.thumbs");
        }

        public System.Drawing.Image LoadImageFromURL(string URL)
        {
            const int BYTESTOREAD = 10000;
            WebRequest myRequest = WebRequest.Create(URL);
            myRequest.Timeout = 10 * 1000;
            WebResponse myResponse = myRequest.GetResponse();
            Stream ReceiveStream = myResponse.GetResponseStream();
            BinaryReader br = new BinaryReader(ReceiveStream);
            MemoryStream memstream = new MemoryStream();
            byte[] bytebuffer = new byte[BYTESTOREAD];
            int BytesRead = br.Read(bytebuffer, 0, BYTESTOREAD);
            while (BytesRead > 0) {
                memstream.Write(bytebuffer, 0, BytesRead);
                BytesRead = br.Read(bytebuffer, 0, BYTESTOREAD);
            }

            return System.Drawing.Image.FromStream(memstream);
        }

        public string GenerateThumb(ItemInfo item)
        {
            FastShotSettings fsSettings = new FastShotSettings();
            fsSettings.Load(item.ModuleId);

            // check cache
            if (HttpRuntime.Cache.Get("FastShot.thumbs") == null) {
                HttpRuntime.Cache.Insert("FastShot.thumbs", new Dictionary<string, string>());
            }

            Dictionary<string, string> thumbCache = (Dictionary<string, string>) HttpRuntime.Cache.Get("FastShot.thumbs");
            string cacheKey = item.ItemId.ToString() + "[" + fsSettings.ThumbWidth.ToString() + "," + fsSettings.ThumbHeight.ToString() + "]";
            if (!thumbCache.ContainsKey(cacheKey)) {

                // check file exists
                System.Drawing.Image image;
                if (item.ImageUrl.IndexOf("http") == 0) {
                    try {
                        image = LoadImageFromURL(item.ImageUrl);
                    } catch {
                        UpdateItem(item.ItemId, item.ModuleId, item.Title, item.Description, "", item.ImageUrl, item.ViewOrder, true, 0, 0, 0, 0, 0); 
                        return "";
                    }
                } else {
                    try {
                        image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(item.ImageUrl));
                    } catch { 
                        UpdateItem(item.ItemId, item.ModuleId, item.Title, item.Description, "", item.ImageUrl, item.ViewOrder, true, 0, 0, fsSettings.ThumbWidth, fsSettings.ThumbHeight, 0); 
                        return ""; 
                    }
                }


                // check if exists on disc first
                string thumbFolder = GetThumbFolder();
                string thumbName = Path.GetFileNameWithoutExtension(item.ImageUrl) + "-[" + fsSettings.ThumbWidth.ToString() + "," + fsSettings.ThumbHeight.ToString() + "]" + Path.GetExtension(item.ImageUrl);
                //if (!File.Exists(thumbFolder + thumbName) || File.GetLastWriteTime(thumbFolder + thumbName).ToFileTime() > item.FileTime) {
                    // we need to create the thumb
                    System.Drawing.Size sz = _GenerateThumb(image, thumbFolder + thumbName, fsSettings.ThumbWidth, fsSettings.ThumbHeight);

                    //if (File.GetLastWriteTime(thumbFolder + thumbName).ToFileTime() > item.FileTime || item.ImageWidth == 0 || item.ImageHeight == 0 || item.ThumbWidth == 0 || item.ThumbHeight == 0) {
                        item.ImageWidth = image.Width;
                        item.ImageHeight = image.Height;
                        item.ThumbWidth = sz.Width;
                        item.ThumbHeight = sz.Height;
                        UpdateItem(item.ItemId, item.ModuleId, item.Title, item.Description, "", item.ImageUrl, item.ViewOrder, true, item.ImageWidth, item.ImageHeight, item.ThumbWidth, item.ThumbHeight, File.GetLastWriteTime(thumbFolder + thumbName).ToFileTime());
                    //}
                //}

                image.Dispose();
                thumbCache[cacheKey] = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().HomeDirectory + "FastShot/thumbs/" + thumbName;
            }

            return thumbCache[cacheKey];
        }

        private System.Drawing.Size _GenerateThumb(System.Drawing.Image image, string thumbUrl, int width, int height)
        {
            if (height <= 0 && width <= 0) {
                height = 64;
                width = 64;
            } else if (height <= 0) {
                height = width * image.Height / image.Width;
            } else if (width <= 0) {
                width = height * image.Width / image.Height;
            }

            System.Drawing.Bitmap thumbnailImage = new System.Drawing.Bitmap(image, width, height);
            thumbnailImage.Save(thumbUrl);
            return new System.Drawing.Size(width, height);
        }



    }








    // ---------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------
    // ---------------------------------------------------------------------------------------


    public class AvtRegCoreClient
    {
        Dictionary<string, AvtActivation> _initActivations;
        Dictionary<string, AvtActivation> _validActivations;
        AvtActivationDataSource _src;
        string _regCoreSrv;

        private AvtRegCoreClient(string regCoreSrv, AvtActivationDataSource src)
        {
            // fill activations
            _initActivations = src.GetActivations();
            _validActivations = new Dictionary<string, AvtActivation>();
            _src = src;
            _regCoreSrv = regCoreSrv;
        }

        static public AvtRegCoreClient Get(string regCoreSrv, string productCode, AvtActivationDataSource src, bool clearCache)
        {
            AvtRegCoreClient regCoreClient;
            if (clearCache == false && HttpRuntime.Cache["avt.RegCoreClient." + productCode] != null) {
                regCoreClient = (AvtRegCoreClient)HttpRuntime.Cache["avt.RegCoreClient." + productCode];
            } else {

                // clear cache, in case it's ignored
                HttpRuntime.Cache.Remove("avt.RegCoreClient." + productCode);

                // create new client
                regCoreClient = new AvtRegCoreClient(regCoreSrv, src);
                HttpRuntime.Cache.Insert("avt.RegCoreClient." + productCode, regCoreClient);
            }

            return regCoreClient;
        }

        public Dictionary<string, AvtActivation> ValidActivations
        {
            get { return _validActivations; }
        }

        public Dictionary<string, AvtActivation> InitActivations
        {
            get { return _initActivations; }
        }

        public void ClearCache(string productCode)
        {
            HttpRuntime.Cache.Remove("avt.RegCoreClient." + productCode);
        }

        public void ClearAll()
        {
            _src.RemoveAll();

            _initActivations = new Dictionary<string, AvtActivation>();
            _validActivations = new Dictionary<string, AvtActivation>();
        }

        public bool IsActivated(string productCode, string version, string host)
        {
            // first, check if we have it in cache
            if (_validActivations.ContainsKey(host) && _validActivations[host].IsValid(productCode, version) && _validActivations[host].TmpKey == GetHash(host + _validActivations[host].ActivationCode)) {
                return true;
            }

            string checkHost = host;

            // let's check the rest of activations
            foreach (AvtActivation act in _initActivations.Values) {

                if (!act.IsValid(productCode, version))
                    continue;

                // remove www
                if (host.IndexOf("www.") == 0 || host.IndexOf("http://www.") == 0 || host.IndexOf("https://www.") == 0) host = host.Substring(host.IndexOf("www.") + 4);

                // for domain license, also remove dev. and staging. (we grant these free)
                if (act.RegCode.VariantCode == "DOM") {
                    if (host.IndexOf("dev.") == 0 || host.IndexOf("http://dev.") == 0 || host.IndexOf("https://dev.") == 0) host = host.Substring(host.IndexOf("dev.") + 4);
                    if (host.IndexOf("staging.") == 0 || host.IndexOf("http://staging.") == 0 || host.IndexOf("https://staging.") == 0) host = host.Substring(host.IndexOf("staging.") + 8);
                }

                if (act.RegCode.VariantCode == "XDOM") {
                    // see if host is included in baseHost
                    if (host.IndexOf(act.Host) + act.Host.Length != host.Length) {
                        continue; // invalid
                    }
                    host = act.Host;
                }

                if (act.RegCode.VariantCode == "SRV" && Regex.Match(host, ".*\\d+\\.\\d+\\.\\d+\\.\\d+.*").Length == 0) {
                    // we need to get IP of domain
                    try {
                        bool bFound = false;
                        foreach (IPAddress addr in System.Net.Dns.GetHostEntry(host).AddressList) {
                            if (addr.ToString() == act.Host) {
                                bFound = true;
                                break;
                            }
                        }
                        if (!bFound)
                            continue; // IP not found

                        host = act.Host;

                    } catch {
                        continue;
                    }
                }

                if (act.Host == host) {
                    // put in tmp key
                    AvtActivation actCpy = act.Clone();
                    actCpy.TmpKey = GetHash(host + actCpy.ActivationCode);
                    _validActivations[checkHost] = actCpy;
                    return true;
                }
            }

            return false;
        }

        private string GetHash(string tk)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] hashBytes = sha1.ComputeHash(Encoding.Unicode.GetBytes(tk));
            string hash = "";
            for (int i = 0; i < hashBytes.Length / 2; ++i) {
                hash += hashBytes[i].ToString("X2");
            }

            return hash;
        }

        public AvtActivation Activate(string regCode, string productCode, string version, string host, string productKey)
        {
            AvtRegistrationCode r = new AvtRegistrationCode(regCode);

            // remove www
            if (host.IndexOf("www.") == 0 || host.IndexOf("http://www.") == 0 || host.IndexOf("https://www.") == 0) host = host.Substring(host.IndexOf("www.") + 4);

            // for domain license, also remove dev. and staging. (we grant these free)
            if (r.VariantCode == "DOM") {
                if (host.IndexOf("dev.") == 0 || host.IndexOf("http://dev.") == 0 || host.IndexOf("https://dev.") == 0) host = host.Substring(host.IndexOf("dev.") + 4);
                if (host.IndexOf("staging.") == 0 || host.IndexOf("http://staging.") == 0 || host.IndexOf("https://staging.") == 0) host = host.Substring(host.IndexOf("staging.") + 8);
            }

            Dictionary<string, string> data = new Dictionary<string, string>();
            Dictionary<string, string> prvData = new Dictionary<string, string>();
            data["product"] = productCode; // this is not encrypted because we need to extract the private key on the server side
            prvData["regcode"] = regCode;
            prvData["version"] = version;
            prvData["hostname"] = host;

            XmlDocument xmlAct = new XmlDocument();
            try {
                xmlAct.LoadXml(SendData(_regCoreSrv + "?cmd=activate", productKey, data, prvData));
            } catch (Exception e) {
                throw new Exception("An error occured (" + e.Message + ")");
            }

            if (xmlAct["error"] != null) {
                throw new Exception(xmlAct["error"].InnerText);
            }

            AvtActivation act = new AvtActivation();
            act.RegistrationCode = regCode;
            act.Host = xmlAct.FirstChild["host"].InnerText;
            act.ActivationCode = xmlAct.FirstChild["activation_code"].InnerText;
            act.ProductKey = xmlAct.FirstChild["product_key"].InnerText;
            act.BaseProductCode = r.ProductCode;
            act.BaseProductVersion = xmlAct.FirstChild["version"].InnerText;

            if (!act.IsValid(productCode, version)) {
                throw new Exception("Invalid activation");
            }

            // add activation
            _src.AddActivation(act);
            _initActivations[act.Host] = act;
            _validActivations[act.Host] = act;

            return act;
        }

        private string SendData(string url, string productKey, Dictionary<string, string> dataParams, Dictionary<string, string> prvDataParams)
        {
            // private params put in xml format and encrypt
            string prvData = null;
            if (prvDataParams.Count > 0) {
                prvData += "<data>";
                foreach (string paramName in prvDataParams.Keys) {
                    prvData += "<" + paramName + ">" + prvDataParams[paramName] + "</" + paramName + ">";
                }
                prvData += "</data>";

                CspParameters cspParam = new CspParameters();
                cspParam.Flags = CspProviderFlags.UseMachineKeyStore;

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParam);
                rsa.FromXmlString(productKey);

                string encrypted = "";
                int chunkSize = 40;
                for (int i = 0; i < prvData.Length / chunkSize; i++) {
                    byte[] encryptedBytes = rsa.Encrypt(Encoding.Unicode.GetBytes(prvData.Substring(i * chunkSize, chunkSize)), true);
                    // convert to hexa string
                    for (int j = 0; j < encryptedBytes.Length; ++j) {
                        encrypted += encryptedBytes[j].ToString("X2");
                    }
                }
                if (prvData.Length % chunkSize != 0) {
                    byte[] encryptedBytes = rsa.Encrypt(Encoding.Unicode.GetBytes(prvData.Substring(prvData.Length - prvData.Length % chunkSize)), true);
                    for (int j = 0; j < encryptedBytes.Length; ++j) {
                        encrypted += encryptedBytes[j].ToString("X2");
                    }
                }
                prvData = encrypted;
            }

            // fill post data (and include private data above)
            string postData = "";
            foreach (string paramName in dataParams.Keys) {
                postData += paramName + "=" + dataParams[paramName] + "&";
            }
            if (prvData != null) {
                postData += "prvdata=" + prvData;
            }
            if (postData[postData.Length - 1] == '&') postData = postData.Substring(0, postData.Length - 1);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
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

            return responseText.Trim();
        }
    }

    public class AvtActivation
    {
        public string Host;
        public string RegistrationCode;
        public string ActivationCode;
        public string ProductKey;
        public string BaseProductCode;
        public string BaseProductVersion;
        public string TmpKey = "";

        AvtRegistrationCode _RegCode = null;
        public AvtRegistrationCode RegCode
        {
            get
            {
                if (_RegCode == null) {
                    try {
                        _RegCode = new AvtRegistrationCode(RegistrationCode);
                    } catch {
                        _RegCode = null;
                    }
                }
                return _RegCode;
            }
        }

        public AvtActivation()
        {
        }

        public AvtActivation Clone()
        {
            AvtActivation act = new AvtActivation();
            act.ActivationCode = ActivationCode;
            act.RegistrationCode = RegistrationCode;
            act.Host = Host;
            act.ProductKey = ProductKey;
            act.BaseProductVersion = BaseProductVersion;
            act.BaseProductCode = BaseProductCode;
            return act;
        }

        public bool IsValid(string productCode, string versionCode)
        {
            // always pass BaseProductVerions in activation code IsValid
            if (RegCode == null || RegCode.IsExpired())
                return false;

            CspParameters cspParam = new CspParameters();
            cspParam.Flags = CspProviderFlags.UseMachineKeyStore;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParam);
            rsa.FromXmlString(ProductKey);

            byte[] signatureBytes = new byte[128];
            for (int i = 0; i < 128; i++) {
                signatureBytes[i] = Convert.ToByte(ActivationCode.Substring(i * 2, 2), 16);
            }

            return rsa.VerifyData(Encoding.Unicode.GetBytes(Host + RegistrationCode + RegCode.R + productCode), "SHA1", signatureBytes);
        }
    }



    public interface AvtActivationDataSource
    {
        Dictionary<string, AvtActivation> GetActivations();
        void AddActivation(string regCode, string host, string actCode, string productKey, string baseProductCode, string baseVersionCode);
        void AddActivation(AvtActivation act);
        void Remove(AvtActivation act);
        void RemoveAll();
    }


    public class AvtActivationDataSourceDb : AvtActivationDataSource
    {
        string _conStr;
        string _dbo;
        string _qualifier;
        string _table;

        public AvtActivationDataSourceDb(string conStr, string dbo, string qualifier, string table)
        {
            _conStr = conStr;
            _dbo = dbo;
            _qualifier = qualifier;
            _table = table;
        }

        public Dictionary<string, AvtActivation> GetActivations()
        {
            SqlConnection conn = new SqlConnection(_conStr);
            SqlDataAdapter a = new SqlDataAdapter("select * from " + _dbo + _qualifier + _table, conn);
            DataSet s = new DataSet(); a.Fill(s);

            Dictionary<string, AvtActivation> activations = new Dictionary<string, AvtActivation>();
            foreach (DataRow dr in s.Tables[0].Rows) {
                AvtActivation act = new AvtActivation();
                act.Host = dr["Host"].ToString();
                act.RegistrationCode = dr["RegistrationCode"].ToString();
                act.ActivationCode = dr["ActivationCode"].ToString();
                act.ProductKey = dr["ProductKey"].ToString();
                act.BaseProductVersion = dr["BaseProductVersion"].ToString();
                act.BaseProductCode = dr["BaseProductCode"].ToString();
                //Console.WriteLine(dr[0].ToString());
                activations[act.Host] = act;
            }

            return activations;
        }

        public void AddActivation(string regCode, string host, string actCode, string productKey, string baseProductCode, string baseVersionCode)
        {
            string sqlF = "DELETE FROM {0} WHERE RegistrationCode = '{1}' AND Host = '{2}'; INSERT INTO {0} VALUES('{1}', '{2}', '{3}', '{4}', '{5}', '{6}')";
            string sql = string.Format(sqlF, _dbo + _qualifier + _table, regCode, host.Replace("'", "''"), actCode.Replace("'", "''"), productKey.Replace("'", "''"), baseProductCode, baseVersionCode);
            SqlConnection conn = new SqlConnection(_conStr);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void AddActivation(AvtActivation act)
        {
            AddActivation(act.RegistrationCode, act.Host, act.ActivationCode, act.ProductKey, act.BaseProductCode, act.BaseProductVersion);
        }


        public void Remove(string regCode, string host)
        {
            string sqlF = "DELETE FROM {0} WHERE RegistrationCode = '{1}' AND Host = '{2}'; ";
            string sql = string.Format(sqlF, _dbo + _qualifier + _table, regCode, host.Replace("'", "''"));
            SqlConnection conn = new SqlConnection(_conStr);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void Remove(AvtActivation act)
        {
            Remove(act.RegistrationCode, act.Host);
        }

        public void RemoveAll()
        {
            string sqlF = "DELETE FROM {0}";
            string sql = string.Format(sqlF, _dbo + _qualifier + _table);
            SqlConnection conn = new SqlConnection(_conStr);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }


    public class AvtRegistrationCode
    {
        static Random rGen = new Random();

        string _RegCode;
        string _prodCode;
        string _variantCode;
        string _hashCheck;
        string _custPart;
        string _randPart;

        DateTime _dateExpire = DateTime.MinValue;

        public bool HasTimeBomb
        {
            get { return _dateExpire != DateTime.MinValue; }
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

        public string R
        {
            get { return _randPart; }
        }

        public bool IsExpired()
        {
            return HasTimeBomb && _dateExpire < DateTime.Now;
        }

        public AvtRegistrationCode(string regCode)
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

            if (parts.Length < 3)
                throw new FormatException("Invalid Registration Code Format");

            _hashCheck = parts[iPart].Substring(0, 20);
            _custPart = parts[iPart].Substring(20);
            _randPart = parts[iPart].Substring(28);

            //HttpContext.Current.Response.Write(_hashCheck+"<Br />");
            //HttpContext.Current.Response.Write(_custPart + "<Br />");
            //HttpContext.Current.Response.Write(_randPart + "<Br />");

            // validate length
            if (_randPart.Length != 6)
                throw new FormatException("Invalid Registration Code Format");
        }

        private AvtRegistrationCode() // private constructor called via Generate
        {
        }


        public override string ToString()
        {
            return _RegCode;
        }

    }

}