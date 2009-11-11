
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
                    
                    // create ordering array
                    avt.fs.ordItems = new Array(avt.fs.items.length);
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
        _c.find("#tbImageUrl").val(avt.fs.$(obj).parents("li").find(".fileLink").attr("href"));
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
        }
        
        var imageUrl = avt.fs.$.trim(_c.find("#tbImageUrl").val());
        if (imageUrl.length == 0) {
            _c.find("#newItem_ImageUrlErr").text("Image Url is required!").show();
        }
        
        var thumbUrl = avt.fs.$.trim(_c.find("#tbThumbUrl").val());
        if (_c.find("#cbAutoGenerate")[0].checked === false && imageUrl.length == 0) {
            _c.find("#newItem_ThumbUrlErr").text("Image Thumb is required when Auto Generate is not set!").show();
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
    },

    
    reindex: function() {
        avt.fs.studio.loading(true);
        avt.fs.$.get(avt.fs.pageUrl, { fn: "reindex" },
            function(data){
                avt.fs.$.jGrowl("Search Index succesfully rebuilt!", {header: 'Success', life: 5000});
                avt.fs.studio.loading(false);
        }, "text");
    },
    
    
    getRules: function() {
        avt.fs.studio.loading(true);
        avt.fs.$.get(avt.fs.pageUrl, { fn: "get_rules" },
            function(data){
                avt.fs.targets = data;
                // populate screen
                avt.fs.studio.loading(false);
                avt.fs.studio.populateSearchTargets();
        }, "json");
    },
    
    populateSearchTargets: function() {

        var dd = avt.fs.$("#list_search_targets");
        dd.empty();
        for (var id in avt.fs.targets) {
            dd.append("<li><a href = 'javascript: void(0);' onclick = 'avt.fs.wizt.editSearchTarget("+ id +");'>"+ avt.fs.targets[id].title +"</a> <a href = 'javascript: avt.fs.studio.deleteRule("+ avt.fs.targets[id].id +")' onclick = 'return confirm(\"Are you sure you want to delete custome rule "+ avt.fs.targets[id].title +"?\");'><img src = '"+ avt.fs.modRoot +"res/delete.gif' border = '0' /></a></li>");
        }
    },
    
    populateSearchInstances: function() {
        var dd = avt.fs.$("#list_inst_cportal");
        var ddTest = avt.fs.$("#ddTestInst");
        ddTest.append("<option value = '-1'>-- All content on current portal --</option>");
        for (var i in avt.fs.inst.cPortal) {
            dd.append("<li><a href = 'javascript: void(0);' onclick = 'avt.fs.studio.editSearchInst(\""+ avt.fs.inst.cPortal[i].inst +"\");'>"+ avt.fs.inst.cPortal[i].caption +"</a></li>");
            ddTest.append("<option value = '"+ avt.fs.inst.cPortal[i].inst +"'>"+ avt.fs.inst.cPortal[i].caption +"</option>");
        }
        
        
        if (avt.fs.isSuperUser) {

            // init all portals listing
            var _ddAll = avt.fs.$("#list_inst_all");
            for (var instId in avt.fs.inst) {
                if (instId == 'cPortal' || instId == 'byPortal') continue;
                _ddAll.append("<li><a href = 'javascript: void(0);' onclick = 'avt.fs.studio.editSearchInst(\""+ instId +"\");'>"+ avt.fs.inst[instId].caption +"</a></li>");
            }
            
            // init filter by portal
            var _ddPortals = avt.fs.$("#ddFilterByPortal");
            for (var pId in avt.fs.inst.byPortal) {
                _ddPortals.append("<option value = '"+ pId +"'>"+ avt.fs.inst.byPortal[pId].caption +"</option>");
            }
            avt.fs.studio.populateByPortal(_ddPortals.val());
            
        } else {
            avt.fs.$("#noAccessAll, #noAccessFilter").show();
            avt.fs.$("#instFilterByPortal, #instAllPortals").hide();
        }
    },
    
    populateByPortal: function(pId) {
        var _dd = avt.fs.$("#list_inst_byportal").empty();
        for (var i in avt.fs.inst.byPortal[pId].data) {
            _dd.append("<li><a href = 'javascript: void(0);' onclick = 'avt.fs.studio.editSearchInst(\""+ avt.fs.inst.byPortal[pId].data[i].inst +"\");'>"+ avt.fs.inst.byPortal[pId].data[i].caption +"</a></li>");
        }
    },
    
    
    editSearchInst: function(inst) {
        // check if instance fully loaded
        if (!avt.fs.inst[inst].targets) {
            avt.fs.studio.loading(true);
            avt.fs.$.get(avt.fs.pageUrl, { fn: "get_inst_targets_json", data: inst},
                function(data){
                    avt.fs.inst[inst].targets = data;
                    avt.fs.studio.loading(false);
                    avt.fs.studio.searchInst_loadData(inst);
            }, "json");
        } else {
            avt.fs.studio.searchInst_loadData(inst);
        }
    },
    
    searchInst_loadData: function(inst) {
        avt.fs.$(".no_instance_sel").hide();
        
        var _s = avt.fs.$(".instance_summary");
        _s.find(".inst_name").text(avt.fs.inst[inst].caption);
        
        // searchin
        var _sin = _s.find(".inst_searchin");
        _sin.empty();
        avt.fs.studio.searchInst_showData(_sin, avt.fs.inst[inst].targets);
        
        if (avt.fs.inst[inst].pagesize <= 0)
            avt.fs.inst[inst].pagesize = 10;
        
        _s.find(".inst_respage").text(avt.fs.inst[inst].resTabName);
        _s.find(".inst_pagesize").text(avt.fs.inst[inst].pagesize);
        _s.find(".inst_textempty").text(avt.fs.inst[inst].empty);
        _s.find(".inst_getparam").text(avt.fs.inst[inst].getparam);
        
        _s.show();

        avt.fs.$("#sbmTabs").tabs('enable', 1);
        avt.fs.$("#sbmTabs").tabs('enable', 2);
        avt.fs.$("#sbmTabs").tabs('select', 0);
        
        avt.fs.current = inst;
    },

    
    searchInst_showData: function(list, data) {
        for (var i in data) {
            var _li = avt.fs.$("<li>"+ data[i].caption +"</li>");
            if (data[i].children && data[i].children.length > 0) {
                var _ul = avt.fs.$("<ul style = 'margin: 0 0 0 20px; padding: 0px;'></ul>");
                avt.fs.studio.searchInst_showData(_ul, data[i].children);
                _li.append(_ul);
            }
            list.append(_li);
        }
    },
    
    
    // Instance Search Target
    // -------------------------------------------------------------------------------------------------------
    
    loadInputSettings: function() {
    
        if (avt.fs.studio.treeLoadedFor != avt.fs.current) {
            // first, check we have portals json
            if (!avt.fs.portals) {
                avt.fs.studio.loading(true);
                
                // load portals json for the trees
                avt.fs.$.get(avt.fs.pageUrl, { fn: "get_portal_items_json"},
                function(data){
                    avt.fs.portals = data;
                    avt.fs.studio.loading(false);
                    
                    // populate dropdown
                    var _ddPortals = avt.fs.$("#si_ddPortal");
                    for (var pid in avt.fs.portals) {
                        _ddPortals.append("<option value = '"+ pid +"'>"+ avt.fs.portals[pid].name +"</option>");
                    }
                    
                    avt.fs.studio.initSearchTargetTrees();
                    
                    // load selection from current instance
                    avt.fs.studio.loadSelItemsIntoTrees();
                }, "json");
            } else {
                // load selection from current instance
                avt.fs.studio.loadSelItemsIntoTrees();
            }
        }
    },
    
    resetSearchTargetTrees: function() {
        delete avt.fs.portals;
        delete avt.fs.studio.treeLoadedFor;
        avt.fs.$("#srcSiTreeContainer").empty();
        avt.fs.$("#si_ddPortal").empty();
    },
    
    initSearchTargetTrees: function() {

        var _treeContainer = avt.fs.$("#srcSiTreeContainer");
        for (var pid in avt.fs.portals) {
            var _tree = avt.fs.$("<div class = 'src_target_tree' id = 'srcSiTree"+ pid +"'></div>");
            _treeContainer.append(_tree);
            
            _tree.tree({
                data: {
                    type: "json",
                    asynch: "false",
                    json: avt.fs.portals[pid].tree
                },
                ui: {
                    animation: 300,
                    context: false,
                    rtl: false,
                    dots: true,
                    theme_path: avt.fs.modRoot + "js/jsTree/source/themes/",
                    theme_name: "checkbox"
                },

                rules: {
                    clickable: "all"
                },

                callback: {

                    onrgtclk: function(NODE, TREE_OBJ, event) {
                        //event.preventDefault(); event.stopPropagation(); return false;
                    },

                    ondblclk: function(NODE, TREE_OBJ) {
                    },
                    
                    beforeclose: function(NODE, TREE_OBJ) {
                        // freeze checkbox changes
                        TREE_OBJ.freezeCb = true;
                    },
                    
                    onclose: function(NODE, TREE_OBJ) {
                        TREE_OBJ.freezeCb = false;
                    },
                    
                    beforeopen : function(NODE, TREE_OBJ) {
    //                    var _this = avt.fs.$(NODE).is("a") ? avt.fs.$(NODE).is("a") : avt.fs.$(NODE).parent();
    //                    if (_this.hasClass("checked")) {
    //                        _this.find("a").removeClass("unchecked").addClass("checked");
    //                    }
                    },

                    onchange: function(NODE, TREE_OBJ) {
                        if (TREE_OBJ.freezeCb) {
                            return;
                        }
                        
                        var $this = avt.fs.$(NODE).is("li") ? avt.fs.$(NODE) : avt.fs.$(NODE).parent();
                        if ($this.children("a.unchecked").size() == 0) {
                            TREE_OBJ.container.find("a").not(".checked").not(".unchecked").not(".undetermined").addClass("unchecked");
                        }
                        //$this.children("a").removeClass("clicked");
                        var state;
                        if ($this.children("a").hasClass("checked")) {
                            $this.find("li").andSelf().children("a").removeClass("checked").removeClass("undetermined").addClass("unchecked");
                            state = 0;
                        }
                        else {
                            $this.find("li").andSelf().children("a").removeClass("unchecked").removeClass("undetermined").addClass("checked");
                            state = 1;
                        }
                        $this.parents("li").each(function() {
                            if (state == 1) {
                                if (avt.fs.$(this).find("a.unchecked, a.undetermined").size() - 1 > 0) {
                                    avt.fs.$(this).parents("li").andSelf().children("a").removeClass("unchecked").removeClass("checked").addClass("undetermined");
                                    return false;
                                }
                                else avt.fs.$(this).children("a").removeClass("unchecked").removeClass("undetermined").addClass("checked");
                            }
                            else {
                                if (avt.fs.$(this).find("a.checked, a.undetermined").size() - 1 > 0) {
                                    avt.fs.$(this).parents("li").andSelf().children("a").removeClass("unchecked").removeClass("checked").addClass("undetermined");
                                    return false;
                                }
                                else avt.fs.$(this).children("a").removeClass("checked").removeClass("undetermined").addClass("unchecked");
                            }
                        });
                    }
                }
            });
        }
        // show first portal
        _treeContainer.find("div:first").show();
    },
    
    loadSelItemsIntoTrees: function() {
        var _st = avt.fs.$("#st_Container");
        var _treeContainer = avt.fs.$("#srcSiTreeContainer");

        // reset to first portal in the dropdown
        avt.fs.$("#si_ddPortal")[0].selectedIndex = 0;
        _treeContainer.children("div").hide();
        _treeContainer.children("div:first").show();
        
        // reset selected items
        _treeContainer.find("a").removeClass("checked undetermined clicked");
        _treeContainer.find("li").removeClass("open").addClass("closed");
        _treeContainer.find("[rel='root']").removeClass("closed").addClass("open");
        
        // select instance items
        _st.find(".wizerror").hide();
        avt.fs.studio._loadSelItemsIntoTrees(avt.fs.inst[avt.fs.current].targets, _treeContainer);

        avt.fs.studio.treeLoadedFor = avt.fs.current;
    },
    
    _loadSelItemsIntoTrees: function(data, tc) {
        for (var i in data) {
            
            if (data[i].children && data[i].children.length > 0 && data[i].children && data[i].children[0].id != 'all') {
                tc.find("#" + data[i].id + ">a").addClass("undetermined");
                avt.fs.studio._loadSelItemsIntoTrees(data[i].children, tc);
            } else {
                tc.find("#" + data[i].id + " a").addClass("checked");
                if (data[i].id.indexOf("asb_mod") >= 0) { // also set Tab Modules to undetermined
                    tc.find("#" + data[i].id + " a").parents("li[rel=mods]").children("a:first").addClass("undetermined");
                }
            }
        }
    },
    
    switchSiPortal: function(pid) {
        var _treeContainer = avt.fs.$("#srcSiTreeContainer");
        _treeContainer.children("div").hide();
        _treeContainer.find("#srcSiTree"+ pid).show();
    },
    
    
    
    savePortalTabs: function() {

        var _treeContainer = avt.fs.$("#srcSiTreeContainer");
        var _ddPortals = avt.fs.$("#si_ddPortal");
        var postParams = {
            fn: "save_search_targets",
            inst: avt.fs.current
        }
        
        avt.fs.studio.loading(true);
        
        var i = 0;
        _treeContainer.children("div").each(function() {
            if (avt.fs.$(this).find("#asb_custom").length > 0) {
                postParams["cstm"] = JSON.stringify(avt.fs.$$.tree_component.inst[this.getAttribute("id")].getJSON(null, null, ["class"]));
            } else {
                postParams["portal" + _ddPortals.find("option").eq(i++).attr("value")] = JSON.stringify(avt.fs.$$.tree_component.inst[this.getAttribute("id")].getJSON(null, null, ["class"]));
            }
        });
        
        
        // now, post this to the server
        avt.fs.$.post(avt.fs.pageUrl, postParams,
            function(data){
                var _st = avt.fs.$("#st_Container");
                if (data == "success") {
                
                    // reload targets so we're in sync
                    avt.fs.$.get(avt.fs.pageUrl, { fn: "get_inst_targets_json", data: avt.fs.current},
                        function(data){
                            avt.fs.inst[avt.fs.current].targets = data;
                            avt.fs.studio.loading(false);
                            avt.fs.$.jGrowl("Search Target succesfully saved!", {header: 'Success', life: 5000});
                            avt.fs.studio.searchInst_loadData(avt.fs.current);
                    }, "json");
                    
                    
                    // propagate changes
                    //avt.fs.inst[avt.fs.current].resTabId = _so.find("#so_ResultsPageTabId").val().length > 0 ? _so.find("#so_ResultsPageTabId").val() : _so.find("#so_ResultsPage").val();
                    //avt.fs.inst[avt.fs.current].pagesize = _so.find("#so_PageSize").val();
                    //avt.fs.inst[avt.fs.current].empty = _so.find("#so_emptyBox").val();
                    //avt.fs.inst[avt.fs.current].getparam = _so.find("#so_getParam").val();
                    
                    // TODO: sync with local copeis

                    _st.find(".wizerror").hide();

                } else {
                    avt.fs.studio.loading(false);
                    _st.find(".wizerror").text(data).show();
                }
                
            }, "text");
    },
    
    testSearch: function() {
        var txt = avt.fs.$("#testSearchInput").val();
        var inst = avt.fs.$("#ddTestInst").val();
        if (txt.length == 0) {
             avt.fs.$("#testSearchResults").html("Empty input...");
             return;
        }
        avt.fs.$("#testSearchResults").html("Searching... please wait...");
        // get response from server via AJAX
        avt.fs.$.post(avt.fs.pageUrl, { fn: "test_search", data: txt, inst: inst },
            function(data){
                avt.fs.$("#testSearchResults").html(data);
        }, "text");

    },
    
    // Instance UI Settings
    // -------------------------------------------------------------------------------------------------------
    
    loadOutputSettings: function() {
        //check if we have results pages loaded
        if (!avt.fs.resultPages) {
            // load them via ajax
            avt.fs.studio.loading(true);
            avt.fs.$.get(avt.fs.pageUrl, { fn: "get_result_pages_json" },
                function(data){
                    avt.fs.resultPages = data;

                    // populate dropdown
                    var _ddResPages = avt.fs.$("#so_ResultsPage");
                    for (var i in avt.fs.resultPages) {
                        _ddResPages.append("<option value = '"+ avt.fs.resultPages[i].id +"'>"+ avt.fs.resultPages[i].name +"</option>");
                    }
                    
                    avt.fs.studio.loading(false);
                    avt.fs.studio._loadOutputSettings();
            }, "json");
        
        } else {
            avt.fs.studio._loadOutputSettings();
        }
    },
    
    _loadOutputSettings: function() {
        if (avt.fs.studio.soLoadedFor != avt.fs.current) {
            var _so = avt.fs.$("#so_Container");
            if (_so.find("#so_ResultsPage").find("[value="+ avt.fs.inst[avt.fs.current].resTabId +"]").length > 0) {
                _so.find("#so_ResultsPage").val(avt.fs.inst[avt.fs.current].resTabId);
                _so.find("#so_ResultsPageTabId").val("");
            } else{
                _so.find("#so_ResultsPage")[0].selectedIndex = 0;
                if (avt.fs.inst[avt.fs.current].resTabId > 0)
                    _so.find("#so_ResultsPageTabId").val(avt.fs.inst[avt.fs.current].resTabId);
            }

            _so.find("#so_PageSize").val(avt.fs.inst[avt.fs.current].pagesize);
            _so.find("#so_emptyBox").val(avt.fs.inst[avt.fs.current].empty);
            _so.find("#so_getParam").val(avt.fs.inst[avt.fs.current].getparam);
            
            avt.fs.studio.soLoadedFor = avt.fs.current;
        }
    },
    
    saveOutputSettings: function() {
        var _so = avt.fs.$("#so_Container");
        avt.fs.$.post(avt.fs.pageUrl, { 
                fn: "save_output_settings",
                inst: avt.fs.current,
                resTabId: _so.find("#so_ResultsPageTabId").val().length > 0 ? _so.find("#so_ResultsPageTabId").val() : _so.find("#so_ResultsPage").val(),
                pageSize: _so.find("#so_PageSize").val(),
                empty: _so.find("#so_emptyBox").val(),
                getParam: _so.find("#so_getParam").val()
            },
            function(data){
                var _so = avt.fs.$("#so_Container"); // so we don't create a closure
                if (data == "success") {
                    avt.fs.$.jGrowl("UI Settings succesfully saved!", {header: 'Success', life: 5000});
                    // propagate changes
                    avt.fs.inst[avt.fs.current].resTabId = _so.find("#so_ResultsPageTabId").val().length > 0 ? _so.find("#so_ResultsPageTabId").val() : _so.find("#so_ResultsPage").val();
                    avt.fs.inst[avt.fs.current].pagesize = _so.find("#so_PageSize").val();
                    avt.fs.inst[avt.fs.current].empty = _so.find("#so_emptyBox").val();
                    avt.fs.inst[avt.fs.current].getparam = _so.find("#so_getParam").val();

                    _so.find("#so_Error").hide();
                    avt.fs.studio.searchInst_loadData(avt.fs.current);

                } else {
                    _so.find("#so_Error").text(data).show();
                }
            }, "text");
    },
    
    
    deleteRule: function(ruleId) {
        avt.fs.studio.loading(true);
        avt.fs.$.get(avt.fs.pageUrl, { fn: "delete_rule", data: ruleId },
            function(data){
                
                // sync
                delete avt.fs.targets[ruleId];
                avt.fs.studio.populateSearchTargets();
                
                // also, invalidate targets for all instances
                for (var inst in avt.fs.inst)
                    delete avt.fs.inst[inst].targets;
                
                // reload current
                if (avt.fs.current)
                    avt.fs.studio.editSearchInst(avt.fs.current);
                    
                // if edit is opened for deleted rule, close it
                if (avt.fs.wizt.current && avt.fs.wizt.current.id == ruleId) {
                    avt.fs.wizt.hide_new_source();
                }
                
                // invalidate json
                avt.fs.studio.resetSearchTargetTrees();
                
                avt.fs.studio.loading(false);
                avt.fs.$.jGrowl("Search Rule succesfully deleted!", {header: 'Success', life: 5000});
        }, "text");
    },
    
    isScrolledIntoView: function (elem) {
        var docViewTop = avt.fs.$(window).scrollTop();
        var docViewBottom = docViewTop + avt.fs.$(window).height();

        var elemTop = avt.fs.$(elem).offset().top;
        var elemBottom = elemTop + avt.fs.$(elem).height();

        return ((elemBottom >= docViewTop) && (elemTop <= docViewBottom));
    }

}
