
if (!avt) { var avt = {}; }
if (!avt.Common) { avt.Common = {}; }
if (!avt.FastShot) { avt.FastShot = { $ : avt_jQuery_1_3_2}; }

avt.fs = avt.fastshot = { 
    $$ : avt.core_1_2, 
    $  : avt_jQuery_1_3_2,
    
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
    }
}

