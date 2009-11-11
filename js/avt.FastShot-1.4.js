
if (!avt) { var avt = {}; }


avt.core_1_5b = {
    
    $ : avt_jQuery_1_3_2_av3,
    
    fnOnReady : [],
    
    options: {
        appPath : "",
        loaderIcon : "",
        bRunPngFix : true
    },
    
    init : function(opts) {
        avt.core_1_5b.$.extend(this.options, opts);
        this.options.bRunPngFix && this.fixPng();
        
        if (this.fnOnReady) {
            for(var i = 0; i < this.fnOnReady.length; i++) {
                this.fnOnReady[i]();
            }
            delete this.fnOnReady;
        }
    },
    
    ready : function(fn) {
        this.fnOnReady.push(fn);
    },
    
    fixPng : function () {
        avt.core_1_5b.$(document).ready(function() {
            var IE6 = (navigator.userAgent.toLowerCase().indexOf('msie 6') != -1) && (navigator.userAgent.toLowerCase().indexOf('msie 7') == -1)
            if (IE6) { // fix PNG transparency
                var arVersion = navigator.appVersion.split("MSIE")
                var version = parseFloat(arVersion[1])
                if ((version >= 5.5) && (document.body.filters)) {
                    for(var i=0; i<document.images.length; i++) {
                        var img = document.images[i]
                        var imgName = img.src.toUpperCase()
                        if (avt.core_1_5b.$(img).hasClass("pngFix") && imgName.substring(imgName.length-3, imgName.length) == "PNG") {
                            var imgID = (img.id) ? "id='" + img.id + "' " : ""
                            var imgClass = (img.className) ? "class='" + img.className + "' " : ""
                            var imgTitle = (img.title) ? "title='" + img.title + "' " : "title='" + img.alt + "' "
                            var imgStyle = "display:inline-block;" + img.style.cssText 
                            if (img.align == "left") imgStyle = "float:left;" + imgStyle
                            if (img.align == "right") imgStyle = "float:right;" + imgStyle
                            if (img.parentElement.href) imgStyle = "cursor:hand;" + imgStyle
                            var strNewHTML = "<span " + imgID + imgClass + imgTitle
                            + " style=\"" + "width:" + img.width + "px; height:" + img.height + "px;" + imgStyle + ";"
                            + "filter:progid:DXImageTransform.Microsoft.AlphaImageLoader"
                            + "(src=\'" + img.src + "\', sizingMethod='scale');\"></span>" 
                            img.outerHTML = strNewHTML
                            i = i-1
                        }
                    }
                }  
            }
        });
    }
}


avt.fs = avt.fastshot = { 
    $$ : avt.core_1_5b, 
    $  : avt_jQuery_1_3_2_av3,
    
    init : function() {
   
        avt.fastshot.$(document).ready(function() {
           avt.fastshot.$(".lightbox").lightbox({
                fileLoadingImage : '/DesktopModules/avt.FastShot/js/jquery-lightbox/images/loading.gif',
                fileBottomNavCloseImage : '/DesktopModules/avt.FastShot/js/jquery-lightbox/images/closelabel.gif'
            });
            
            avt.fastshot.$$.fixPng();
        });
    },
    
    initGrid : function(grid) {

        // make elements same size so they float nice
        var maxWidth = 0;
        var maxHeight = 0;
        
        avt.fs.$(grid).find("li").each(function() {
            if (avt.fs.$(this).width() > maxWidth) maxWidth = avt.fs.$(this).width();
            if (avt.fs.$(this).height() > maxHeight) maxHeight = avt.fs.$(this).height();
        });

        avt.fs.$(grid).find("li").css("width", maxWidth + "px").css("height", maxHeight + "px");
        avt.fs.$("head").append("<style>#"+ avt.fs.$(grid).parent().attr("id") +" ul.FastShot_grid .ui-sortable-placeholder { height: " + maxHeight + "px !important; } </style>");
    }
}

