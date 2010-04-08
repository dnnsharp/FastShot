
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


avt.fs = { 
    $$ : avt.core_1_5b, 
    $  : avt_jQuery_1_3_2_av3,
    
    initLightBox : function(fsRootUrl) {
   
        avt.fs.$(document).ready(function() {
           avt.fs.$(".lightbox").lightbox({
                fileLoadingImage : fsRootUrl + '/js/jquery-lightbox/images/loading.gif',
                fileBottomNavCloseImage : fsRootUrl + '/js/jquery-lightbox/images/closelabel.gif'
            });
            
            avt.fs.$$.fixPng();
        });
    },
    
    initSpaceGallery: function(fsRootUrl) {
        avt.fs.$(document).ready(function() {
            avt.fs.$('.fsSpaceGallery').spacegallery({loadingClass: 'loading'});
        });

    },
    
    initGalleriffic: function(fsRootUrl) {
        avt.fs.$(document).ready(function($) {
            // We only want these styles applied when javascript is enabled
            $('div.fsgContent').css('display', 'block');

            // Initially set opacity on thumbs and add
            // additional styling for hover effect on thumbs
            var onMouseOutOpacity = 0.67;
            $('.fsgThumbs ul.fsgThumbsC li, div.fsgThumbs a.pageLink').opacityrollover({
                mouseOutOpacity:   onMouseOutOpacity,
                mouseOverOpacity:  1.0,
                fadeSpeed:         'fast',
                exemptionSelector: '.selected'
            });
            
            // Initialize Advanced Galleriffic Gallery
            $('.fsGalleriffic').each(function() {
                var $this = $(this);
                var gallery = $this.find('.fsgThumbs').galleriffic({
                    delay:                     2500,
                    numThumbs:                 6,
                    preloadAhead:              10,
                    enableTopPager:            false,
                    enableBottomPager:         false,
                    imageContainerSel:         $this.find('.slideshow'),
                    controlsContainerSel:      $this.find('.controls'),
                    captionContainerSel:       $this.find('.caption-container'),
                    loadingContainerSel:       $this.find('.loader'),
                    renderSSControls:          true,
                    renderNavControls:         true,
                    playLinkText:              'Play Slideshow',
                    pauseLinkText:             'Pause Slideshow',
                    prevLinkText:              '&lsaquo; Previous Photo',
                    nextLinkText:              'Next Photo &rsaquo;',
                    nextPageLinkText:          'Next &rsaquo;',
                    prevPageLinkText:          '&lsaquo; Prev',
                    enableHistory:             true,
                    autoStart:                 false,
                    syncTransitions:           true,
                    defaultTransitionDuration: 900,
                    onSlideChange: function(prevIndex, nextIndex) {
                        
                        this.find('ul.fsgThumbsC').children()
                            .eq(prevIndex).fadeTo('fast', onMouseOutOpacity).end()
                            .eq(nextIndex).fadeTo('fast', 1.0);

                        // Update the photo index display
                        var _r = this.parents(".fsGalleriffic:first");
                        _r.find('div.photo-index')
                            .html('Photo '+ (nextIndex+1) +' of '+ this.data.length);
                        
                    },
                    onPageTransitionOut: function(callback) {
                        this.fadeTo('fast', 0.0, callback);
                    },
                    onPageTransitionIn: function() {
                        var prevPageLink = this.find('a.prev').css('visibility', 'hidden');
                        var nextPageLink = this.find('a.next').css('visibility', 'hidden');
                        
                        // Show appropriate next / prev page links
                        if (this.displayedPage > 0)
                            prevPageLink.css('visibility', 'visible');

                        var lastPage = this.getNumPages() - 1;
                        if (this.displayedPage < lastPage)
                            nextPageLink.css('visibility', 'visible');

                        this.fadeTo('fast', 1.0);
                    }
                });

                /**************** Event handlers for custom next / prev page links **********************/

                gallery.find('a.prev').click(function(e) {
                    gallery.previousPage();
                    e.preventDefault();
                });

                gallery.find('a.next').click(function(e) {
                    gallery.nextPage();
                    e.preventDefault();
                });
            });
            
            
        });
    },
    
    inits3Slider: function(fsRootUrl) {
        avt.fs.$(document).ready(function($) {
            $('.fss3Slider').s3Slider({
                timeOut: 4000 
            });
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

