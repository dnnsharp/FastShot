using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Xml;

namespace avt.FastShot
{
    public interface IMain
    {
        void RenderItems();
    }


    public class FastShotController
    {
        static public string Build = "1.6.0";

        public FastShotController()
        {
           
        }

        public int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb, string tplParams)
        {
            return DataProvider.Instance().AddItem(moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb, tplParams);
        }

        public void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb, int imageWidth, int imageHeight, int thumbWidth, int thumbHeight, long lastWriteTime, string tplParams)
        {
            DataProvider.Instance().UpdateItem(itemId, moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb, imageWidth, imageHeight, thumbWidth, thumbHeight, lastWriteTime, tplParams);
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

        private string GetAuthThumbUrl(string thumbName)
        {
            return string.Format("~" + DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().HomeDirectory + "FastShot/thumbs/{0}", thumbName);
        }

        public void ClearCache()
        {
            HttpRuntime.Cache.Remove("FastShot.thumbs");
        }

        public static void Log(string txt)
        {
            // TODO
            if (!DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalName.Contains("Avatar Software - Playground")) {
                return;
            }

            string logFolder = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath;
            string logFilePath = Path.Combine(logFolder, "log.fastshot." + DateTime.Now.ToString("yyyy.MM.dd") + ".txt");

            File.AppendAllText(logFilePath, "[" + DateTime.Now.ToString() + "] " + txt + "\r\n");
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
                //HttpContext.Current.Response.Write("here");
                // check file exists
                System.Drawing.Image image;
                if (item.ImageUrl.IndexOf("http") == 0) {
                    try {
                        image = LoadImageFromURL(item.ImageUrl);
                    } catch {
                        UpdateItem(item.ItemId, item.ModuleId, item.Title, item.Description, "", item.ImageUrl, item.ViewOrder, true, 0, 0, 0, 0, 0, item.TplParams); 
                        return "";
                    }
                } else {
                    try {
                        image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(item.ImageUrl));
                    } catch {
                        UpdateItem(item.ItemId, item.ModuleId, item.Title, item.Description, "", item.ImageUrl, item.ViewOrder, true, 0, 0, fsSettings.ThumbWidth, fsSettings.ThumbHeight, 0, item.TplParams); 
                        return ""; 
                    }
                }


                // check if exists on disc first
                string thumbFolder = GetThumbFolder();
                string thumbName = Path.GetFileNameWithoutExtension(item.ImageUrl) + "-[" + fsSettings.ThumbWidth.ToString() + "," + fsSettings.ThumbHeight.ToString() + "]" + Path.GetExtension(item.ImageUrl);
                //if (!File.Exists(thumbFolder + thumbName) || File.GetLastWriteTime(thumbFolder + thumbName).ToFileTime() > item.FileTime) {
                    // we need to create the thumb
                    System.Drawing.Size sz = _GenerateThumb(image, Path.Combine(thumbFolder, thumbName), fsSettings.ThumbWidth, fsSettings.ThumbHeight);
                
                    //if (File.GetLastWriteTime(thumbFolder + thumbName).ToFileTime() > item.FileTime || item.ImageWidth == 0 || item.ImageHeight == 0 || item.ThumbWidth == 0 || item.ThumbHeight == 0) {
                        item.ImageWidth = image.Width;
                        item.ImageHeight = image.Height;
                        item.ThumbWidth = sz.Width;
                        item.ThumbHeight = sz.Height;
                        UpdateItem(item.ItemId, item.ModuleId, item.Title, item.Description, GetAuthThumbUrl(thumbName), item.ImageUrl, item.ViewOrder, true, item.ImageWidth, item.ImageHeight, item.ThumbWidth, item.ThumbHeight, File.GetLastWriteTime(thumbFolder + thumbName).ToFileTime(), item.TplParams);
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

            Bitmap thumbnailImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(thumbnailImage)) {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, width, height);
                thumbnailImage.Save(thumbUrl, ImageFormat.Png);
                thumbnailImage.Dispose();

                FastShotController.Log("Created thumbnail " + thumbUrl);
            }

            //System.Drawing.Bitmap thumbnailImage = new System.Drawing.Bitmap(image, width, height);
            //thumbnailImage.Save(thumbUrl);
            //thumbnailImage.Dispose();
            return new System.Drawing.Size(width, height);
        }



        public string Tokenize(string strContent, ModuleInfo modInfo)
        {
            bool bMyTokensInstalled = false;
            MethodInfo methodReplace = null;
            MethodInfo methodReplaceWMod = null;

            // first, determine if MyTokens is installed
            if (HttpRuntime.Cache.Get("avt.MyTokens.Installed") != null) {
                bMyTokensInstalled = Convert.ToBoolean(HttpRuntime.Cache.Get("avt.MyTokens.Installed"));
                if (bMyTokensInstalled == true) {
                    methodReplace = (MethodInfo)HttpRuntime.Cache.Get("avt.MyTokens.MethodReplace");
                    methodReplaceWMod = (MethodInfo)HttpRuntime.Cache.Get("avt.MyTokens.MethodReplaceWMod");
                }
            } else {
                // it's not in cache, let's determine if it's installed
                try {
                    Type myTokensRepl = DotNetNuke.Framework.Reflection.CreateType("avt.MyTokens.MyTokensReplacer");
                    if (myTokensRepl == null)
                        throw new Exception(); // handled in catch

                    bMyTokensInstalled = true;

                    // we now know MyTokens is installed, get ReplaceTokensAll methods

                    methodReplace = myTokensRepl.GetMethod(
                        "ReplaceTokensAll",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                         CallingConventions.Any,
                        new Type[] { 
                            typeof(string), 
                            typeof(DotNetNuke.Entities.Users.UserInfo), 
                            typeof(bool) 
                        },
                        null
                    );

                    methodReplaceWMod = myTokensRepl.GetMethod(
                        "ReplaceTokensAll",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                         CallingConventions.Any,
                        new Type[] { 
                            typeof(string), 
                            typeof(DotNetNuke.Entities.Users.UserInfo), 
                            typeof(bool),
                            typeof(ModuleInfo)
                        },
                        null
                    );

                    if (methodReplace == null || methodReplaceWMod == null) {
                        // this shouldn't really happen, we know MyTokens is installed
                        throw new Exception();
                    }

                } catch {
                    bMyTokensInstalled = false;
                }

                // cache values so next time the funciton is called the reflection logic is skipped
                HttpRuntime.Cache.Insert("avt.MyTokens.Installed", bMyTokensInstalled);
                if (bMyTokensInstalled) {
                    HttpRuntime.Cache.Insert("avt.MyTokens.MethodReplace", methodReplace);
                    HttpRuntime.Cache.Insert("avt.MyTokens.MethodReplaceWMod", methodReplaceWMod);
                }
            }


            // revert to standard DNN Token Replacement if MyTokens is not installed

            if (!bMyTokensInstalled) {
                DotNetNuke.Services.Tokens.TokenReplace dnnTknRepl = new DotNetNuke.Services.Tokens.TokenReplace();
                dnnTknRepl.AccessingUser = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
                dnnTknRepl.DebugMessages = !DotNetNuke.Common.Globals.IsTabPreview();
                if (modInfo != null)
                    dnnTknRepl.ModuleInfo = modInfo;

                // MyTokens is not installed, execution ends here
                return dnnTknRepl.ReplaceEnvironmentTokens(strContent);
            }

            // we have MyTokens installed, proceed to token replacement
            // Note that we could be using only the second overload and pass null to the ModuleInfo parameter,
            //  but this will break compatibility with integrations made before the second overload was added
            if (modInfo == null) {
                return (string)methodReplace.Invoke(
                    null,
                    new object[] {
                        strContent,
                        DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo(),
                        !DotNetNuke.Common.Globals.IsTabPreview()
                    }
                );
            } else {
                return (string)methodReplaceWMod.Invoke(
                    null,
                    new object[] {
                        strContent,
                        DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo(),
                        !DotNetNuke.Common.Globals.IsTabPreview(),
                        modInfo
                    }
                );
            }
        }



        public XmlDocument MyTokens_GetDefinition()
        {
            // since token descriptors are static,
            // let MyTokens cache it for a reasonable amount of time;
            // if namespace info would be dynamic (for example, token definitions would change
            // based on current user roles). then set it to 0 (which is the default).
            // note this only affects retrieval of token definitions, caching values 
            // returned from tokens is treaded separately in MyTokens_Replace method
            //cacheTimeSeconds = 86400; // 1 day

            string xml = @"
                <mytokens><!-- namespace is Module Name -->
                    <receiveOnlyKnownTokens>true</receiveOnlyKnownTokens>
                    <cacheTimeSeconds>86400</cacheTimeSeconds><!-- instructs MyTokens to cache this token definition for specified amount of time; set to 0 to disable caching if token definitions are dynamic (for example, changes based on roles of current user or based on time events) -->
                    <docurl>http://docs.avatar-soft.ro</docurl>
                    <token>
                        <name>Random</name><!-- case insensitive -->
                        <desc>Returns a random image from the specified module.</desc>
                        <cacheTimeSeconds>0</cacheTimeSeconds>
                        <docurl>http://doc.avatar-soft.ro</docurl>
                        <param>
                            <name>ModuleId</name><!-- case insensitive -->
                            <desc>
                                Id of the module to extract random image from.
                            </desc>
                            <type>int</type>
                            <values></values><!-- only for enums, comma separated list -->
                            <required>true</required>
                        </param>
                        <example>
                            <codeSnippet>[FastShot:Random(ModuleId=100)]</codeSnippet>
                            <desc>Returns random image from the FastShot module with id 100.</desc>
                        </example>
                    </token>

                    <token>
                        <name>ByName</name><!-- case insensitive -->
                        <desc>Returns a image identified by name from the specified module.</desc>
                        <cacheTimeSeconds>0</cacheTimeSeconds>
                        <docurl>http://doc.avatar-soft.ro</docurl>
                        <param>
                            <name>ModuleId</name><!-- case insensitive -->
                            <desc>
                                Id of the module to extract the image from.
                            </desc>
                            <type>int</type>
                            <values></values><!-- only for enums, comma separated list -->
                            <required>true</required>
                        </param>
                        <param>
                            <name>Name</name><!-- case insensitive -->
                            <desc>
                                Name of the image to retrieve.
                            </desc>
                            <type>string</type>
                            <values></values><!-- only for enums, comma separated list -->
                            <required>true</required>
                        </param>
                        <example>
                            <codeSnippet>[FastShot:ByName(ModuleId=100,Name='Sunset Image')]</codeSnippet>
                            <desc>Returns image with name <i>Sunset Image</i> from the FastShot module with id 100.</desc>
                        </example>
                    </token>
                    <token>
                        <name>Latest</name><!-- case insensitive -->
                        <desc>Returns latest added image from the specified module.</desc>
                        <cacheTimeSeconds>0</cacheTimeSeconds>
                        <docurl>http://doc.avatar-soft.ro</docurl>
                        <param>
                            <name>ModuleId</name><!-- case insensitive -->
                            <desc>
                                Id of the module to return latest image from.
                            </desc>
                            <type>int</type>
                            <values></values><!-- only for enums, comma separated list -->
                            <required>true</required>
                        </param>
                        <example>
                            <codeSnippet>[FastShot:Latest(ModuleId=100)]</codeSnippet>
                            <desc>Returns last added image from the FastShot module with id 100.</desc>
                        </example>
                    </token>
                </mytokens>
            ";

            XmlDocument xmlTknNs = new XmlDocument();
            xmlTknNs.LoadXml(xml);
            return xmlTknNs;

            //return new string[] { "Faqs", "LatestFaq" };
        }


        public string MyTokens_ReplaceToken(string tokenNamespace, string tokenName, IDictionary<string, object> tokenParams, CultureInfo formatProvider, UserInfo AccessingUser, ref bool PropertyNotFound, ref bool bRecursivelyReplaceTokens, ref int cacheTimeSeconds)
        {
            //string tmp = "";
            //foreach (string key in tokenParams.Keys) {
            //    tmp += key + ":"+ tokenParams[key] + ", ";
            //}
            //return tmp;

            List<ItemInfo> images = GetItems((int)tokenParams["ModuleId".ToLower()]);
            if (images.Count == 0) {
                return "";
            }

            ItemInfo retItem = null;
            switch (tokenName) {
                case "Random":
                    Random r = new Random();
                    retItem = images[r.Next(0, images.Count - 1)];
                    break;
                case "ByName":
                    foreach (ItemInfo item in images) {
                        if (item.Title.ToLower() == tokenParams["Name".ToLower()].ToString().ToLower()) {
                            retItem = item;
                            break;
                        }
                    }
                    break;
                case "Latest":
                    retItem = images[images.Count - 1];
                    break;
                
                default:
                    PropertyNotFound = true;
                    return "";
            }

            if (retItem == null) {
                return "";
            }

            // good to go
            
            // add includes
            CDefault p = (CDefault)HttpContext.Current.Handler;
            p.ClientScript.RegisterClientScriptInclude("avt_jQuery_1_3_2_av3", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/jQuery-1.3.2.js?v=" + avt.FastShot.FastShotController.Build);
            p.ClientScript.RegisterClientScriptInclude("avtFastShot_1_5", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/avt.FastShot-1.5.js?v=" + avt.FastShot.FastShotController.Build);
            p.ClientScript.RegisterClientScriptInclude("jQueryLightbox_av3", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/jquery-lightbox/jquery.lightbox-av3.js?v=" + avt.FastShot.FastShotController.Build);
            p.ClientScript.RegisterClientScriptBlock(this.GetType(), "initLightbox", "avt.fs.initLightBox('" + HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot');", true);
            p.AddStyleSheet("skinLightbox", HttpContext.Current.Request.ApplicationPath + "DesktopModules/avt.FastShot/js/jquery-lightbox/css/lightbox.css");

            // build token
            return
                string.Format("<a class = 'lightbox' href = '{0}' alt = '{4}' title = '{4}'><img src = '{1}' width = '{2}' height = '{3}' title = '{4}' alt = '{4}' /></a>",
                    p.ResolveUrl(retItem.ImageUrl),
                    p.ResolveUrl(retItem.ThumbnailUrl),
                    retItem.ThumbWidth, retItem.ThumbHeight,
                    retItem.Title
                );
        }

    }

}