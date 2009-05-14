
if (!avt) { var avt = {}; }
if (!avt.Common) { avt.Common = {}; }
if (!avt.FastShot) { avt.FastShot = { $ : avt_jQuery_1_3_2}; }

avt.fastshot = { 
    $$ : avt.core_1_0, 
    $  : avt_jQuery_1_3_2,
    
    init : function() {
   
       avt.fastshot.$(document).ready(function() {
           avt.fastshot.$(".lightbox").lightbox({
                fileLoadingImage : '/DesktopModules/avt.FastShot/js/jquery-lightbox/images/loading.gif',
                fileBottomNavCloseImage : '/DesktopModules/avt.FastShot/js/jquery-lightbox/images/closelabel.gif'
            });
            
            avt.fastshot.$$.fixPng();
        });
    }
}



if (!avt.FastShot.core) { avt.FastShot.core = {}; }

avt.Common = {

    confirm : function(title, text, postBackId, data) {
       avt.fastshot.$(".NavXpMsg").children(":visible").hide("slide", { direction: "down"}, "fast");
        if (avt.fastshot.$$.$('#NavXp_ConfirmDlg').length == 0) {
           avt.fastshot.$('form').append("<div id = 'NavXp_ConfirmDlg' class = 'FastShot_dlg' style ='display: none; padding-top: 20px;'></div>");
           avt.fastshot.$('#NavXp_ConfirmDlg').append("<table border='0' cellspacing='0' cellpadding='0' width = '100%'></table>");
           avt.fastshot.$('#NavXp_ConfirmDlg > table').append("<tr><td class = 'navxp_confirm_text'>"+ text +"</td></tr>");
           avt.fastshot.$('#NavXp_ConfirmDlg > table').append("<tr><td align = 'middle' style = 'padding-left: 70px; padding-bottom: 10px; padding-top: 20px;'></td></tr>");
           avt.fastshot.$('#NavXp_ConfirmDlg').children("table").children("tbody").children("tr:last").children("td:first")
                .append("<a href = 'javascript: void(0)' class = 'navxp_tab' onClick = 'avt.fastshot.$$.$(\"#NavXp_ConfirmDlg\").dialog(\"destroy\").remove()'>Cancel</a>")
                .append("<a href = 'javascript: void(0)' class = 'navxp_tab' onClick = 'avt.fastshot.$$.$(\"#NavXp_ConfirmDlg\").dialog(\"destroy\").remove(); avt.Common.disableInput(); __doPostBack(\""+ postBackId +"\", \"" + data + "\")'>Confirm</a>");
        }

       avt.fastshot.$("#NavXp_ConfirmDlg").css('display', 'block');
       avt.fastshot.$("#NavXp_ConfirmDlg").dialog({ 
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

       avt.fastshot.$("#NavXp_ConfirmDlg").parent().parent().css("opacity", 0).fadeTo('slow', 1);

    },
    
    
    openActivation : function(controlId, triggerCtrl) {
       avt.fastshot.$(controlId).show();
       avt.fastshot.$(controlId).dialog({ 
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
        
       avt.fastshot.$(controlId).parent().parent().show("scale", {}, "normal", function() { 
                avt.Common.disableInput();
                __doPostBack(triggerCtrl, '');
               avt.fastshot.$(controlId).parent().parent().css("z-index", "1003");
            });
    },
    
    closeActivation: function(controlId) {
        //avt.fastshot.$$.$("#"+menuRenderCtrl).children(":first").html("Loading...");
       avt.fastshot.$(".NavXpOverlayDisable").fadeTo('fast', 0.0, function() { 
           avt.fastshot.$("#NavXp_PreventInputDuringAJAX").dialog("destroy").remove(); 
        });
       avt.fastshot.$(controlId).parent().parent().hide("drop", {}, "slow", function() { 
               avt.fastshot.$(controlId).dialog("destroy");//.eq(1).remove();
                //__doPostBack(triggerCtrl, ''); 
            });
    }

}


avt.FastShot.core = {

    
    
    addEditItem : function(triggerCtrl, itemId) {
    
        if (avt.fastshot.$$.$(".ui-dialog:visible").length > 0) {
           avt.fastshot.$(".ui-dialog-content").dialog("close")
            return;
        }

        
        if (avt.fastshot.$$.$(".ui-dialog").length == 0) {

           avt.fastshot.$("#newItem").show();

           avt.fastshot.$("#newItem").css('display', 'block');
           avt.fastshot.$("#newItem").dialog({ 
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
           avt.fastshot.$("#newItem").dialog("open");
        }
        

       avt.fastshot.$("#newItem").parent().parent().hide().show("scale", {}, "fast", function() { 
                avt.Common.disableInput();
                __doPostBack(triggerCtrl, itemId);
            });
    },
    
    // deleteItem : function(triggerCtrl, itemId) {
        // avt.Common.confirm("Confirm Delete", "Are you sure you want to delete this item?", triggerCtrl, itemId);
    // },
    
    onSaveItem : function(objid, bEdit) {

        avt.FastShot.core._objSave = null;
        avt.FastShot.core._thumbUploaded = false;
        avt.FastShot.core._imageUploaded = false;
        
        var bValid = Page_ClientValidate("avtFastShotAddEdit");

        var iDocThumb;// = window.frames["iUploadThumbFrame"].document;
        var iDocImage;// = window.frames["iUploadImageFrame"].document;
        
        for (f = 0; f < window.frames.length; f++) {
            try {
                if (window.frames[f].name == "iUploadThumbFrame") iDocThumb = window.frames[f].document;
                if (window.frames[f].name == "iUploadImageFrame") iDocImage = window.frames[f].document;
            } catch (err) { } // most likely this is a frame we don't have access to
        }
        
        //var iDocThumb = window.frames["iUploadThumbFrame"].document;
        //var iDocImage = window.frames["iUploadImageFrame"].document;

        if (!bEdit && iDocThumb.forms[0].upl.value == "") { 
           avt.fastshot.$("#iUploadThumbFrame").parent().find(".vld").show();
            bValid = false;
        } else {
           avt.fastshot.$("#iUploadThumbFrame").parent().find(".vld").hide();
        }
        
        if (!bEdit && iDocImage.forms[0].upl.value == "") { 
           avt.fastshot.$("#iUploadImageFrame").parent().find(".vld").show();
            bValid = false;
        } else {
           avt.fastshot.$("#iUploadImageFrame").parent().find(".vld").hide();
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
    
        var iDocThumb;// = window.frames["iUploadThumbFrame"].document;
        
        for (f = 0; f < window.frames.length; f++) {
            try {
                if (window.frames[f].name == "iUploadThumbFrame") iDocThumb = window.frames[f].document;
            } catch (err) { } // most likely this is a frame we don't have access to
        }
        
       avt.fastshot.$("#" + hdId).val(iDocThumb.forms[0].hdFilename.value)
        
        avt.FastShot.core._thumbUploaded = true;
        
        if (avt.FastShot.core._imageUploaded === true) {
            var objid = avt.FastShot.core._objSave;
            avt.FastShot.core._objSave = null;
            __doPostBack(objid,'');
        }
    },
    
    imageUploaded : function(hdId) {
    
        if (!avt.FastShot.core._objSave) { return; }
    
        var iDocImage;// = window.frames["iUploadImageFrame"].document;
        
        for (f = 0; f < window.frames.length; f++) {
            try {
                if (window.frames[f].name == "iUploadImageFrame") iDocImage = window.frames[f].document;
            } catch (err) { } // most likely this is a frame we don't have access to
        }
        
       avt.fastshot.$("#" + hdId).val(iDocImage.forms[0].hdFilename.value)
        
        avt.FastShot.core._imageUploaded = true;
        
        if (avt.FastShot.core._thumbUploaded === true) {
            var objid = avt.FastShot.core._objSave;
            avt.FastShot.core._objSave = null;
            __doPostBack(objid,'');
        }
    }
}
