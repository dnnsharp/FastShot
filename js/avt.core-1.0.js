
if (!avt) { var avt = {}; }

avt.core_1_0 = {
    
    $ : avt_jQuery_1_3_2,
    
    fixPng : function () {
        avt.core_1_0.$(document).ready(function() {
            var IE6 = (navigator.userAgent.toLowerCase().indexOf('msie 6') != -1) && (navigator.userAgent.toLowerCase().indexOf('msie 7') == -1)
            if (IE6) { // fix PNG transparency
                var arVersion = navigator.appVersion.split("MSIE")
                var version = parseFloat(arVersion[1])
                if ((version >= 5.5) && (document.body.filters)) {
                    for(var i=0; i<document.images.length; i++) {
                        var img = document.images[i]
                        var imgName = img.src.toUpperCase()
                        if (avt.core_1_0.$(img).hasClass("pngFix") && imgName.substring(imgName.length-3, imgName.length) == "PNG") {
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
    
    disableInput : function(ctrl, bHideIframe) {

        if (avt.core_1_0.$(ctrl).next(".ui-widget-overlay").next(".avtDisabled").length > 0) {
            return; // control is already disabled
        }
        
        avt.core_1_0.$(ctrl).after("<div class = 'avtDisabled' style ='display: none;'><div class = 'navxp_loader'><img src = '/DesktopModules/avt.NavXp/res/loader.gif' /></div></div>");
        var dsbl = avt.core_1_0.$(ctrl).next(".avtDisabled");

        dsbl.dialog({
            dialogClass : 'avtDisabled',
            modal: true,
            resizable: false,
            width: avt.core_1_0.$(ctrl).width(),
            height: avt.core_1_0.$(ctrl).height(),
            title: "&nbsp;",
            closeOnEscape: false,
            shadow: false
        });
        
        bHideIframe && avt.core_1_0.$(ctrl).find("iframe").css("visibility", "hidden");

        dsbl.parent().css("z-index", parseInt(avt.core_1_0.$(ctrl).css("z-index")) + 100);
        dsbl.parent().css("top", avt.core_1_0.$(ctrl).css("top"))
        dsbl.parent().css("left", avt.core_1_0.$(ctrl).css("left"));
        dsbl.parent().css("width", avt.core_1_0.$(ctrl).width());
        dsbl.parent().css("height", avt.core_1_0.$(ctrl).height());
        dsbl.css("padding-top", avt.core_1_0.$(ctrl).height() / 2 - 20);
        
        dsbl.css('display', 'block');
        dsbl.parent().css("opacity", 0).stop().queue(function() {
            avt.core_1_0.$(this).fadeTo('slow', 0.4, function() {
            });
            avt.core_1_0.$(this).dequeue();
        });
    },
    
   
    enableInput : function(ctrl) {

        ctrl = avt.core_1_0.$(ctrl);
        if (!ctrl.hasClass(".ui-dialog")) {
            ctrl = ctrl.parents(".ui-dialog");
        }

        if (ctrl.next(".ui-widget-overlay").next(".avtDisabled").find(".ui-dialog-content").length == 0) {
            return; // control is not disabled
        }
        
        ctrl.find("iframe").css("visibility", "visible");

        ctrl.next(".ui-widget-overlay").next(".avtDisabled").fadeTo('slow', 0.0, function() {
            var ctrlDis = avt.core_1_0.$(this).find(".ui-dialog-content");
            ctrlDis.dialog("destroy");
            if (ctrlDis.parents(".ui-dialog").length == 0) {
                ctrlDis.remove();
            }
        });
    },
    
    showDlg : function(src, options) {
    
        var dlgOpts = {
            title : "",
            cssClass : "",
            width: 500,
            height: 400,
            buttons: [],
            show_save: true,
            show_cancel: true,
            save_text: "Save",
            cancel_text: "Cancel",
            refresh_onsave: false,
            postbackid_onsave : null,
            rightText: null
        };
        
        avt.core_1_0.$.extend(dlgOpts, options);
        var butHtml = "";
        if (dlgOpts.show_save) {
            butHtml += "<a href = 'javascript: void(0)' class = 'sysBtn' onclick = 'avt.core_1_0._onDlgSave(this);' rel = 'save'>"+dlgOpts.save_text+"</a>";
        }
        
        if (dlgOpts.show_cancel) {
            butHtml += "<a href = 'javascript: void(0)' class = 'sysBtn' onclick = 'avt.core_1_0._onDlgCancel(this);' rel = 'cancel'>"+dlgOpts.cancel_text+"</a>";
        }

        for (var but in dlgOpts.buttons) {
            butHtml += "<a href = '"+ dlgOpts.buttons[but].src +"' class = 'sysBtn' target = '"+ dlgOpts.buttons[but].target +"'>"+ dlgOpts.buttons[but].title +"</a>";
        }
        
        if (dlgOpts.rightText) {
            dlgOpts.title += "<div style = 'float: right; margin-top: -10px; margin-right: 10px; color: #DEDEDE; font-size: 11px; font-weight: bold;'>"+dlgOpts.rightText+"</div><div style = 'clear: both;'></div>";
        }
        
        
        avt.core_1_0.$("body").append("<div class = 'dlg "+ dlgOpts.cssClass +"' style = 'text-align: center; width: auto; height: auto;'><iframe src = '' onload = 'avt.core_1_0.enableInput(this);' frameborder = '0' style = 'width: 100%; height: "+ (dlgOpts.height - 65) +"px; border-width: 0px; overflow-x: hidden;'></iframe><div class = 'sysBtnC'>" + butHtml + "</div></div>");
        
        var ctrl = avt.core_1_0.$("body").find(".dlg:last");

        ctrl.dialog({ 
            modal: true, 
            dialogClass : dlgOpts.cssClass,
            resizable: false,
            draggable: true,
            width: dlgOpts.width,
            height: dlgOpts.height,
            title: dlgOpts.title,
            closeOnEscape: false,
            shadow: false,
            noShow: true
        });

        ctrl.parent().hide();
        
        ctrl.parent().next(".ui-widget-overlay").css("background-color", "#000000").css("opacity", 0).fadeTo('slow', 0.7);
        ctrl.parent().show("drop", {direction: 'right'}, "normal", function() { 
            avt.core_1_0.disableInput(ctrl.parent(), true);
            ctrl.show();
            ctrl.find("iframe").get(0).contentWindow.location = src;
            ctrl.find("iframe").get(0).avtRefreshMain = dlgOpts.refresh_onsave;
            ctrl.find("iframe").get(0).avtDlgOpts = dlgOpts;
        });
    },
    
    frameLoading : function (parentWnd, frameWnd, bSkinValidate) {

        if (!bSkinValidate && typeof(Page_ClientValidate) == 'function' && !Page_ClientValidate()) {
            this.frameLoaded(parentWnd, frameWnd);
            return;
        }

        for (var f = 0; f < parentWnd.frames.length; f++) {
             if (parentWnd.frames[f].window == frameWnd) {
                avt.core_1_0.disableInput(avt.core_1_0.$(parentWnd.frames[f].frameElement).parents(".avtNXPDlg:last"));
                break;
            }
        }
    },
    
    frameLoaded : function (parentWnd, frameWnd) {
    
        for (var f = 0; f < parentWnd.frames.length; f++) {
             if (parentWnd.frames[f].window == frameWnd) {
                avt.core_1_0.enableInput(avt.core_1_0.$(parentWnd.frames[f].frameElement).parents(".avtNXPDlg:last"));
                break;
            }
        }
    },
    
    _onDlgCancel : function(ctrl) {

        var iFrame = avt.core_1_0.$(ctrl).parents(".dlg").find("iframe").get(0);
        if (iFrame.avtRefreshMain || iFrame.contentWindow.avtRefreshMain) {
            location.reload(true);
        }

        avt.core_1_0.closeDlg(ctrl);
    },
    
    _onDlgSave : function(ctrl) {

        var iFrame = avt.core_1_0.$(ctrl).parents(".dlg").find("iframe").get(0);
    
        if (typeof(iFrame.contentWindow.Page_ClientValidate) == 'function' && !iFrame.contentWindow.Page_ClientValidate()) {
            return;
        }
        
        avt.core_1_0.$(ctrl).parents(".dlg").find("iframe").bind("load", function() {
            avt.core_1_0.closeDlg(ctrl);
            if (iFrame.avtRefreshMain || iFrame.contentWindow.avtRefreshMain) {
                location.reload(true);
            }
            
            if (iFrame.avtDlgOpts.postbackid_onsave) {
                __doPostBack(iFrame.avtDlgOpts.postbackid_onsave, '');
            }
            
            avt.core_1_0.$(this).unbind("load");
        });

        avt.core_1_0.disableInput(avt.core_1_0.$(ctrl).parents(".ui-dialog"));
        if (avt.core_1_0.$(iFrame.contentWindow.document.forms[0]).find("[rel=save]").size() > 0) {
            iFrame.contentWindow.__doPostBack(avt.core_1_0.$(iFrame.contentWindow.document.forms[0]).find("[rel=save]").attr("id"), "");
        } else {
            avt.core_1_0.closeDlg(ctrl);
        }
    },
    
    closeDlg : function(ctrl) {
        avt.core_1_0.enableInput(ctrl);
        
        var iFrame = avt.core_1_0.$(ctrl).parents(".dlg").find("iframe").get(0);
        
        avt.core_1_0.$(ctrl).parents(".ui-dialog").next(".ui-widget-overlay").fadeTo('normal', 0);
        avt.core_1_0.$(ctrl).parents(".ui-dialog").hide("drop", {}, "slow", function() { 
                if (iFrame.avtRefreshMain || iFrame.contentWindow.avtRefreshMain) {
                    avt.core_1_0.$(ctrl).parents(".ui-dialog-content").dialog("destroy").remove();
                    location.reload(true);
                    return;
                }
                avt.core_1_0.$(ctrl).parents(".ui-dialog-content").dialog("destroy").remove();
                //__doPostBack(triggerCtrl, ''); 
            });
    },
    
    confirm : function(title, text, postBackId, data, cssClass) {

        if (avt.core_1_0.$('#avt_ConfirmDlg').length == 0) {
            avt.core_1_0.$('body').append("<div id = 'avt_ConfirmDlg' class = '"+ cssClass +"' style ='display: none; padding-top: 20px;'></div>");
            avt.core_1_0.$('#avt_ConfirmDlg').append("<table border='0' cellspacing='0' cellpadding='0' width = '100%'></table>");
            avt.core_1_0.$('#avt_ConfirmDlg > table').append("<tr><td class = 'navxp_confirm_text'>"+ text +"</td></tr>");
            avt.core_1_0.$('#avt_ConfirmDlg > table').append("<tr><td align = 'middle' style = 'padding-left: 70px; padding-bottom: 10px; padding-top: 20px;'></td></tr>");
            avt.core_1_0.$('#avt_ConfirmDlg').children("table").children("tbody").children("tr:last").children("td:first")
                .append("<a href = 'javascript: void(0)' class = 'navxp_tab' onClick = 'avt.core_1_0.$(\"#avt_ConfirmDlg\").parent().hide(\"drop\", {}, \"normal\", function() { avt.core_1_0.$(\"#avt_ConfirmDlg\").dialog(\"destroy\").remove()});'>Cancel</a>")
                .append("<a href = 'javascript: void(0)' class = 'navxp_tab' onClick = 'avt.core_1_0.$(\"#avt_ConfirmDlg\").parent().hide(\"drop\", {}, \"normal\", function() { avt.core_1_0.$(\"#avt_ConfirmDlg\").dialog(\"destroy\").remove(); __doPostBack(\""+ postBackId +"\", \"" + data + "\")});'>Confirm</a>");
        }

        avt.core_1_0.$("#avt_ConfirmDlg").css('display', 'block');
        avt.core_1_0.$("#avt_ConfirmDlg").dialog({ 
            modal: true, 
            dialogClass : cssClass,
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

        avt.core_1_0.$("#avt_ConfirmDlg").parent().css("opacity", 0).fadeTo('slow', 1);

    }
    
}