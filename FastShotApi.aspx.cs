using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;

namespace avt.FastShot
{
    public partial class FastShotApi : System.Web.UI.Page // DotNetNuke.Framework.CDefault
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            try {
                AvtApi api = new AvtApi();
                api.RegisterFn("list_items", ListItems);
                api.RegisterFn("add_item", AddItem);
                api.RegisterFn("edit_item", UpdateItem);
                api.RegisterFn("del_items", DelItems);
                api.RegisterFn("order_items", OrderItems);

                api.RegisterFn("update_settings", UpdateSettings);

                api.RegisterFn("clear_cache", ClearCache);
                api.RegisterFn("get_folders", ListPortalFolders);
                api.RegisterFn("get_files", ListPortalImages);
                api.RegisterFn("del_file", DeletePortalImage);

                api.Execute();
            } catch (Exception ex) { // everything should already be handled by the api
                //Response.Write("Internal Error: " + ex.Message);
                return; 
            }
        }

        // Handlers
        // ------------------------------------------------------------------------------


        void ListItems(AvtApi api, IResponseBuilder rb)
        {
            FastShotController fsCtrl = new FastShotController();
            List<ItemInfo> items = fsCtrl.GetItems(api.Module.ModuleID);
            
            rb.BeginObject("");
            int viewOrder = 0;
            foreach (ItemInfo item in items) {
                rb.WriteObject(item.ItemId.ToString(), 
                    "id", item.ItemId, false, 
                    "title", item.Title, true,
                    "desc", item.Description, true,
                    "thumb", ResolveUrl(item.ThumbnailUrl), true,
                    "thumb_w", item.ThumbWidth, false,
                    "thumb_h", item.ThumbHeight, false,
                    "image", ResolveUrl(item.ImageUrl), true,
                    "image_w", item.ImageWidth, false,
                    "image_h", item.ImageHeight, false,
                    "order", viewOrder++, false,
                    "tplParams", item.TplParams, true
                );
            }
            rb.EndObject("");
        }

        void AddItem(AvtApi api, IResponseBuilder rb)
        {
            string title = api.RequiredParameter<string>("title");
            string desc = api.OptionalParameter<string>("desc", "");
            string imageUrl = api.RequiredParameter<string>("image");
            string thumbUrl = api.OptionalParameter<string>("thumb", "");

            // translate url to portal
            if (imageUrl.IndexOf("portal://") == 0) {
                PortalController portalCtrl = new PortalController();
                PortalInfo portal = portalCtrl.GetPortal(api.Module.PortalID);
                imageUrl = "~/" +  portal.HomeDirectory + "/" + imageUrl.Substring("portal://".Length);
            }

            if (thumbUrl.IndexOf("portal://") == 0) {
                PortalController portalCtrl = new PortalController();
                PortalInfo portal = portalCtrl.GetPortal(api.Module.PortalID);
                thumbUrl = "~/" + portal.HomeDirectory + "/" + thumbUrl.Substring("portal://".Length);
            }

            FastShotController fsCtrl = new FastShotController();
            int id = fsCtrl.AddItem(api.Module.ModuleID, title, desc, thumbUrl, imageUrl, -1, string.IsNullOrEmpty(thumbUrl), api.OptionalParameter<string>("tplParams", ""));
            ItemInfo item = fsCtrl.GetItemById(id);
            rb.WriteObject(item.ItemId.ToString(),
                    "id", item.ItemId, false,
                    "title", item.Title, true,
                    "desc", item.Description, true,
                    "thumb", ResolveUrl(item.ThumbnailUrl), true,
                    "thumb_w", item.ThumbWidth, false,
                    "thumb_h", item.ThumbHeight, false,
                    "image", ResolveUrl(item.ImageUrl), true,
                    "image_w", item.ImageWidth, false,
                    "image_h", item.ImageHeight, false,
                    "order", item.ViewOrder, false,
                    "tplParams", item.TplParams, true
                );
        }

        void UpdateItem(AvtApi api, IResponseBuilder rb)
        {
            int id = api.RequiredParameter<int>("id");
            string title = api.RequiredParameter<string>("title");
            string desc = api.OptionalParameter<string>("desc", "");

            FastShotController fsCtrl = new FastShotController();
            ItemInfo item = fsCtrl.GetItemById(id);
            fsCtrl.UpdateItem(id, api.Module.ModuleID, title, desc, item.AutoGenerateThumb ? "" : item.ThumbnailUrl, item.ImageUrl, item.ViewOrder, item.AutoGenerateThumb, item.ImageWidth, item.ImageHeight, item.ThumbWidth, item.ThumbHeight, item.FileTime, api.OptionalParameter<string>("tplParams", ""));

            rb.WriteObject("response", "success", "true", false);
        }

        void DelItems(AvtApi api, IResponseBuilder rb)
        {
            FastShotController fsCtrl = new FastShotController();
            foreach (string strId in api.RequiredParameter<string>("ids").Split(',')) {
                fsCtrl.DeleteItem(Convert.ToInt32(strId));
            }
            rb.WriteObject("response", "success", "true", false);
        }

        void OrderItems(AvtApi api, IResponseBuilder rb)
        {
            FastShotController fsCtrl = new FastShotController();
            int viewOrder = 0;
            foreach (string strId in api.RequiredParameter<string>("ids").Split(',')) {
                fsCtrl.UpdateItemOrder(Convert.ToInt32(strId), viewOrder++);
            }
            rb.WriteObject("response", "success", "true", false);
        }


        void UpdateSettings(AvtApi api, IResponseBuilder rb)
        {
            string template = api.RequiredParameter<string>("template");
            int thumb_w = api.OptionalParameter<int>("thumb_w", 0);
            int thumb_h = api.OptionalParameter<int>("thumb_h", 0);

            FastShotSettings fsSettings = new FastShotSettings();
            fsSettings.Load(api.Module.ModuleID);

            ModuleController modCtrl = new ModuleController();
            modCtrl.UpdateModuleSetting(api.Module.ModuleID, "template", template);
            modCtrl.UpdateModuleSetting(api.Module.ModuleID, "thumb_width", thumb_w.ToString());
            modCtrl.UpdateModuleSetting(api.Module.ModuleID, "thumb_height", thumb_h.ToString());

            //Response.Write(fsSettings.ThumbHeight);
            //Response.Write(thumb_h);

            //Response.Write(fsSettings.ThumbWidth);
            //Response.Write(thumb_w);

            if (fsSettings.ThumbHeight != thumb_h || fsSettings.ThumbWidth != thumb_w) {
                // reset all thumbnails
                FastShotController fsCtrl = new FastShotController();
                //fsCtrl.ClearCache();
                List<ItemInfo> items = fsCtrl.GetItems(api.Module.ModuleID);
                foreach (ItemInfo item in items) {
                    if (item.AutoGenerateThumb) {
                        fsCtrl.UpdateItem(item.ItemId, item.ModuleId, item.Title, item.Description, "-", item.ImageUrl, item.ViewOrder, item.AutoGenerateThumb, item.ImageWidth, item.ImageHeight, -1, -1, item.FileTime, item.TplParams);
                    }
                }
            }
            
            

            rb.WriteObject("response", "success", "true", false);
        }


        void ListPortalFolders(AvtApi api, IResponseBuilder rb)
        {
            string parentFolder = api.RequiredParameter<string>("pdir");
            if (parentFolder.Length > 0 && parentFolder[parentFolder.Length - 1] != '\\')
                parentFolder += '\\';

            DirectoryInfo dirInfo = new DirectoryInfo(api.Portal.HomeDirectoryMapPath + parentFolder);
            rb.BeginArray("folders");
            foreach (DirectoryInfo folder in dirInfo.GetDirectories()) {
                bool hasChildren = false;
                if (folder.GetDirectories().Length > 0)
                    hasChildren = true;

                rb.WriteObject("folder", "name", folder.Name, true, "path", parentFolder + folder.Name, true, "children", hasChildren ? 1 : 0, false);
            }
            rb.EndArray("folders");
        }

        void ListPortalImages(AvtApi api, IResponseBuilder rb)
        {
            string parentFolder = api.RequiredParameter<string>("pdir");
            if (parentFolder.Length > 0 && parentFolder[parentFolder.Length - 1] != '\\')
                parentFolder += '\\';

            DirectoryInfo dirInfo = new DirectoryInfo(api.Portal.HomeDirectoryMapPath + parentFolder);
            rb.BeginArray("files");
            Dictionary<string, bool> ext = new Dictionary<string, bool>();
            ext[".jpg"] = true; ext[".jpeg"] = true; ext[".gif"] = true; 
            ext[".png"] = true;ext[".bmp"] = true;

            PortalSettings portalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();

            foreach (FileInfo file in dirInfo.GetFiles()) {
                if (ext.ContainsKey(file.Extension.ToLower()))
                    rb.WriteObject("folder", 
                        "name", file.Name, true, 
                        "path", (parentFolder.Substring(1) + file.Name).Replace('\\', '/'), true,
                        "absoluteUrl", portalSettings.HomeDirectory + (parentFolder.Substring(1) + Server.UrlEncode(file.Name)).Replace('\\', '/'),true
                    );
            }
            rb.EndArray("files");
        }

        void DeletePortalImage(AvtApi api, IResponseBuilder rb)
        {
            string path = api.RequiredParameter<string>("path");
            if (path[0] == '~')
                path = path.Substring(1);
            path = Server.MapPath("~") + path;
            File.Delete(path);
            rb.WriteObject("response", "success", "true", false);
        }

        void ClearCache(AvtApi api, IResponseBuilder rb)
        {
            FastShotController fsCtrl = new FastShotController();
            fsCtrl.ClearCache();
            rb.WriteObject("response", "success", "true", false);
        }


        // Api
        // ------------------------------------------------------------------------------

        class AvtApi
        {
            public delegate void HandlerProc(AvtApi api, IResponseBuilder rb);

            ModuleInfo _mod;
            PortalInfo _portal;
            private HttpContext _httpContext;
            private Dictionary<string, HandlerProc> _procs;
            private IResponseBuilder _rb;

            public AvtApi()
            {
                _httpContext = HttpContext.Current;
                _httpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                _httpContext.Response.Cache.SetValidUntilExpires(false);
                _httpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                _httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                _httpContext.Response.Cache.SetNoStore();

                if (!string.IsNullOrEmpty(_httpContext.Request.Params["format"])) {
                    switch (_httpContext.Request.Params["format"]) {
                        case "json":
                            _rb = new JsonResponse();
                            break;
                        case "xml":
                            _rb = new XmlResponse();
                            break;
                        default:
                            _rb = new JsonResponse();
                            break;
                    }
                } else {
                    _rb = new JsonResponse();
                }

                // TODO: check API access
                if (!DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().IsInRole(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().AdministratorRoleName)) {
                    _httpContext.Response.Write(_rb.Error("Access denied!", false));
                    throw new Exception();
                }

                // load module
                try {
                    ModuleController modCtrl = new ModuleController();
                    int mid = Convert.ToInt32(_httpContext.Request.Params["mid"]);

                    ArrayList mods = modCtrl.GetModuleTabs(mid);
                    if (mods == null || mods.Count == 0) {
                        throw new Exception();
                    }

                    _mod = (ModuleInfo)mods[0];
                    if (_mod == null || _mod.ModuleID <= 0) {
                        throw new Exception();
                    }
                    PortalController portalCtrl = new PortalController();
                    _portal = portalCtrl.GetPortal(_mod.PortalID);
                } catch (Exception ex) {
                    _httpContext.Response.Write(_rb.Error("Invalid module!", false));
                    throw new Exception();
                }

                // iniliaze handlers
                _procs = new Dictionary<string, HandlerProc>();
            }

            public ModuleInfo Module { get { return _mod; } }
            public PortalInfo Portal { get { return _portal; } }

            public void RegisterFn(string fn, HandlerProc handler)
            {
                _procs[fn] = handler;
            }

            public void Execute()
            {
                if (string.IsNullOrEmpty(_httpContext.Request.Params["fn"]) || !_procs.ContainsKey(_httpContext.Request.Params["fn"])) {
                    _httpContext.Response.Write(_rb.Error("Invalid Call!", false));
                    throw new Exception();
                }

                try {
                    _procs[_httpContext.Request.Params["fn"]](this, _rb);
                } catch (Exception ex) {
                    _httpContext.Response.Write(_rb.Error("Handler Error: " + ex.Message, true));
                    throw new Exception();
                }

                _httpContext.Response.Write(_rb.ToString());
            }



            public T RequiredParameter<T>(string paramName)
            {
                if (_httpContext.Request.Params[paramName] == null) {
                    throw new Exception("Parameter " + paramName + " does not exist in current context."); ;
                }

                try {
                    if (typeof(T) == typeof(int)) {
                        return (T)(object)Convert.ToInt32(_httpContext.Request.Params[paramName]);
                    } else if (typeof(T) == typeof(string)) {
                        return (T)(object)_httpContext.Request.Params[paramName].ToString();
                    } else if (typeof(T) == typeof(Guid)) {
                        return (T)(object)new Guid(_httpContext.Request.Params[paramName]);
                    } else if (typeof(T) == typeof(bool)) {
                        return (T)(object)Convert.ToBoolean(_httpContext.Request.Params[paramName]);
                    } else if (typeof(T) == typeof(DateTime)) {
                        return (T)(object)DateTime.Parse(_httpContext.Request.Params[paramName]);
                    }

                    // no conversion defined
                    return default(T);

                } catch (Exception) {
                    throw new Exception("Parameter " + paramName + " has invalid format.");
                }
            }

            public T OptionalParameter<T>(string paramName, T defaulValue)
            {
                if (_httpContext.Request.Params[paramName] == null) {
                    return defaulValue;
                }

                return RequiredParameter<T>(paramName);
            }

            public T OptionalParameter<T>(string paramName, string aliasParam, T defaulValue)
            {
                if (_httpContext.Request.Params[paramName] == null && _httpContext.Request.Params[aliasParam] == null) {
                    return defaulValue;
                }

                if (_httpContext.Request.Params[paramName] != null)
                    return RequiredParameter<T>(paramName);

                return RequiredParameter<T>(aliasParam);
            }
        }



        interface IResponseBuilder
        {
            string Error(string message, bool escape);

            void BeginArray(string collectionName);
            void EndArray(string collectionName);

            void BeginObject(string objName);
            void EndObject(string objName);

            void WriteObject(string name, params object[] keysAndValuesAndIfEncode);

            string ToString();
        }


        class JsonResponse : IResponseBuilder
        {
            StringBuilder _sb;

            bool _isRoot = true;
            bool _inArray = false;
            bool _inObject = false;

            public JsonResponse()
            {
                _sb = new StringBuilder();
            }

            public string Error(string message, bool escape)
            {
                return "{error: '" + (escape ? JsonEncode(message) : message) + "'}";
            }

            public void BeginArray(string collectionName)
            {
                _sb.Append('[');
                _inArray = true;
                _isRoot = false;
            }

            public void EndArray(string collectionName)
            {
                if (_sb.Length > 0 && _sb[_sb.Length - 1] == ',') { // remove last comma
                    _sb.Remove(_sb.Length - 1, 1);
                }
                _sb.Append(']');
                _inArray = false;
            }

            public void BeginObject(string objName)
            {
                _sb.Append('{');
                _inObject = true;
                _isRoot = false;
            }

            public void EndObject(string objName)
            {
                if (_sb.Length > 0 && _sb[_sb.Length - 1] == ',') { // remove last comma
                    _sb.Remove(_sb.Length - 1, 1);
                }
                _sb.Append('}');
                _inObject = false;
            }


            public void WriteObject(string name, params object[] keysAndValues)
            {
                if (keysAndValues.Length % 3 != 0) {
                    throw new Exception("Invalid number of parameters passed in WriteObject");
                }

                if (_inObject)
                    _sb.Append(name + ":{");
                else
                    _sb.Append('{');

                for (int i = 0; i < keysAndValues.Length; i += 3) {
                    _sb.Append(keysAndValues[i].ToString() + ":");
                    if (keysAndValues[i + 1].GetType() == typeof(int) || keysAndValues[i + 1].GetType() == typeof(double) || keysAndValues[i + 1].GetType() == typeof(bool)) {
                        _sb.Append(keysAndValues[i + 1].ToString() + ",");
                    } else {
                        if ((bool)keysAndValues[i + 2]) {
                            _sb.Append("\"" + JsonEncode(keysAndValues[i + 1].ToString()) + "\",");
                        } else {
                            _sb.Append("\"" + keysAndValues[i + 1].ToString() + "\",");
                        }
                    }
                }
                if (_sb.Length > 0 && _sb[_sb.Length - 1] == ',') { // remove last comma
                    _sb.Remove(_sb.Length - 1, 1);
                }
                _sb.Append("},");
            }

            public override string ToString()
            {
                if (_sb.Length > 0 && _sb[_sb.Length - 1] == ',') { // remove last comma
                    _sb.Remove(_sb.Length - 1, 1);
                }
                return _sb.ToString();
            }

            public static string JsonEncode(string s)
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
        }

        class XmlResponse : IResponseBuilder
        {
            StringBuilder _sb;

            public XmlResponse()
            {
                HttpContext.Current.Response.ContentType = "text/xml";
                _sb = new StringBuilder();
            }

            public string Error(string message, bool escape)
            {
                return "<response><error>" + message + "</error></response>";
            }

            public void BeginArray(string collectionName)
            {
                _sb.Append("<"+ collectionName +">");
            }

            public void EndArray(string collectionName)
            {
                _sb.Append("</" + collectionName + ">");
            }

            public void BeginObject(string objName)
            {
                //_sb.Append('{');
                //_inObject = true;
                //_isRoot = false;
            }

            public void EndObject(string objName)
            {
                //if (_sb.Length > 0 && _sb[_sb.Length - 1] == ',') { // remove last comma
                //    _sb.Remove(_sb.Length - 1, 1);
                //}
                //_sb.Append('}');
                //_inObject = false;
            }

            public void WriteObject(string name, params object[] keysAndValues)
            {
                if (keysAndValues.Length % 3 != 0) {
                    throw new Exception("Invalid number of parameters passed in WriteObject");
                }

                _sb.Append("<" + name + ">");
                for (int i = 0; i < keysAndValues.Length; i += 3) {
                    _sb.Append("<" + keysAndValues[i] + ">" + keysAndValues[i+1] + "</" + keysAndValues[i] + ">");
                }
                _sb.Append("</" + name + ">");
            }

            public override string ToString()
            {
                return _sb.ToString();
            }
        }

    }
}