<%@ Page Language="c#" AutoEventWireup="True" Explicit="True" Inherits="avt.FastShot.FastShotStudio" CodeFile="FastShotStudio.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>

    <title>FastShot Studio</title>

    <link type ="text/css" rel="stylesheet" href = "<%= TemplateSourceDirectory + "/studio.css" %>" />
    <link type = "text/css" rel = "stylesheet" href = "<%=TemplateSourceDirectory %>/js/ui-themes/smoothness/jquery-ui-1.7.2.css" />
    <link rel="stylesheet" type="text/css" href="<%=TemplateSourceDirectory %>/js/layout/css/complex.css" />
    <link rel="stylesheet" type="text/css" href="<%=TemplateSourceDirectory %>/js/beautytips/bt-0.9.5-rc1/jquery.bt.css" />
    <link rel = "stylesheet" type = "text/css" href = "<%=TemplateSourceDirectory %>/js/beautytips/bt-0.9.5-rc1/jquery.bt.css" />
    <link rel="stylesheet" type="text/css" href="<%=TemplateSourceDirectory %>/js/jGrowl-1.2.0/jquery.jgrowl.css" />


    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jquery-1.3.2.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jquery-ui-1.7.2.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
<%--    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jquery-ui.accordion-1.7.2.js"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jquery-ui.tabs-1.7.2.js"></script>--%>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/layout/jquery.layout.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/beautytips/bt-0.9.5-rc1/jquery.bt.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/jGrowl-1.2.0/jquery.jgrowl.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/excanvas_r3/excanvas.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/scrollTo/jquery.scrollTo.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>

    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/avt.FastShot-1.5.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/studio-1.0.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>
    
    <script type = "text/javascript" src = "<%=TemplateSourceDirectory %>/js/json2.js?v=<%= avt.FastShot.FastShotController.Build %>"></script>

    <style type = "text/css">
    
    p {
        margin: 1em 0;
    }

    .ui-layout-pane-west, .ui-layout-pane-east {
        /* OVERRIDE 'default styles' */
        padding: 0px 0 0px 0 !important;
        overflow: hidden !important;
    }
    
    .pane-center, ui-layout-content, #mainContent .ui-layout-content {
        padding: 0px;
    }
    
    ..ui-layout-pane-east {
        padding: 0px;
    }

    #lefttabs {
        padding: 0px;
    }
    
    #lefttabs .ui-tabs-nav li a {
        padding: 4px !important;
        font-size: 11px !important;
    }
    
    #helpTabs {
        padding: 0px;
    }
    
    #helpTabs .ui-tabs-nav li a {
        padding: 4px !important;
        font-size: 11px !important;
    }


    .dtree {
            width: 280px; 
            height: 350px; 
            overflow: auto; 
            border: 1px solid #d2d2d2; 
            float: left;
            display: none;
        }
        
    </style>
    
    <script type="text/javascript">
    var outerLayout; // init global vars

    avt.fs.$(document).ready( function() {

        // create the OUTER LAYOUT
        outerLayout = avt.fs.$("body").layout( layoutSettings_Outer );

        outerLayout.addToggleBtn( "#tbarToggleFileManager", "south" );
        outerLayout.addPinBtn( "#tbarPinSidebar", "east" );

        // save selector strings to vars so we don't have to repeat it
        // must prefix paneClass with "form > " to target ONLY the outerLayout panes
        var westSelector = "form > .ui-layout-west"; // outer-west pane
        var eastSelector = "form > .ui-layout-east"; // outer-east pane

         // CREATE SPANs for pin-buttons - using a generic class as identifiers
        avt.fs.$("<span></span>").addClass("pin-button").prependTo( westSelector );
        avt.fs.$("<span></span>").addClass("pin-button").prependTo( eastSelector );
        
        // BIND events to pin-buttons to make them functional
        outerLayout.addPinBtn( westSelector +" .pin-button", "west");
        outerLayout.addPinBtn( eastSelector +" .pin-button", "east" );

         // CREATE SPANs for close-buttons - using unique IDs as identifiers
        avt.fs.$("<span></span>").attr("id", "west-closer" ).prependTo( westSelector );
        avt.fs.$("<span></span>").attr("id", "east-closer").prependTo( eastSelector );
        
        // BIND layout events to close-buttons to make them functional
        outerLayout.addCloseBtn("#west-closer", "west");
        outerLayout.addCloseBtn("#east-closer", "east");


        // DEMO HELPER: prevent hyperlinks from reavt.fs.studio.loading page when a 'base.href' is set
        avt.fs.$("a").each(function () {
            var path = document.location.href;
            if (path.substr(path.length-1)=="#") path = path.substr(0,path.length-1);
            if (this.href.substr(this.href.length-1) == "#") this.href = path +"#";
        });

        avt.fs.$("#lefttabs")
                .tabs()
                .find(".ui-tabs-nav")
                    .sortable({ axis: 'x', zIndex: 2 });

        avt.fs.$("#helpTabs")
                .tabs()
                .find(".ui-tabs-nav")
                    .sortable({ axis: 'x', zIndex: 2 });

        avt.fs.$("#sbmTabs")
                .tabs()
                .find(".ui-tabs-nav")
                    .sortable({ axis: 'x', zIndex: 2 })
            ;
            

        // ACCORDION - inside the West pane
        
        //avt.fs.$("#accordion_controls").parent().css("overflow", "hidden");
        
        avt.fs.$("#accordion_controls").accordion({
            fillSpace: true
        });


        // resize everything so it fits nice
        avt.fs.studio.resizeEditor();

        // disable tabs until token is loaded
        avt.fs.$("#sbmTabs").tabs('disable', 1);
        avt.fs.$("#sbmTabs").tabs('disable', 2);

        // init paths
       avt.fs.pageUrl = '<%= Request.Url.OriginalString %>';
       avt.fs.serverRoot = '<%= ResolveUrl("~/") %>';
       avt.fs.modRoot = '<%= TemplateSourceDirectory[TemplateSourceDirectory.Length - 1] == '/' ? TemplateSourceDirectory : TemplateSourceDirectory + "/" %>';
       avt.fs.apiUrl = avt.fs.modRoot + "FastShotApi.aspx";
       avt.fs.mid = <%= Request.QueryString["mid"] %>;
       avt.fs.palias = "<%= Request.QueryString["alias"] %>";

        avt.fs.studio.init();

        // populate images
        avt.fs.studio.loading(true);
        avt.fs.studio.loadImages();
        
        // initalize tooltips
        initAllTooltips();
    });


    function initAllTooltips(parent) {
        initTooltips(parent, "hover");
        initTooltips(parent, "click");
        initTooltips(parent, ['focus', 'blur']);
        initTooltips(parent, "hover");
        initTooltips(parent, "hover", "hover_v", ["bottom", "top"]);
        initTooltips(parent, ['focus', 'blur'], "focus_v", ["bottom", "top"]);
    }

    function initTooltips(parent, action, cssClass, pos) {
    
        if (!cssClass)
            cssClass = action;
        
        if (!pos)
            pos = ["most"];
        
        if (!parent)
            parent = avt.fs.$("body");
            
        parent.find('.tooltip_' + cssClass).bt({
              padding: 20,
              width: 200,
              spikeLength: 30,
              spikeGirth: 10,
              cornerRadius: 20,
              fill: 'rgba(0, 0, 0, .8)',
              strokeWidth: 2,
              strokeStyle: '#CC0',
              cssStyles: {color: '#FFF', fontWeight: 'bold', zIndex: '99999'},
              offsetParent: avt.fs.$("body"), // TODO: analyze this for sf
              trigger: action,
              positions: pos
            });
    }


var layoutSettings_Outer = {
        name: "outerLayout" // NO FUNCTIONAL USE, but could be used by custom code to 'identify' a layout
        // options.defaults apply to ALL PANES - but overridden by pane-specific settings
    ,    defaults: {
            size:                    "auto"
        ,    minSize:                50
        ,    paneClass:                "pane"         // default = 'ui-layout-pane'
        ,    resizerClass:            "resizer"    // default = 'ui-layout-resizer'
        ,    togglerClass:            "toggler"    // default = 'ui-layout-toggler'
        ,    buttonClass:            "button"    // default = 'ui-layout-button'
        ,    contentSelector:        ".content"    // inner div to auto-size so only it scrolls, not the entire pane!
        ,    contentIgnoreSelector:    "span"        // 'paneSelector' for content to 'ignore' when measuring room for content
        ,    togglerLength_open:        35            // WIDTH of toggler on north/south edges - HEIGHT on east/west edges
        ,    togglerLength_closed:    35            // "100%" OR -1 = full height
        ,    hideTogglerOnSlide:        true        // hide the toggler when pane is 'slid open'
        ,    togglerTip_open:        "Close This Pane"
        ,    togglerTip_closed:        "Open This Pane"
        ,    resizerTip:                "Resize This Pane"
        //    effect defaults - overridden on some panes
        ,    fxName:                    "slide"        // none, slide, drop, scale
        ,    fxSpeed_open:            750
        ,    fxSpeed_close:            1500
        ,    fxSettings_open:        { easing: "easeInQuint" }
        ,    fxSettings_close:        { easing: "easeOutQuint" }
        , enableCursorHotkey: false
    }
    ,    north: {
            spacing_open:            1            // cosmetic spacing
        ,    togglerLength_open:        0            // HIDE the toggler button
        ,    togglerLength_closed:    -1            // "100%" OR -1 = full width of pane
        ,    resizable:                 false
        ,    slidable:                false
        //    override default effect
        ,    fxName:                    "none"
        }
    ,    south: {
            size:                    200
            //,maxSize:                400
        ,    spacing_closed:            10            // HIDE resizer & toggler when 'closed'
        ,    slidable:                false        // REFERENCE - cannot slide if spacing_closed = 0
        ,    initClosed:                true
        }
    ,    west: {
            size:                    250
        ,    spacing_closed:            21            // wider space when closed
        ,    togglerLength_closed:    21            // make toggler 'square' - 21x21
        ,    togglerAlign_closed:    "top"        // align to top of resizer
        ,    togglerLength_open:        0            // NONE - using custom togglers INSIDE west-pane
        ,    togglerTip_open:        "Close Help"
        ,    togglerTip_closed:        "Open Help"
        ,    resizerTip_open:        "Resize Help"
        ,    slideTrigger_open:        "click"     // default
        ,    initClosed:                false
        //    add 'bounce' option to default 'slide' effect
        ,    fxSettings_open:        { easing: "easeOutBounce" }
        }
    ,    east: {
            size:                    240
        ,    spacing_closed:            21            // wider space when closed
        ,    togglerLength_closed:    21            // make toggler 'square' - 21x21
        ,    togglerAlign_closed:    "top"        // align to top of resizer
        ,    togglerLength_open:        0             // NONE - using custom togglers INSIDE east-pane
        ,    togglerTip_open:        "Close Tokens List"
        ,    togglerTip_closed:        "Open Tokens List"
        ,    resizerTip_open:        "Resize Tokens List"
        ,    slideTrigger_open:        "mouseover"
        ,    initClosed:                false
        //    override default effect, speed, and settings
        ,    fxName:                    "drop"
        ,    fxSpeed:                "normal"
        ,    fxSettings:                { easing: "" } // nullify default easing
        }
    ,    center: {
            paneSelector:            "#mainContent"             // sample: use an ID to select pane instead of a class
        //,    onresize:                "innerLayout.resizeAll"    // resize INNER LAYOUT when center pane resizes
        ,    minWidth:                200
        ,    minHeight:                200
        ,   onresize_end:           function () { avt.fs.studio.resizeEditor(); }
        }
    };


    </script>

</head>
<body>
<form runat = "server" method = "post" enctype = "multipart/form-data">


<div class="ui-layout-west">

    <div class="header" style = "height: 14px; text-align: left; padding-left: 24px; font-size: 13px;">Settings</div>
    
    <div id = "lefttabs" style = "">
        <ul>
            <li><a href="#tabs-left-settings">Settings</a></li>
            <li><a href="#tabs-left-cat">Categories</a></li>
            <li><a href="#tabs-left-api">Api</a></li>
        </ul>
        
        <div id="tabs-left-settings" style = "padding: 0px; padding-bottom: 4px; font-size: 12px;">

            <div id="accordion_controls" class="basic" style = "font-size: 11px;">

                <a href="#">General Settings</a>
                <div style = "padding: 6px;" id = "pnlGeneralSettings">
                
                    <div class = "set_modTitleC">
                        <div class = "label_blue"><b>Module: </b></div> <span id = "set_modTitle"></span>
                    </div>
                    <div class = "grayed_desc">
                        Focus controls for additional information...
                    </div>
                    <br />
                    
                    <div class = "label_blue">Template:</b></div>
                    <select id = "ddTemplates" class = "wizTextInputMedium tooltip_hover" title = "Template determines how images are rendered on front-end." size = "5"></select>
                    <div class = "grayed_desc">
                        Template selection only reflects on front-end.
                    </div>
                    <br /><br />
                    
                    <div class = "grayed_desc">
                        Set only one coordinate to scale down while maintaining proportions. Set both coordinates to scale to desired size with distortions (if proportions don't match).
                    </div>
                    <br />
                    
                    <div class = "label_blue"><b>Thumb Width:</b></div>
                    <input type = "text" id = "set_modWidth" class = "wizTextInputXSmall tooltip_focus" title = "Select the width at which to auto generate thumbnails. If width is set and height is not set (it equals 0), then the images are scaled at the target width (therefore maintainng proprotions)." />
                    <br /><br />
                    
                    <div class = "label_blue"><b>Thumb Height:</b></div>    
                    <input type = "text" id = "set_modHeight" class = "wizTextInputXSmall tooltip_focus" title = "Select the height at which to auto generate thumbnails. If height is set and width is not set (it equals 0), then the images are scaled at the target height (therefore maintainng proprotions)." />
                    
                    <br /><br /><br /><br />
                    
                    <a href = "javascript: avt.fs.studio.saveSettings();" class = "blue btn_border">
                        <img src = "<%=TemplateSourceDirectory %>/res/save.gif" border = "0" align = "absmiddle" style = "padding: 3px;" />Save Settings
                    </a>

                </div>
                
                <%--<a href="#">Display Settings</a>
                <div>
                    
                </div>
                
                <a href="#">Advanced Settings</a>
                <div>
                    
                </div>--%>
                
            </div>
        </div>
        
        <div id="tabs-left-cat" style = "font-size: 12px; padding: 8px; font-style: italic; color: #525252;">
            <br /><br />
            This is a placeholder for future development. <br />
            You will be able to define categories (folders) to organize your images<br /><br />
            If you'd like to comment on this or other enhancements please do so on our <a href = "http://www.avatar-soft.ro/Support/Forums/tabid/99/Default.aspx">forums</a>.
            <br /><br /><br />
        </div>
        <div id="tabs-left-api" style = "font-size: 12px; padding: 8px; font-style: italic; color: #525252;">
            <br /><br />
            This is a placeholder for future development. 
            FastShot uses an Web Api to manage content through AJAX. This Api can also be used to manage content remotely and programatically by anyone.<br />
            Although the Api is complete, it will not be released until next version. From this window you will be able to setup Api access. <br /><br />
            If you'd like to comment on this or other enhancements please do so on our <a href = "http://www.avatar-soft.ro/Support/Forums/tabid/99/Default.aspx">forums</a>.
            <br /><br /><br />
        </div>
    </div>

</div>

<div class="ui-layout-east">

    <div class="header" style = "height: 14px; text-align: left; padding-left: 24px; font-size: 13px;">Item Properties</div>

    <div id = "helpTabs" style = "">
        <ul>
            <li><a href="#tabs-help-props">Properties</a></li>
            <li><a href="#tabs-help-links">Quick Links</a></li>
        </ul>

        <div id="tabs-help-props" style = "padding: 8px; padding-bottom: 4px; font-size: 11px;">
            <div class = "propsNoItem" style = "">Click Item in main area to load its properties...</div>
            <div class = "propsRoot" style = "">
                <span class = "itemId" style = "display: none;"></span>
                
                <div class = "label_blue">Title: </div><br />
                <input type = "text" class = "propsItemName wizTextInputSmall" style = "width: 95%;"/>
                <div class = "grayed_desc">Can contain <a href = "http://www.avatar-soft.ro/Products/MyTokens/tabid/148/Default.aspx">MyTokens</a> if installed...</div>
                <span class = "wizerror" id = "errEditTitle"></span>
                <br />
                
                <div class = "label_blue">Description: </div><br />
                <textarea class = "propsItemDesc wizTextInputSmall" style = "width: 95%;"></textarea>
                <div class = "grayed_desc">Can contain <a href = "http://www.avatar-soft.ro/Products/MyTokens/tabid/148/Default.aspx">MyTokens</a> if installed...</div>
                <br />
                
                <div class = "grayed_desc">
                    To change image or thumbnail, create new item and delete this one.<br /><br />
                </div>
                
                <div class = "label_blue" style = "float: none; text-align:left;">Image Url: </div>
                <div class = "propsItemImage" style = ""></div>
                <br />
                
                <div class = "label_blue" style = "float: none; text-align:left;">Thumb Url: </div>
                <div class = "propsItemThumb" style = ""></div>
                <br />
                
                <div class = "label_blue">Template Params: </div><br />
                <input type = "text" class = "propsItemParams wizTextInputSmall" style = "width: 95%;"/>
                <br /><br />
                
                <a href = "javascript: avt.fs.studio.updateItem();" class = "blue btn_border">
                    <img src = "<%=TemplateSourceDirectory %>/res/edit.gif" border = "0" align = "absmiddle" style = "padding: 3px;"/>Update
                </a>
                
            </div>
        </div>

        
        <div id="tabs-help-links" style = "padding: 8px; padding-bottom: 4px; font-size: 12px;">
            <ul class = "links_list">
                <li><a href = "http://www.avatar-soft.ro/Products/FastShot/tabid/143/Default.aspx">Product Homepage</a></li>
                <li><a href = "<%= avt.FastShot.FastShotController.DocSrv %>">Documentation</a></li>
                <li><a href = "http://www.avatar-soft.ro/Support/Forums/tabid/99/Default.aspx">Forums</a></li>
                <li><a href = "<%= avt.FastShot.FastShotController.BuyLink %>">Purchase</a></li>
            </ul>
        </div>
    </div>

</div>


<div class="ui-layout-north">
    <div class="header" style = "text-align: left; padding: 4px; font-size: 16px;">
        <div style = "float: right; font-size: 11px; text-align: right;">
            <span style = "">build <b><%= avt.FastShot.FastShotController.VersionAll%></b></span>
            by <a href = "http://www.avatar-soft.ro">Avatar Software</a>
        </div>
        <div class = "load_maker_vis" style = "float: left; visibility: visible; margin: 1px 3px 0 0;">
            <img src = "<%=TemplateSourceDirectory %>/res/ajax-loader_small.gif" />
        </div>
        FastShot Studio<span class = "current_token" style = "display: none;">- </span>
        <div style = "clear: both;"></div>
    </div>

    <ul class="toolbar">
        <li><div style = "padding: 4px 4px 4px 22px;" onclick = "avt.fs.studio.saveSettings();" class = "btn_new tooltip_hover" title = "Click here to save your changes."> Save Settings</div></li>
        <li><div style = "padding: 4px 4px 4px 22px;" onclick = "avt.fs.studio.newItem();" class = "btn_new_search tooltip_hover" title = "Add new image to current module."> New Image</div></li>
        <li id = "tbarToggleFileManager"><div style = "padding: 4px 4px 4px 22px;" class = "btn_file_manager tooltip_hover" title = "Click here to open File Manger and start importing images from your portal."> File Manager</div></li>
        <%--<li id = "Li1"><div style = "padding: 4px 4px 4px 22px;" class = "btn_test_search tooltip_hover" title = "Quickly test your search instances and rules with this simple and fast tool."> Import</div></li>--%>
        <%--<li id = "Li2"><div style = "padding: 4px 4px 4px 22px;" class = "btn_test_search tooltip_hover" title = "Quickly test your search instances and rules with this simple and fast tool."> Export</div></li>--%>
        <li><div style = "padding: 4px 4px 4px 22px;" onclick = "avt.fs.studio.clearCache();" class = "btn_reindex tooltip_hover" title = "FastShot is caching thumbnails and settings. Click here to have cache cleared."> Clear Cache</div></li>
        <li><div style = "padding: 4px 4px 4px 22px;" onclick = "window.location = avt.fs.returnUrl;" class = "btn_return tooltip_hover" title = "Click here to return to previous page."> Back</div></li>
        <li id="tbarPinSidebar"  style = "float: right; margin-right: 4px;"><div class = "btn_help" style = "padding: 4px 4px 4px 22px;">Help</div></li>
    </ul>
</div>


<div class="ui-layout-south" style = "">
    <div class="header" style = "text-align: left; padding: 3px; font-size: 12px;" >File Browser</div>
    
    <ul id = "fileBrowserFolders"></ul>
    <ul id = "fileBrowserFiles"></ul>
    <div id = "fileBrowserUpload">
        <b>Upload image</b><br />
        <input type="file" name="uplFile" /><br />
        <asp:CheckBox runat = "server" ID = "cbUploadOverwrite" Text = "overwrite existing file (if exists)" /><br />
        <a href = "javascript: ;" onclick = "avt.fs.AIM.submit(document.forms[0], {'onStart' : startCallback, 'onComplete' : avt.fs.fb.uploadComplete}); document.forms[0].submit()">upload</a>
    </div>
    
    <asp:HiddenField runat = "server" ID = "uploadPath" />
    <script type="text/javascript">
		function startCallback() {
			avt.fs.$("#<%= uploadPath.ClientID %>").val(avt.fs.$("#fileBrowserFiles").find(".folderHeader").text()) 
			return true;
		}

	</script>


    <div style = "clear: both;"></div>


    <ul style = "display: none;">
    <li id = "tplFolder">
        <span style = "width: 16px; height: 16px; float: left; display: block;">
            <a href = "javascript: ;" onclick = "avt.fs.fb.expand(avt.fs.$(this));" class = "folderExpand">
                <img src = "<%=TemplateSourceDirectory %>/res/plus.gif" border = "0" />
            </a>
            <a href = "javascript: ;" onclick = "avt.fs.fb.collapse(avt.fs.$(this));" class = "folderCollapse">
                <img src = "<%=TemplateSourceDirectory %>/res/minus.gif" border = "0" />
            </a> 
        </span>
        <a href = "javascript: ;" rel = "" class = "folderLink" onclick = "avt.fs.fb.loadFiles(avt.fs.$(this));"></a>
        <ul class = "subFolders"></ul>
    </li>
    </ul>
    
    <ul style = "display: none;">
    <li id = "tplFolderHeader" class = "folderHeader">
        
    </li>
    <li id = "tplFile">
        <span style = "float: right; display: block; margin-right: 10px;">
            <a href = "javascript: ;" onclick = "avt.fs.studio.useAsImage(this);" class = "fileUseAsImage">
                <img src = "<%=TemplateSourceDirectory %>/res/use_image.gif" border = "0" />use as image
            </a>
            <a href = "javascript: ;" onclick = "avt.fs.studio.useAsThumb(this);" class = "fileUseAsThumb">
                <img src = "<%=TemplateSourceDirectory %>/res/use_thumb.gif" border = "0" />use as thumb
            </a> 
        </span>
        <a href = "" rel = "" class = "fileLink" onclick = "return false;" ondblclick = "avt.fs.studio.useAsImage(this);"></a>
        <a href = "javascript: ;" onclick = "avt.fs.fb.deleteFile(avt.fs.$(this).parents('li').find('.fileLink'));"><img src = "<%=TemplateSourceDirectory %>/res/delete.gif" border = "0" /></a>
        <span style = "clear: both; display: block;"></span>
    </li>
    </ul>
    
</div>


<div id="mainContent" style = "overflow: hidden;">

    <div class="ui-layout-center">
        <div class="ui-layout-content">
            <div id = "sbmTabs" style = "font-size: 13px;">
                <ul>
                    <li><a href="#tabs-sb-info">Images</a></li>
                    <li><a href="#tabs-sb-new">New Image</a></li>
                </ul>
                <div id="tabs-sb-info" style = "">
                    <div class = "main_auto_scroll" style = "overflow: auto;">
                    
                        <div id = "itemTrashBinBox" style = "float: right; border: 1px solid #bb5555; width: 28%;">
                            <div class = "trashBinTitle" style = "padding: 5px;">Trash</div>
                            
                            <ul id = "itemTrashBin" class = "FastShot_grid" style = "width: 100%; overflow: visible; height: 80%; text-align: left; overflow: auto;  ">
                                <li class = "trashEmpty">Drag items here<br /> to delete them...</li>
                            </ul>
                            <div class = "trashBinActions">
                                <a href = "javascript: avt.fs.studio.emptyRecycleBin();" class = "blue btn_border">
                                    <img src = "<%=TemplateSourceDirectory %>/res/action_delete.gif" border = "0" align = "absmiddle" style = "padding: 3px;" />Empty
                                </a>
                                <%--<a href = "javascript: ;">restore all</a>--%>
                            </div>
                        </div>
                        
                        <div id = "pnlTipOrdering" style = "position: absolute; margin-top: -10px; padding: 3px; border: 1px solid #885555; color: #fafafa; background-color: #6666ee; font-size: 11px;">
                            <b>Tip: </b>Click, hold and <b>drag</b> images around the order them...
                        </div>
                        
                        <div id = "pnlOrderingChanges" style = "position: absolute; margin-top: -10px; padding: 3px; border: 1px solid #885555; color: #fafafa; background-color: #ee6666; font-size: 11px; display: none;">
                            <a href = "javascript: avt.fs.studio.saveOrdering();" style = "color: #fafafa; font-weight: bold;">Click here to Save New Ordering</a> when done...
                        </div>
                        <ul id = "imgContainer" class = "FastShot_grid" style = "width: 65%; height: 100%; overflow: auto;"></ul>
                        <div style = "clear: both;"></div>
                    </div>
                </div>


                <div id="tabs-sb-new">
                    <div class = "main_auto_scroll" style = "overflow: auto; padding: 20px 0 0 20px;" id = "newItem_Container">
                        <div class = "page_title">New Image</div>
                        <div class = "grayed_desc">Focus controls for additional information...</div>

                        <br /><br />
                        <div class = "label_blue lbl">Title</div>
                        <input type = "text" id = "newItem_Title" class = "wizTextInputLarge tooltip_focus" title = "Please provide a title. It is used as a label in listing and on lightbox screen." />
                        <span class = "wizerror" id = "newItem_TitleErr"></span>
                        <br /><br />
                        
                        <div class = "label_blue lbl">Description</div>
                        <textarea type = "text" id = "newItem_Desc" class = "wizTextInputLarge tooltip_focus" title = "Optionally provide a description that will be displayed as the alt/title of the thumbnail and on lightbox screen."></textarea>
                        <br /><br />
                        
                        <div class = "label_blue lbl">Image Url</div>
                        <input type = "text" id = "tbImageUrl" class = "wizTextInputLarge tooltip_focus" title = "Provide the image URL or open the <i>File Manager</i> to select an existing portal image or upload new one. Once you locate the image, drag&drop the filename into this field or click <i>use as image</i>." />
                        <a href = "javascript: outerLayout.open('south');" class = "blue" >
                            <img src = "<%=TemplateSourceDirectory %>/res/OpenFolder.gif" border = "0" style = "margin-right: 4px;" align = "absmiddle" />
                            open file manager
                        </a>
                        <br /><div class = "wizerror" id = "newItem_ImageUrlErr" style = "margin-left: 90px;"></div>
                        <br />
                        
                        <div class = "label_blue lbl">Thumbnail Url</div>
                        <input type = "text" id = "tbThumbUrl" class = "wizTextInputLarge tooltip_focus" title = "Please provide a title. It is used as a label in listing and on lightbox screen." />
                        <a href = "javascript: outerLayout.open('south');" class = "blue">
                            <img src = "<%=TemplateSourceDirectory %>/res/OpenFolder.gif" border = "0" style = "margin-right: 4px;" align = "absmiddle" />
                            open file manager
                        </a>
                        <div style = "margin-left: 90px;">
                            <label class = "tooltip_hover" title = "Auto generates thumbnail based on module settings.">
                                <input type = "checkbox" id = "cbAutoGenerate" checked = "checked" onclick = "avt.fs.studio.toggleAutoGenerate(this.checked);" />Auto Generate Thumbnail
                            </label>
                        </div>
                        <div class = "wizerror" id = "newItem_ThumbUrlErr" style = "margin-left: 90px;"></div>
                        <br />

                        <br /><br /><br /><br /><br /><br />
                        <div style = "" class = "wizBar">
                            <div style = "float: right; margin-right: 20px;" >
                                 <a href = "javascript: ;" class = "wizNext blue btn_border" onclick = "avt.fs.studio.saveItem();" class = "blue btn_border">
                                    <img src = "<%=TemplateSourceDirectory %>/res/save.gif" border = "0" style = "margin-right: 4px;" align = "absmiddle" />
                                    Save
                                 </a>
                            </div>
                            <a href = "javascript: ;" onclick = "avt.fs.studio.hideNewItem();"  style = "float: left; display: block; margin-left: 20px;  " class = "blue btn_border">
                                <img src = "<%=TemplateSourceDirectory %>/res/delete.gif" border = "0" style = "margin-right: 4px;" align = "absmiddle" /> 
                                Cancel New Item
                            </a>
                            <div style = "clear: both;"></div>
                        </div>
                        
                    </div>
                </div>
            </div>
        </div>
    </div>

</div>



</form> 



<ul>
<li id = "tplItemView" class = "itemViewRoot">
    <span class = "itemId" style = "display: none;"></span>
    <img src = "" class = "itemThumb" />
</li>
</ul>

</body>
</html>