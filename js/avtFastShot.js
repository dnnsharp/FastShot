
if (!avt) { var avt = {}; }
if (!avt.Common) { avt.Common = {}; }
if (!avt.FastShot) { avt.FastShot = {}; }
if (!avt.FastShot.core) { avt.FastShot.core = {}; }

avt.Common = {

    disableInput : function() {

        /* if (typeof(Page_ClientValidate) == 'function' && !Page_ClientValidate()) {
            return; // page doesn't validate on client side
        } */
        
        if (jQuery('#NavXp_PreventInputDuringAJAX').length == 0) {
            jQuery("body").append("<div id = 'NavXp_PreventInputDuringAJAX' class = 'NavXpOverlayDisable' style ='display: none;'><div class = 'navxp_loader'><img src = '"+ NavXp.core.appPath +"/res/loader.gif' /></div></div>");
        }

        // find the window we want to cover

        jQuery("#NavXp_PreventInputDuringAJAX").css('display', 'block');
        jQuery("#NavXp_PreventInputDuringAJAX").dialog({ 
            modal: true, 
            resizable: false,
            width: jQuery(".ui-dialog").outerWidth(),
            height: jQuery(".ui-dialog").outerHeight(),
            title: "&nbsp;",
            closeOnEscape: false
        });
        
        jQuery(".NavXpOverlayDisable").css("opacity", 0).fadeTo('slow', 0.4);
        jQuery(".NavXpOverlayDisable").parent().parent().css("top", jQuery(".ui-dialog").css("top")).css("left", jQuery(".ui-dialog").css("left"));
        jQuery(".navxp_loader").css("margin-top", jQuery(".ui-dialog").outerHeight() / 2 - 20);

    },
    
    fixPng : function () {
        jQuery(document).ready(function() {
            var IE6 = (navigator.userAgent.toLowerCase().indexOf('msie 6') != -1) && (navigator.userAgent.toLowerCase().indexOf('msie 7') == -1)
            if (IE6) { // fix PNG transparency
                var arVersion = navigator.appVersion.split("MSIE")
                var version = parseFloat(arVersion[1])
                if ((version >= 5.5) && (document.body.filters)) 
                {
                    for(var i=0; i<document.images.length; i++)
                    {
                        var img = document.images[i]
                        var imgName = img.src.toUpperCase()
                        if (jQuery(img).hasClass("pngFix") && imgName.substring(imgName.length-3, imgName.length) == "PNG")
                        {
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
    },
    
    enableInput : function() {
    
        if (jQuery("#NavXp_PreventInputDuringAJAX").length === 0) return;
        
        jQuery(".NavXpOverlayDisable").fadeTo('slow', 0.0, function() { 
            jQuery("#NavXp_PreventInputDuringAJAX").dialog("destroy").remove();
        });
        
    },
    
    confirm : function(title, text, postBackId, data) {
        jQuery(".NavXpMsg").children(":visible").hide("slide", { direction: "down"}, "fast");
        if (jQuery('#NavXp_ConfirmDlg').length == 0) {
            jQuery('form').append("<div id = 'NavXp_ConfirmDlg' class = 'blue dlg_small' style ='display: none; padding-top: 20px;'></div>");
            jQuery('#NavXp_ConfirmDlg').append("<table border='0' cellspacing='0' cellpadding='0' width = '100%'></table>");
            jQuery('#NavXp_ConfirmDlg > table').append("<tr><td class = 'navxp_confirm_text'>"+ text +"</td></tr>");
            jQuery('#NavXp_ConfirmDlg > table').append("<tr><td align = 'middle' style = 'padding-left: 70px; padding-bottom: 10px; padding-top: 20px;'></td></tr>");
            jQuery('#NavXp_ConfirmDlg').children("table").children("tbody").children("tr:last").children("td:first")
                .append("<a href = 'javascript: void(0)' class = 'navxp_tab' onClick = 'jQuery(\"#NavXp_ConfirmDlg\").dialog(\"destroy\").remove()'>Cancel</a>")
                .append("<a href = 'javascript: void(0)' class = 'navxp_tab' onClick = 'jQuery(\"#NavXp_ConfirmDlg\").dialog(\"destroy\").remove(); avt.Common.disableInput(); __doPostBack(\""+ postBackId +"\", \"" + data + "\")'>Confirm</a>");
        }

        jQuery("#NavXp_ConfirmDlg").css('display', 'block');
        jQuery("#NavXp_ConfirmDlg").dialog({ 
            modal: true, 
            /* overlay: { 
                opacity: 0.3, 
                background: "black"
            }, */
            resizable: false,
            width: 300,
            //height: 100,
            title: title,
            closeOnEscape: false
        });

        jQuery("#NavXp_ConfirmDlg").parent().parent().css("opacity", 0).fadeTo('slow', 1);

    },
    
    
    openActivation : function(controlId, triggerCtrl) {
        jQuery(controlId).show();
        jQuery(controlId).dialog({ 
            modal: true, 
            overlay: { 
                opacity: 0.5, 
                background: "black"
            },
            //show: "scale",
            //hide: "drop",
            resizable: false,
            width: 400,
            height: 320,
            title: "FastShot Activation",
            closeOnEscape: false
        });
        
        jQuery(controlId).parent().parent().show("scale", {}, "normal", function() { 
                avt.Common.disableInput();
                __doPostBack(triggerCtrl, '');
            });
    },
    
    closeActivation: function(controlId) {
        //jQuery("#"+menuRenderCtrl).children(":first").html("Loading...");
        jQuery(".NavXpOverlayDisable").fadeTo('fast', 0.0, function() { 
            jQuery("#NavXp_PreventInputDuringAJAX").dialog("destroy").remove(); 
        });
        jQuery(controlId).parent().parent().hide("drop", {}, "slow", function() { 
                jQuery(controlId).dialog("destroy");//.eq(1).remove();
                //__doPostBack(triggerCtrl, ''); 
            });
    }

}


avt.FastShot.core = {

    init : function() {
   
        jQuery(document).ready(function() {
            jQuery(".lightbox").lightbox({
                fileLoadingImage : '/DesktopModules/avt.FastShot/js/jquery-lightbox/images/loading.gif',
                fileBottomNavCloseImage : '/DesktopModules/avt.FastShot/js/jquery-lightbox/images/closelabel.gif'
            });
            
            avt.Common.fixPng();
        });
    },
    
    addEditItem : function(triggerCtrl, itemId) {
    
        if (jQuery(".ui-dialog:visible").length > 0) {
            jQuery(".ui-dialog-content").dialog("close")
            return;
        }

        
        if (jQuery(".ui-dialog").length == 0) {

            jQuery("#newItem").show();

            jQuery("#newItem").css('display', 'block');
            jQuery("#newItem").dialog({ 
                modal: true, 
                /* overlay: { 
                    opacity: 0.5, 
                    background: "black"
                }, */
                resizable: false,
                width: 380,
                height: 400,
                title: "New Item",
                closeOnEscape: false
            });
        } else {
            jQuery("#newItem").dialog("open");
        }
        

        jQuery("#newItem").parent().parent().hide().show("scale", {}, "fast", function() { 
                avt.Common.disableInput();
                __doPostBack(triggerCtrl, itemId);
            });
    },
    
    deleteItem : function(triggerCtrl, itemId) {
        avt.Common.confirm("Confirm Delete", "Are you sure you want to delete this item?", triggerCtrl, itemId);
    },
    
    onSaveItem : function(objid, bEdit) {

        avt.FastShot.core._objSave = null;
        avt.FastShot.core._thumbUploaded = false;
        avt.FastShot.core._imageUploaded = false;
        
        var bValid = Page_ClientValidate();
        
        var iDocThumb = window.frames[0].document;
        var iDocImage = window.frames[1].document;

        if (bEdit && iDocThumb.forms[0].upl.value == "") { 
            jQuery("#iUploadThumbFrame").next("span").show();
            bValid = false;
        } else {
            jQuery("#iUploadThumbFrame").next("span").hide();
        }
        
        if (bEdit && iDocImage.forms[0].upl.value == "") { 
            jQuery("#iUploadImageFrame").next("span").show();
            bValid = false;
        } else {
            jQuery("#iUploadImageFrame").next("span").hide();
        }

        if (!bValid) {
            avt.Common.enableInput();
            return false; // page does not validate
        }
        
        // we're ready to upload
        iDocThumb.forms[0].submit();
        iDocImage.forms[0].submit();

        avt.Common.disableInput();
        avt.FastShot.core._objSave = objid;
        return false; // we'll submit later, after files are uploaded
    },
    
    
    thumbUploaded : function(hdId) {
    
        if (!avt.FastShot.core._objSave) { return; }
    
        var iDocThumb = window.frames[0].document;
        jQuery("#" + hdId).val(iDocThumb.forms[0].hdFilename.value)
        
        avt.FastShot.core._thumbUploaded = true;
        
        if (avt.FastShot.core._imageUploaded === true) {
            var objid = avt.FastShot.core._objSave;
            avt.FastShot.core._objSave = null;
            __doPostBack(objid,'');
        }
    },
    
    imageUploaded : function(hdId) {
    
        if (!avt.FastShot.core._objSave) { return; }
    
        var iDocImage = window.frames[1].document;
        jQuery("#" + hdId).val(iDocImage.forms[0].hdFilename.value)
        
        avt.FastShot.core._imageUploaded = true;
        
        if (avt.FastShot.core._thumbUploaded === true) {
            var objid = avt.FastShot.core._objSave;
            avt.FastShot.core._objSave = null;
            __doPostBack(objid,'');
        }
    }
}

avt.FastShot.core.init();