
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
        string _TplParams;

        public ItemInfo()
        {
            ItemId = -1;
        }

        public int ItemId
        {
            get { return _ItemId; }
            set { _ItemId = value; }
        }

        public int ModuleId
        {
            get { return _ModuleId; }
            set { _ModuleId = value; }
        }

        public string Title
        {
            get { return _ItemTitle; }
            set { _ItemTitle = value; }
        }

        public string Description
        {
            get { return _ItemDescription; }
            set { _ItemDescription = value; }
        }

        public string ThumbnailUrl
        {
            get
            {
                if (AutoGenerateThumb || string.IsNullOrEmpty(_ThumbUrl)) {
                    FastShotController fsCtrl = new FastShotController();
                    _ThumbUrl = fsCtrl.GenerateThumb(this);
                } else {
                    if (ItemId > 0 && (ThumbHeight <= 0 || ThumbWidth <= 0)) {
                        FastShotController ctrl = new FastShotController();
                        try {
                            ModuleController modCtrl = new ModuleController();
                            int portalId = ((ModuleInfo)modCtrl.GetModuleTabs(_ModuleId)[0]).PortalID;
                            PortalController portalCtrl = new PortalController();
                            PortalInfo portal = portalCtrl.GetPortal(portalId);
                            string url = _ThumbUrl;
                            if (url.IndexOf("http") != 0) {
                                url = HttpContext.Current.Request.MapPath(url);
                            }
                            System.Drawing.Image thumbImg = ctrl.LoadImageFromURL(url);
                            ctrl.UpdateItem(_ItemId, _ModuleId, _ItemTitle, _ItemDescription, _ThumbUrl, _ImageUrl, _ViewOrder, _AutoGenerateThumb, _ImageWidth, _ImageHeight, thumbImg.Width, thumbImg.Height, _FileTime, _TplParams);
                        } catch {
                            // set to -1 to flag error?
                            ctrl.UpdateItem(_ItemId, _ModuleId, _ItemTitle, _ItemDescription, _ThumbUrl, _ImageUrl, _ViewOrder, _AutoGenerateThumb, _ImageWidth, _ImageHeight, -1, -1, _FileTime, _TplParams);
                        }
                    }
                }
                return _ThumbUrl;
            }
            set { _ThumbUrl = value; }
        }

        public string ImageUrl
        {
            get { return _ImageUrl; }
            set { _ImageUrl = value; }
        }

        public int ViewOrder
        {
            get { return _ViewOrder; }
            set { _ViewOrder = value; }
        }

        public bool AutoGenerateThumb
        {
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

        public string TplParams
        {
            get { return _TplParams; }
            set { _TplParams = value; }
        }
    }
}