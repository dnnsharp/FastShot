
avt.fs.fb = {
    loadFolders: function(pdirObj) {
    
        var pdir = pdirObj.attr("rel");

        avt.fs.studio.loading(true);
        avt.fs.$.get(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "get_folders",
            pdir: pdir
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error loading folders!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                    avt.fs.fb.folders[pdir] = data;
                    avt.fs.fb.populateFolders(pdirObj);
                }
        }, "json");
    },
    
    populateFolders: function(pdirObj) {
        var _c = pdirObj.parent().find(".subFolders");
        var _rel = pdirObj.attr("rel");
        for (var i in avt.fs.fb.folders[_rel]) {
            var tpl = avt.fs.$("#tplFolder").clone().removeAttr("id");
            tpl.find(".folderLink").text(avt.fs.fb.folders[_rel][i].name).attr("rel", avt.fs.fb.folders[_rel][i].path);
            if (avt.fs.fb.folders[_rel][i].children > 0) {
                tpl.find(".folderExpand").css("display", "inline");
            }
            _c.append(tpl.show());
        }
        
        if (_rel == '\\') { // also laod files
            avt.fs.fb.loadFiles(pdirObj);
        }
    },
    
    popuplateRoot: function() {
        var tpl = avt.fs.$("#tplFolder").clone().removeAttr("id");
        tpl.find(".folderLink").text("Portal Root").attr("rel", "\\");
        tpl.find(".folderCollapse").css("display", "inline");
        tpl.find(".subFolders:first").css("display", "block");
        avt.fs.$("#fileBrowserFolders").append(tpl.show());
    },
    
    expand: function(aObj) {
        var pdirObj = aObj.parents("li:first").find(".folderLink:first");
        if (!avt.fs.fb.folders[pdirObj.attr("rel")]) {
            // get it
            avt.fs.fb.loadFolders(pdirObj);
        }
        
        avt.fs.fb._expand(pdirObj);
    },
    
    _expand: function(pdirObj) {
        pdirObj.parents("li:first").find(".subFolders:first").show();
        pdirObj.parents("li:first").find(".folderCollapse:first").css("display", "inline");
        pdirObj.parents("li:first").find(".folderExpand:first").css("display", "none");
    },
    
    collapse: function(aObj) {
        aObj.parents("li:first").find(".subFolders:first").hide();
        aObj.parent().find(".folderExpand:first").css("display", "inline");
        aObj.hide();
    },
    
    loadFiles: function(pdirObj) {
        var pdir = pdirObj.attr("rel");

        if (!avt.fs.fb.files[pdir]) {
            avt.fs.studio.loading(true);
            avt.fs.$.get(avt.fs.apiUrl, { 
                format: "json",
                mid: avt.fs.mid,
                fn: "get_files",
                pdir: pdir
            }, function(data){
                    avt.fs.studio.loading(false);
                    if (data.error) {
                        avt.fs.$.jGrowl("Error loading files!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                    } else {
                        avt.fs.fb.files[pdir] = data;
                        avt.fs.fb.populateFiles(pdirObj);
                    }
            }, "json");
        } else {
            avt.fs.fb.populateFiles(pdirObj);
        }
    },

    
    populateFiles: function(pdirObj) {
        var _c = avt.fs.$("#fileBrowserFiles:first").empty();
        var _rel = pdirObj.attr("rel");
        
        var tplHead = avt.fs.$("#tplFolderHeader").clone().removeAttr("id");
        tplHead.html(_rel);
        _c.append(tplHead);
        
        if (avt.fs.fb.files[_rel].length == 0) {
            _c.append("<li class = 'noFiles'>No Files</li>");
            return;
        }

        for (var i in avt.fs.fb.files[_rel]) {
            var tpl = avt.fs.$("#tplFile").clone().removeAttr("id");
            tpl.find(".fileLink").text(avt.fs.fb.files[_rel][i].name).attr("title", "<img src = '" + avt.fs.fb.files[_rel][i].absoluteUrl + "' style = 'height: 64px; border: 2px solid rgb(30, 30, 200);' />").attr("href", "portal://" + avt.fs.fb.files[_rel][i].path);
            _c.append(tpl.show());
        }
        
        avt.fs.$(".fileLink").bt({
            width: 1,
              padding: 0,
              spikeLength: 20,
              spikeGirth: 6,
              cornerRadius: 0,
              fill: 'rgba(30, 30, 200, .8)',
              strokeWidth: 0,
              strokeStyle: '#CC0',
              cssStyles: {color: '#FFF', fontWeight: 'bold', zIndex: '99999'},
              offsetParent: avt.fs.$("body"), // TODO: analyze this for sf
              trigger: "hover",
              positions: "right"
            });
    },
    
    deleteFile :function(fileObj) {
        var filename = fileObj.text();
        if (!confirm("Are you sure you want to delete file " + filename + "?"))
            return;
            
        avt.fs.studio.loading(true);
        avt.fs.$.get(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "del_file",
            path: fileObj.attr("rel")
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error deleting file!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                    var cdir = avt.fs.$("#fileBrowserFiles").find(".folderHeader").text();
                    delete avt.fs.fb.files[cdir];
                    avt.fs.fb.loadFiles(avt.fs.$("#fileBrowserFolders").find("[rel="+cdir+"]"));
                    avt.fs.$.jGrowl("File succesfully deleted!", {header: 'Success', life: 5000});
                }
        }, "json");

    },
    
    uploadComplete: function(response) {
        if (response.indexOf("Error") === 0) {
            avt.fs.$.jGrowl(response, {header: 'Error', life: 5000});
        } else {
            delete avt.fs.fb.files[response];
            avt.fs.fb.loadFiles(avt.fs.$("#fileBrowserFolders").find("[rel="+response+"]"));
            avt.fs.$.jGrowl("File succesfully uploaded!", {header: 'Success', life: 5000});
        }
    }
}

avt.fs.AIM = {
 
	frame : function(c) {
 
		var n = 'f' + Math.floor(Math.random() * 99999);
		var d = document.createElement('DIV');
		d.innerHTML = '<iframe style="display:none" src="about:blank" id="'+n+'" name="'+n+'" onload="avt.fs.AIM.loaded(\''+n+'\')"></iframe>';
		document.body.appendChild(d);
 
		var i = document.getElementById(n);
		if (c && typeof(c.onComplete) == 'function') {
			i.onComplete = c.onComplete;
		}
 
		return n;
	},
 
	form : function(f, name) {
		f.setAttribute('target', name);
	},
 
	submit : function(f, c) {
		avt.fs.AIM.form(f, avt.fs.AIM.frame(c));
		if (c && typeof(c.onStart) == 'function') {
			return c.onStart();
		} else {
			return true;
		}
	},
 
	loaded : function(id) {
		var i = document.getElementById(id);
		if (i.contentDocument) {
			var d = i.contentDocument;
		} else if (i.contentWindow) {
			var d = i.contentWindow.document;
		} else {
			var d = window.frames[id].document;
		}
		if (d.location.href == "about:blank") {
			return;
		}
 
		if (typeof(i.onComplete) == 'function') {
			i.onComplete(d.body.innerHTML);
		}
	}
 
}

avt.fs.studio = {

    resizeEditor : function() {
        avt.fs.$("#accordion_controls").parent().css("height", (avt.fs.$("#accordion_controls").parents(".ui-layout-west").innerHeight() - 60) + "px");
        avt.fs.$("#sbmTabs").css("height", (avt.fs.$("#sbmTabs").parents(".pane-center").innerHeight() - 11) + "px");
        avt.fs.$("#accordion_controls").accordion("resize");
        
        avt.fs.$(".main_auto_scroll").width(avt.fs.$("#tabs-form-designer").width() - 5);
        avt.fs.$(".main_auto_scroll").height(avt.fs.$("#sbmTabs").height() - 60);

        avt.fs.$("#itemTrashBin").height(avt.fs.$("#imgContainer").height() - 100);
    },

    init: function() {
        avt.fs.$("#itemTrashBin").sortable({
            revert: true,
            placeholder: "itemPlaceHolder",
            forcePlaceholderSize: true,
            delay: 300,
            connectWith: "#imgContainer",
            receive: function(event, ui) {
                avt.fs.$("#itemTrashBinBox").find(".trashEmpty").hide();
            }
        });
        
        avt.fs.fb.folders = {};
        avt.fs.fb.files = {};
        avt.fs.fb.popuplateRoot();
        avt.fs.fb.loadFolders(avt.fs.$("#fileBrowserFolders").find(".folderLink"));
        

        // load templates
        var _ddt = avt.fs.$("#ddTemplates").empty();
        for (var i in avt.fs.templates) {
            _ddt.append("<option value = '"+ avt.fs.templates[i] +"'>"+ avt.fs.templates[i] +"</option>");
        }
        
        // load settings
        var _cs = avt.fs.$("#pnlGeneralSettings");
        _cs.find("#set_modTitle").text(avt.fs.settings.title + " (id: "+ avt.fs.settings.mid +")");
        try {
            _ddt.val(avt.fs.settings.template);
        } catch (ex) {
            setTimeout(function() { avt.fs.$("#ddTemplates").val(avt.fs.settings.template); }, 10);
        }
        _cs.find("#set_modWidth").val(avt.fs.settings.thumb_w);
        _cs.find("#set_modHeight").val(avt.fs.settings.thumb_h);
    },

    loading: function(bLoading) {
        if (bLoading) {
            avt.fs.$(".load_maker_vis").css("visibility", "visible");
            avt.fs.$(".load_maker_dis").show();
        } else {
            avt.fs.$(".load_maker_vis").css("visibility", "hidden");
            avt.fs.$(".load_maker_dis").hide();
        }
    },

    loadImages: function() {
        avt.fs.studio.loading(true);
        avt.fs.$.get(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "list_items" 
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error loading items!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                    avt.fs.items = data;
                    
                    // check we have at least one element
                    var bEmpty = true;
                    for (var i in avt.fs.items) {
                        bEmpty = false;
                        break;
                    }
                    
                    if (bEmpty === true) {
                        avt.fs.items = {};
                        avt.fs.ordItems = [];
                        return; // nothing to populate
                    }
                    
                    // create ordering array
                    avt.fs.ordItems = [];//new Array(avt.fs.items.length);
                    for (var id in avt.fs.items) {
                        avt.fs.ordItems[avt.fs.items[id].order] = id;
                    }
                    avt.fs.studio.populateItems();
                }
        }, "json");
    },
    
    clearCache: function() {
        avt.fs.$.get(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "clear_cache" 
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error loading items!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                    avt.fs.$.jGrowl("Cache succesfully cleared. Please <a href = 'javascript: window.location.reload(true)'>refresh</a> page to get accurate results.", {header: 'Success', life: 20000});
                }
        }, "json");
    },


    populateItems: function() {
        var _c = avt.fs.$("#imgContainer").empty();
        _c.sortable("destroy");
        var _tpl = avt.fs.$("#tplItemView");
        
        // determine cell width and height by finding max width and max height
        var w = 0;
        var h = 0;
        
        for (var i=0; i < avt.fs.ordItems.length; i++) {
            var tpl = _tpl.clone().removeAttr("id");
            var item = avt.fs.items[avt.fs.ordItems[i]];
            tpl.find(".itemId").text(item.id);
            tpl.find(".itemThumb").attr("src", item.thumb);
            _c.append(tpl.css("display", "block"));
            
            if (item.thumb_w > w) w = item.thumb_w;
            if (item.thumb_h > h) h = item.thumb_h;
        }
        _c.append("<div style = 'clear: both;'></div>");

        // resize grid
        _c.find(".itemViewRoot")
            .width(w)
            .height(h)
            .click(function() { avt.fs.studio.loadProps(avt.fs.$(this))});
        
        _c.sortable({
            revert: true,
            placeholder: "itemPlaceHolder",
            forcePlaceholderSize: true,
            delay: 10,
            connectWith: "#itemTrashBin",
            update: function(event, ui) { // sync ordering
                avt.fs.ordItems = [];
                avt.fs.$("#imgContainer").find(".itemId").each(function() {
                    avt.fs.ordItems.push(parseInt(this.innerHTML));
                    avt.fs.items[avt.fs.ordItems[avt.fs.ordItems.length-1]].order = avt.fs.ordItems.length-1;
                });
                avt.fs.$("#pnlOrderingChanges").show();
                avt.fs.$("#pnlTipOrdering").hide();
            }

        });
        
        
        
        //avt.fs.$("#imgContainer").sortable("option", "connectWith", "#itemTrashBin");
        //avt.fs.$("#itemTrashBin").sortable("option", "connectWith", "#imgContainer");
    },

    loadProps: function(itemRoot) {
        avt.fs.$(".itemViewRoot").removeClass("sel");
        itemRoot.addClass("sel");
        
        var _props = avt.fs.$(".propsRoot:first");
        _props.find(".wizerror").hide();
        var _item = avt.fs.items[itemRoot.find(".itemId").text()];
        _props.find(".itemId").text(_item.id)
        _props.find(".propsItemName").val(_item.title)
        _props.find(".propsItemDesc").val(_item.desc)
        _props.find(".propsItemImage").text(_item.image)
        _props.find(".propsItemThumb").text(_item.thumb)
        _props.show();
        avt.fs.$("#helpTabs").tabs("select", 0);
        
        avt.fs.$(".propsNoItem:first").hide();
        
    },

    newItem: function() {
        avt.fs.$("#sbmTabs").tabs("enable",1);
        avt.fs.$("#sbmTabs").tabs("select",1);
        var _c = avt.fs.$("#newItem_Container");
        _c.find(":input").val("");
        avt.fs.studio.toggleAutoGenerate(true);
        _c.find("#newItem_Title").focus();
    },
    
    hideNewItem: function() {
        avt.fs.$("#sbmTabs").tabs("select",0);
        avt.fs.$("#sbmTabs").tabs("disable",1);
    },

    toggleAutoGenerate: function(bDisable) {
        if (bDisable) {
            avt.fs.$('#newItem_Container').find('#tbThumbUrl').addClass("inpDisabled").attr("disabled", "disabled");
            avt.fs.$('#cbAutoGenerate')[0].checked = true;
        } else {
            avt.fs.$('#newItem_Container').find('#tbThumbUrl').removeClass("inpDisabled").removeAttr("disabled");
            avt.fs.$('#cbAutoGenerate')[0].checked = false;
        }
    },
    
    useAsImage: function(obj) {
        if (avt.fs.$("[href='#tabs-sb-new']").parent().hasClass("ui-state-disabled")) {
            avt.fs.studio.newItem();
        }
        avt.fs.$("#sbmTabs").tabs("select",1);
        var _c = avt.fs.$("#newItem_Container");
        var filePath = avt.fs.$(obj).parents("li").find(".fileLink").attr("href");
        _c.find("#tbImageUrl").val(filePath);
        
        // check if title is empty, and put the filename in there
        if (avt.fs.$.trim(_c.find("#newItem_Title").val()) == "") {
        
            if (filePath.indexOf('/') > -1)
                filePath = filePath.substring(filePath.lastIndexOf('/')+1,filePath.length);
            else if (filePath.indexOf('\\') > -1)
                filePath = filePath.substring(filePath.lastIndexOf('\\')+1,filePath.length);

            _c.find("#newItem_Title").val(filePath);
        }
    },
    
    useAsThumb: function(obj) {
        if (avt.fs.$("[href='#tabs-sb-new']").parent().hasClass("ui-state-disabled")) {
            avt.fs.studio.newItem();
        }
        avt.fs.$("#sbmTabs").tabs("select",1);
        var _c = avt.fs.$("#newItem_Container");
        _c.find("#tbThumbUrl").val(avt.fs.$(obj).parents("li").find(".fileLink").attr("href"));
        _c.find(".wizerror").hide();
        avt.fs.studio.toggleAutoGenerate(false);
    },


    saveItem: function() {

        var _c = avt.fs.$("#newItem_Container");

        // validate data
        _c.find(".wizerror").hide();
        
        var title = avt.fs.$.trim(_c.find("#newItem_Title").val());
        if (title.length == 0) {
            _c.find("#newItem_TitleErr").text("Title is required!").show();
            return;
        }
        
        var imageUrl = avt.fs.$.trim(_c.find("#tbImageUrl").val());
        if (imageUrl.length == 0) {
            _c.find("#newItem_ImageUrlErr").text("Image Url is required!").show();
            return;
        }
        
        var thumbUrl = avt.fs.$.trim(_c.find("#tbThumbUrl").val());
        if (_c.find("#cbAutoGenerate")[0].checked === false && imageUrl.length == 0) {
            _c.find("#newItem_ThumbUrlErr").text("Image Thumb is required when Auto Generate is not set!").show();
            return;
        }
        
        // ready to save
        avt.fs.studio.loading(true);
        avt.fs.$.post(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "add_item",
            title: title,
            desc: avt.fs.$.trim(_c.find("#newItem_Desc").val()),
            image: imageUrl,
            thumb: thumbUrl
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error adding item!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                    // append item
                    avt.fs.items[data.id] = data;
                    avt.fs.items[data.id].order = avt.fs.ordItems.length;
                    avt.fs.ordItems.push(data.id);
                    
                    // and refresh
                    avt.fs.studio.populateItems();
                    
                    // focus new item
                    avt.fs.$("#imgContainer").find(".itemId:contains("+ data.id +")").parents("li:first").click();
                    
                    // switch to list
                    avt.fs.$("#sbmTabs").tabs("select",0);
                    avt.fs.$("#sbmTabs").tabs("disable",1);
                    
                    //avt.fs.$("#_fsibottom").focus();
                    avt.fs.$("#imgContainer").attr({ scrollTop: avt.fs.$("#imgContainer").attr("scrollHeight") });
                    //avt.fs.$("#imgContainer").scrollTo(avt.fs.$("#imgContainer").find("li:last"));
                    
                    avt.fs.$.jGrowl("New item succesfully saved.", {header: 'Success', life: 5000});
                }
        }, "json");
    },


    updateItem: function() {
        var _c = avt.fs.$("#tabs-help-props");
        _c.find(".wizerror").hide();
        
        var id = _c.find(".itemId").text();
        var title = avt.fs.$.trim(_c.find(".propsItemName").val());
        var desc = avt.fs.$.trim(_c.find(".propsItemDesc").val());
        if (title.length == 0) {
            _c.find("#errEditTitle").text("Title is required!").show();
            return;
        }
        
        // ready to save
        avt.fs.studio.loading(true);
        avt.fs.$.post(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "edit_item",
            id: id,
            title: title,
            desc: desc
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error updating item!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                    // update item
                    avt.fs.items[id].title = title;
                    avt.fs.items[id].desc = desc;
                    
                    // TODO:
                    // update interface?

                    avt.fs.$.jGrowl("Item succesfully updated.", {header: 'Success', life: 5000});
                }
        }, "json");
    },
    
    
    saveOrdering: function() {
        
        var ids = [];
        for (var i in avt.fs.ordItems) {
            ids.push(avt.fs.ordItems[i]);
        }

        // start request
        avt.fs.studio.loading(true);
        avt.fs.$.post(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "order_items",
            ids: ids.join(",")
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error saving items ordering!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                    avt.fs.$.jGrowl("New ordering succesfully saved.", {header: 'Success', life: 5000});
                    avt.fs.$("#pnlOrderingChanges").hide();
                }
        }, "json");
    
    },


    emptyRecycleBin: function() {
        var _c = avt.fs.$("#itemTrashBin");
        var ids = [];
        _c.find(".itemId").each(function() {
            ids.push(this.innerHTML);
        });
        
        if (ids.length == 0) {
            avt.fs.$.jGrowl("There are no items in recycle bin...", {header: 'Error', life: 5000});
            return;
        }
        
        // start request
        avt.fs.studio.loading(true);
        avt.fs.$.post(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "del_items",
            ids: ids.join(",")
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error deleting items!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                
                    // remove items
                    for (var i in ids) {
                        delete avt.fs.items[ids[i]];
                    }
                    avt.fs.$("#itemTrashBin").empty();
                    
                    // and refresh
                    //avt.fs.studio.populateItems();
                    avt.fs.$.jGrowl("Recycle bin succesfully cleared.", {header: 'Success', life: 5000});
                }
        }, "json");
    },

    
    saveSettings: function() {
        var _cs = avt.fs.$("#pnlGeneralSettings");
        var template = _cs.find("#ddTemplates").val();
        var thumb_w = parseInt(_cs.find("#set_modWidth").val());
        var thumb_h = parseInt(_cs.find("#set_modHeight").val());
        
        if (isNaN(thumb_w) || isNaN(thumb_h)) {
            avt.fs.$.jGrowl("Error saving settings!<br />Both thumbnail width and height must be integers. Set one to 0 to scale proportionally.", {header: 'Error', life: 5000});
            return;
        }
        
        // start request
        avt.fs.studio.loading(true);
        avt.fs.$.post(avt.fs.apiUrl, { 
            format: "json",
            mid: avt.fs.mid,
            fn: "update_settings",
            template: template,
            thumb_w: thumb_w,
            thumb_h: thumb_h
        }, function(data){
                avt.fs.studio.loading(false);
                if (data.error) {
                    avt.fs.$.jGrowl("Error saving settings!<br />Server response: " + data.error, {header: 'Error', life: 5000});
                } else {
                
                    // refresh list
                    avt.fs.studio.loadImages();
                    
                    avt.fs.$.jGrowl("Settings succesfully updated.", {header: 'Success', life: 5000});
                }
        }, "json");
    }

}
