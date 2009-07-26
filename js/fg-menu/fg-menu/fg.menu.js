/*-------------------------------------------------------------------- 
Scripts for creating and manipulating custom menus based on standard <ul> markup
Version: 3.0, 03.31.2009

By: Maggie Costello Wachs (maggie@filamentgroup.com) and Scott Jehl (scott@filamentgroup.com)
	http://www.filamentgroup.com
	* reference articles: http://www.filamentgroup.com/lab/avt_jQuery_1_3_2_av1_ipod_style_drilldown_menu/
		
Copyright (c) 2009 Filament Group
Dual licensed under the MIT (filamentgroup.com/examples/mit-license.txt) and GPL (filamentgroup.com/examples/gpl-license.txt) licenses.
--------------------------------------------------------------------*/


var allUIMenus = [];

avt_jQuery_1_3_2_av1.fn.menu = function(options){
	var caller = this;
	var options = options;
	var m = new Menu(caller, options);	
	allUIMenus.push(m);
	
	avt_jQuery_1_3_2_av1(this)
	.mousedown(function(){
		if (!m.menuOpen) { m.showLoading(); };
	})	
	.click(function(){
		if (m.menuOpen == false) { m.showMenu(); }
		else { m.kill(); };
		return false;
	});	
};

function Menu(caller, options){
	var menu = this;
	var caller = avt_jQuery_1_3_2_av1(caller);
	var container = avt_jQuery_1_3_2_av1('<div class="fg-menu-container ui-widget ui-widget-content ui-corner-all">'+options.content+'</div>');
	
	this.menuOpen = false;
	this.menuExists = false;
	
	var options = avt_jQuery_1_3_2_av1.extend({
		content: null,
		width: 180, // width of menu container, must be set or passed in to calculate widths of child menus
		maxHeight: 180, // max height of menu (if a drilldown: height does not include breadcrumb)
		positionOpts: {
			posX: 'left', 
			posY: 'bottom',
			offsetX: 0,
			offsetY: 0,
			directionH: 'right',
			directionV: 'down', 
			detectH: true, // do horizontal collision detection  
			detectV: true, // do vertical collision detection
			linkToFront: false
		},
		showSpeed: 200, // show/hide speed in milliseconds
		callerOnState: 'ui-state-active', // class to change the appearance of the link/button when the menu is showing
		loadingState: 'ui-state-loading', // class added to the link/button while the menu is created
		linkHover: 'ui-state-hover', // class for menu option hover state
		linkHoverSecondary: 'li-hover', // alternate class, may be used for multi-level menus		
	// ----- multi-level menu defaults -----
		crossSpeed: 200, // cross-fade speed for multi-level menus
		crumbDefaultText: 'Choose an option:',
		backLink: true, // in the ipod-style menu: instead of breadcrumbs, show only a 'back' link
		backLinkText: 'Back',
		flyOut: false, // multi-level menus are ipod-style by default; this parameter overrides to make a flyout instead
		flyOutOnState: 'ui-state-default',
		nextMenuLink: 'ui-icon-triangle-1-e', // class to style the link (specifically, a span within the link) used in the multi-level menu to show the next level
		topLinkText: 'All',
		nextCrumbLink: 'ui-icon-carat-1-e'	
	}, options);
	
	var killAllMenus = function(){
		avt_jQuery_1_3_2_av1.each(allUIMenus, function(i){
			if (allUIMenus[i].menuOpen) { allUIMenus[i].kill(); };
		});
	};
	
	this.kill = function(){
		caller
			.removeClass(options.loadingState)
			.removeClass('fg-menu-open')
			.removeClass(options.callerOnState);	
		container.find('li').removeClass(options.linkHoverSecondary).find('a').removeClass(options.linkHover);		
		if (options.flyOutOnState) { container.find('li a').removeClass(options.flyOutOnState); };	
		if (options.callerOnState) { 	caller.removeClass(options.callerOnState); };			
		if (container.is('.fg-menu-ipod')) { menu.resetDrilldownMenu(); };
		if (container.is('.fg-menu-flyout')) { menu.resetFlyoutMenu(); };	
		container.parent().hide();	
		menu.menuOpen = false;
		avt_jQuery_1_3_2_av1(document).unbind('click', killAllMenus);
		avt_jQuery_1_3_2_av1(document).unbind('keydown');
	};
	
	this.showLoading = function(){
		caller.addClass(options.loadingState);
	};

	this.showMenu = function(){
		killAllMenus();
		if (!menu.menuExists) { menu.create() };
		caller
			.addClass('fg-menu-open')
			.addClass(options.callerOnState);
		container.parent().show().click(function(){ menu.kill(); return false; });
		container.hide().slideDown(options.showSpeed).find('.fg-menu:eq(0)');
		menu.menuOpen = true;
		caller.removeClass(options.loadingState);
		avt_jQuery_1_3_2_av1(document).click(killAllMenus);
		
		// assign key events
		avt_jQuery_1_3_2_av1(document).keydown(function(event){
			var e;
			if (event.which !="") { e = event.which; }
			else if (event.charCode != "") { e = event.charCode; }
			else if (event.keyCode != "") { e = event.keyCode; }
			
			var menuType = (avt_jQuery_1_3_2_av1(event.target).parents('div').is('.fg-menu-flyout')) ? 'flyout' : 'ipod' ;
			
			switch(e) {
				case 37: // left arrow 
					if (menuType == 'flyout') {
						avt_jQuery_1_3_2_av1(event.target).trigger('mouseout');
						if (avt_jQuery_1_3_2_av1('.'+options.flyOutOnState).size() > 0) { avt_jQuery_1_3_2_av1('.'+options.flyOutOnState).trigger('mouseover'); };
					};
					
					if (menuType == 'ipod') {
						avt_jQuery_1_3_2_av1(event.target).trigger('mouseout');
						if (avt_jQuery_1_3_2_av1('.fg-menu-footer').find('a').size() > 0) { avt_jQuery_1_3_2_av1('.fg-menu-footer').find('a').trigger('click'); };
						if (avt_jQuery_1_3_2_av1('.fg-menu-header').find('a').size() > 0) { avt_jQuery_1_3_2_av1('.fg-menu-current-crumb').prev().find('a').trigger('click'); };
						if (avt_jQuery_1_3_2_av1('.fg-menu-current').prev().is('.fg-menu-indicator')) {
							avt_jQuery_1_3_2_av1('.fg-menu-current').prev().trigger('mouseover');							
						};						
					};
					return false;
					break;
					
				case 38: // up arrow 
					if (avt_jQuery_1_3_2_av1(event.target).is('.' + options.linkHover)) {	
						var prevLink = avt_jQuery_1_3_2_av1(event.target).parent().prev().find('a:eq(0)');						
						if (prevLink.size() > 0) {
							avt_jQuery_1_3_2_av1(event.target).trigger('mouseout');
							prevLink.trigger('mouseover');
						};						
					}
					else { container.find('a:eq(0)').trigger('mouseover'); }
					return false;
					break;
					
				case 39: // right arrow 
					if (avt_jQuery_1_3_2_av1(event.target).is('.fg-menu-indicator')) {						
						if (menuType == 'flyout') {
							avt_jQuery_1_3_2_av1(event.target).next().find('a:eq(0)').trigger('mouseover');
						}
						else if (menuType == 'ipod') {
							avt_jQuery_1_3_2_av1(event.target).trigger('click');						
							setTimeout(function(){
								avt_jQuery_1_3_2_av1(event.target).next().find('a:eq(0)').trigger('mouseover');
							}, options.crossSpeed);
						};				
					}; 
					return false;
					break;
					
				case 40: // down arrow 
					if (avt_jQuery_1_3_2_av1(event.target).is('.' + options.linkHover)) {
						var nextLink = avt_jQuery_1_3_2_av1(event.target).parent().next().find('a:eq(0)');						
						if (nextLink.size() > 0) {							
							avt_jQuery_1_3_2_av1(event.target).trigger('mouseout');
							nextLink.trigger('mouseover');
						};				
					}
					else { container.find('a:eq(0)').trigger('mouseover'); }		
					return false;						
					break;
					
				case 27: // escape
					killAllMenus();
					break;
					
				case 13: // enter
					if (avt_jQuery_1_3_2_av1(event.target).is('.fg-menu-indicator') && menuType == 'ipod') {							
						avt_jQuery_1_3_2_av1(event.target).trigger('click');						
						setTimeout(function(){
							avt_jQuery_1_3_2_av1(event.target).next().find('a:eq(0)').trigger('mouseover');
						}, options.crossSpeed);					
					}; 
					break;
			};			
		});
	};
	
	this.create = function(){	
		container.css({ width: options.width }).appendTo('body').find('ul:first').not('.fg-menu-breadcrumb').addClass('fg-menu');
		container.find('ul, li a').addClass('ui-corner-all');
		
		// aria roles & attributes
		container.find('ul').attr('role', 'menu').eq(0).attr('aria-activedescendant','active-menuitem').attr('aria-labelledby', caller.attr('id'));
		container.find('li').attr('role', 'menuitem');
		container.find('li:has(ul)').attr('aria-haspopup', 'true').find('ul').attr('aria-expanded', 'false');
		container.find('a').attr('tabindex', '-1');
		
		// when there are multiple levels of hierarchy, create flyout or drilldown menu
		if (container.find('ul').size() > 1) {
			if (options.flyOut) { menu.flyout(container, options); }
			else { menu.drilldown(container, options); }	
		}
		else {
			container.find('a').click(function(){
				menu.chooseItem(this);
				return false;
			});
		};	
		
		if (options.linkHover) {
			var allLinks = container.find('.fg-menu li a');
			allLinks.hover(
				function(){
					var menuitem = avt_jQuery_1_3_2_av1(this);
					avt_jQuery_1_3_2_av1('.'+options.linkHover).removeClass(options.linkHover).blur().parent().removeAttr('id');
					avt_jQuery_1_3_2_av1(this).addClass(options.linkHover).focus().parent().attr('id','active-menuitem');
				},
				function(){
					avt_jQuery_1_3_2_av1(this).removeClass(options.linkHover).blur().parent().removeAttr('id');
				}
			);
		};
		
		if (options.linkHoverSecondary) {
			container.find('.fg-menu li').hover(
				function(){
					avt_jQuery_1_3_2_av1(this).siblings('li').removeClass(options.linkHoverSecondary);
					if (options.flyOutOnState) { avt_jQuery_1_3_2_av1(this).siblings('li').find('a').removeClass(options.flyOutOnState); }
					avt_jQuery_1_3_2_av1(this).addClass(options.linkHoverSecondary);
				},
				function(){ avt_jQuery_1_3_2_av1(this).removeClass(options.linkHoverSecondary); }
			);
		};	
		
		menu.setPosition(container, caller, options);
		menu.menuExists = true;
	};
	
	this.chooseItem = function(item){
		menu.kill();
		// edit this for your own custom function/callback:
		avt_jQuery_1_3_2_av1('#menuSelection').text(avt_jQuery_1_3_2_av1(item).text());	
        if (avt_jQuery_1_3_2_av1(item).attr('href').indexOf("javascript:") == -1) {
            location.href = avt_jQuery_1_3_2_av1(item).attr('href');
        }
        // if (!avt_jQuery_1_3_2_av1(item).attr('onclick') || avt_jQuery_1_3_2_av1(item).attr('onclick').length == 0) {
            // if (avt_jQuery_1_3_2_av1(item).attr('href').indexOf("javascript:") == -1) {
                // location.href = avt_jQuery_1_3_2_av1(item).attr('href');
            // } else {
                // eval(avt_jQuery_1_3_2_av1(item).attr('href').substring(avt_jQuery_1_3_2_av1(item).attr('href').indexOf("javascript:")));
            // }
        // }
	};
};

Menu.prototype.flyout = function(container, options) {
	var menu = this;
	
	this.resetFlyoutMenu = function(){
		var allLists = container.find('ul ul');
		allLists.removeClass('ui-widget-content').hide();	
	};
	
	container.addClass('fg-menu-flyout').find('li:has(ul)').each(function(){
		var linkWidth = container.width();
		var showTimer, hideTimer;
		var allSubLists = avt_jQuery_1_3_2_av1(this).find('ul');		
		
		allSubLists.css({ left: linkWidth, width: linkWidth }).hide();
			
		avt_jQuery_1_3_2_av1(this).find('a:eq(0)').addClass('fg-menu-indicator').html('<span>' + avt_jQuery_1_3_2_av1(this).find('a:eq(0)').text() + '</span><span class="ui-icon '+options.nextMenuLink+'"></span>').hover(
			function(){
				clearTimeout(hideTimer);
				var subList = avt_jQuery_1_3_2_av1(this).next();
				if (!fitVertical(subList, avt_jQuery_1_3_2_av1(this).offset().top)) { subList.css({ top: 'auto', bottom: 0 }); };
				if (!fitHorizontal(subList, avt_jQuery_1_3_2_av1(this).offset().left + 100)) { subList.css({ left: 'auto', right: linkWidth, 'z-index': 999 }); };
				showTimer = setTimeout(function(){
					subList.addClass('ui-widget-content').show(options.showSpeed).attr('aria-expanded', 'true');	
				}, 300);	
			},
			function(){
				clearTimeout(showTimer);
				var subList = avt_jQuery_1_3_2_av1(this).next();
				hideTimer = setTimeout(function(){
					subList.removeClass('ui-widget-content').hide(options.showSpeed).attr('aria-expanded', 'false');
				}, 400);	
			}
		);

		avt_jQuery_1_3_2_av1(this).find('ul a').hover(
			function(){
				clearTimeout(hideTimer);
				if (avt_jQuery_1_3_2_av1(this).parents('ul').prev().is('a.fg-menu-indicator')) {
					avt_jQuery_1_3_2_av1(this).parents('ul').prev().addClass(options.flyOutOnState);
				}
			},
			function(){
				hideTimer = setTimeout(function(){
					allSubLists.hide(options.showSpeed);
					container.find(options.flyOutOnState).removeClass(options.flyOutOnState);
				}, 500);	
			}
		);	
	});
	
	container.find('a').click(function(){
		menu.chooseItem(this);
		return false;
	});
};


Menu.prototype.drilldown = function(container, options) {
	var menu = this;	
	var topList = container.find('.fg-menu');	
	var breadcrumb = avt_jQuery_1_3_2_av1('<ul class="fg-menu-breadcrumb ui-widget-header ui-corner-all ui-helper-clearfix"></ul>');
	var crumbDefaultHeader = avt_jQuery_1_3_2_av1('<li class="fg-menu-breadcrumb-text">'+options.crumbDefaultText+'</li>');
	var firstCrumbText = (options.backLink) ? options.backLinkText : options.topLinkText;
	var firstCrumbClass = (options.backLink) ? 'fg-menu-prev-list' : 'fg-menu-all-lists';
	var firstCrumbLinkClass = (options.backLink) ? 'ui-state-default ui-corner-all' : '';
	var firstCrumbIcon = (options.backLink) ? '<span class="ui-icon ui-icon-triangle-1-w"></span>' : '';
	var firstCrumb = avt_jQuery_1_3_2_av1('<li class="'+firstCrumbClass+'"><a href="#" class="'+firstCrumbLinkClass+'">'+firstCrumbIcon+firstCrumbText+'</a></li>');
	
	container.addClass('fg-menu-ipod');
	
	if (options.backLink) { breadcrumb.addClass('fg-menu-footer').appendTo(container).hide(); }
	else { breadcrumb.addClass('fg-menu-header').prependTo(container); };
	breadcrumb.append(crumbDefaultHeader);
	
	var checkMenuHeight = function(el){
		if (el.height() > options.maxHeight) { el.addClass('fg-menu-scroll') };	
		el.css({ height: options.maxHeight });
	};
	
	var resetChildMenu = function(el){ el.removeClass('fg-menu-scroll').removeClass('fg-menu-current').height('auto'); };
	
	this.resetDrilldownMenu = function(){
		avt_jQuery_1_3_2_av1('.fg-menu-current').removeClass('fg-menu-current');
		topList.animate({ left: 0 }, options.crossSpeed, function(){
			avt_jQuery_1_3_2_av1(this).find('ul').each(function(){
				avt_jQuery_1_3_2_av1(this).hide();
				resetChildMenu(avt_jQuery_1_3_2_av1(this));				
			});
			topList.addClass('fg-menu-current');			
		});		
		avt_jQuery_1_3_2_av1('.fg-menu-all-lists').find('span').remove();	
		breadcrumb.empty().append(crumbDefaultHeader);		
		avt_jQuery_1_3_2_av1('.fg-menu-footer').empty().hide();	
		checkMenuHeight(topList);		
	};
	
	topList
		.addClass('fg-menu-content fg-menu-current ui-widget-content ui-helper-clearfix')
		.css({ width: container.width() })
		.find('ul')
			.css({ width: container.width(), left: container.width() })
			.addClass('ui-widget-content')
			.hide();		
	checkMenuHeight(topList);	
	
	topList.find('a').each(function(){
		// if the link opens a child menu:
		if (avt_jQuery_1_3_2_av1(this).next().is('ul')) {
			avt_jQuery_1_3_2_av1(this)
				.addClass('fg-menu-indicator')
				.each(function(){ avt_jQuery_1_3_2_av1(this).html('<span>' + avt_jQuery_1_3_2_av1(this).text() + '</span><span class="ui-icon '+options.nextMenuLink+'"></span>'); })
				.click(function(){ // ----- show the next menu			
					var nextList = avt_jQuery_1_3_2_av1(this).next();
		    		var parentUl = avt_jQuery_1_3_2_av1(this).parents('ul:eq(0)');   		
		    		var parentLeft = (parentUl.is('.fg-menu-content')) ? 0 : parseFloat(topList.css('left'));    		
		    		var nextLeftVal = Math.round(parentLeft - parseFloat(container.width()));
		    		var footer = avt_jQuery_1_3_2_av1('.fg-menu-footer');
		    		
		    		// show next menu   		
		    		resetChildMenu(parentUl);
		    		checkMenuHeight(nextList);
					topList.animate({ left: nextLeftVal }, options.crossSpeed);						
		    		nextList.show().addClass('fg-menu-current').attr('aria-expanded', 'true');    
		    		
		    		var setPrevMenu = function(backlink){
		    			var b = backlink;
		    			var c = avt_jQuery_1_3_2_av1('.fg-menu-current');
			    		var prevList = c.parents('ul:eq(0)');
			    		c.hide().attr('aria-expanded', 'false');
		    			resetChildMenu(c);
		    			checkMenuHeight(prevList);
			    		prevList.addClass('fg-menu-current').attr('aria-expanded', 'true');
			    		if (prevList.hasClass('fg-menu-content')) { b.remove(); footer.hide(); };
		    		};		
		
					// initialize "back" link
					if (options.backLink) {
						if (footer.find('a').size() == 0) {
							footer.show();
							avt_jQuery_1_3_2_av1('<a href="#"><span class="ui-icon ui-icon-triangle-1-w"></span> <span>Back</span></a>')
								.appendTo(footer)
								.click(function(){ // ----- show the previous menu
									var b = avt_jQuery_1_3_2_av1(this);
						    		var prevLeftVal = parseFloat(topList.css('left')) + container.width();		    						    		
						    		topList.animate({ left: prevLeftVal },  options.crossSpeed, function(){
						    			setPrevMenu(b);
						    		});			
									return false;
								});
						}
					}
					// or initialize top breadcrumb
		    		else { 
		    			if (breadcrumb.find('li').size() == 1){				
							breadcrumb.empty().append(firstCrumb);
							firstCrumb.find('a').click(function(){
								menu.resetDrilldownMenu();
								return false;
							});
						}
						avt_jQuery_1_3_2_av1('.fg-menu-current-crumb').removeClass('fg-menu-current-crumb');
						var crumbText = avt_jQuery_1_3_2_av1(this).find('span:eq(0)').text();
						var newCrumb = avt_jQuery_1_3_2_av1('<li class="fg-menu-current-crumb"><a href="javascript://" class="fg-menu-crumb">'+crumbText+'</a></li>');	
						newCrumb
							.appendTo(breadcrumb)
							.find('a').click(function(){
								if (avt_jQuery_1_3_2_av1(this).parent().is('.fg-menu-current-crumb')){
									menu.chooseItem(this);
								}
								else {
									var newLeftVal = - (avt_jQuery_1_3_2_av1('.fg-menu-current').parents('ul').size() - 1) * 180;
									topList.animate({ left: newLeftVal }, options.crossSpeed, function(){
										setPrevMenu();
									});
								
									// make this the current crumb, delete all breadcrumbs after this one, and navigate to the relevant menu
									avt_jQuery_1_3_2_av1(this).parent().addClass('fg-menu-current-crumb').find('span').remove();
									avt_jQuery_1_3_2_av1(this).parent().nextAll().remove();									
								};
								return false;
							});
						newCrumb.prev().append(' <span class="ui-icon '+options.nextCrumbLink+'"></span>');
		    		};			
		    		return false;    		
    			});
		}
		// if the link is a leaf node (doesn't open a child menu)
		else {
			avt_jQuery_1_3_2_av1(this).click(function(){
				menu.chooseItem(this);
				return false;
			});
		};
	});
};


/* Menu.prototype.setPosition parameters (defaults noted with *):
	referrer = the link (or other element) used to show the overlaid object 
	settings = can override the defaults:
		- posX/Y: where the top left corner of the object should be positioned in relation to its referrer.
				X: left*, center, right
				Y: top, center, bottom*
		- offsetX/Y: the number of pixels to be offset from the x or y position.  Can be a positive or negative number.
		- directionH/V: where the entire menu should appear in relation to its referrer.
				Horizontal: left*, right
				Vertical: up, down*
		- detectH/V: detect the viewport horizontally / vertically
		- linkToFront: copy the menu link and place it on top of the menu (visual effect to make it look like it overlaps the object) */

Menu.prototype.setPosition = function(widget, caller, options) { 
	var el = widget;
	var referrer = caller;
	var dims = {
		refX: referrer.offset().left,
		refY: referrer.offset().top,
		refW: referrer.getTotalWidth(),
		refH: referrer.getTotalHeight()
	};	
	var options = options;
	var xVal, yVal;
	
	var helper = avt_jQuery_1_3_2_av1('<div class="positionHelper"></div>');
	helper.css({ position: 'absolute', left: dims.refX, top: dims.refY, width: dims.refW, height: dims.refH });
	el.wrap(helper);
	
	// get X pos
	switch(options.positionOpts.posX) {
		case 'left': 	xVal = 0; 
			break;				
		case 'center': xVal = dims.refW / 2;
			break;				
		case 'right': xVal = dims.refW;
			break;
	};
	
	// get Y pos
	switch(options.positionOpts.posY) {
		case 'top': 	yVal = 0;
			break;				
		case 'center': yVal = dims.refH / 2;
			break;				
		case 'bottom': yVal = dims.refH;
			break;
	};
	
	// add the offsets (zero by default)
	xVal += options.positionOpts.offsetX;
	yVal += options.positionOpts.offsetY;
	
	// position the object vertically
	if (options.positionOpts.directionV == 'up') {
		el.css({ top: 'auto', bottom: yVal });
		if (options.positionOpts.detectV && !fitVertical(el)) {
			el.css({ bottom: 'auto', top: yVal });
		}
	} 
	else {
		el.css({ bottom: 'auto', top: yVal });
		if (options.positionOpts.detectV && !fitVertical(el)) {
			el.css({ top: 'auto', bottom: yVal });
		}
	};
	
	// and horizontally
	if (options.positionOpts.directionH == 'left') {
		el.css({ left: 'auto', right: xVal });
		if (options.positionOpts.detectH && !fitHorizontal(el)) {
			el.css({ right: 'auto', left: xVal });
		}
	} 
	else {
		el.css({ right: 'auto', left: xVal });
		if (options.positionOpts.detectH && !fitHorizontal(el)) {
			el.css({ left: 'auto', right: xVal });
		}
	};
	
	// if specified, clone the referring element and position it so that it appears on top of the menu
	if (options.positionOpts.linkToFront) {
		referrer.clone().addClass('linkClone').css({
			position: 'absolute', 
			top: 0, 
			right: 'auto', 
			bottom: 'auto', 
			left: 0, 
			width: referrer.width(), 
			height: referrer.height()
		}).insertAfter(el);
	};
};


/* Utilities to sort and find viewport dimensions */

function sortBigToSmall(a, b) { return b - a; };

avt_jQuery_1_3_2_av1.fn.getTotalWidth = function(){
	return avt_jQuery_1_3_2_av1(this).width() + parseInt(avt_jQuery_1_3_2_av1(this).css('paddingRight')) + parseInt(avt_jQuery_1_3_2_av1(this).css('paddingLeft')) + parseInt(avt_jQuery_1_3_2_av1(this).css('borderRightWidth')) + parseInt(avt_jQuery_1_3_2_av1(this).css('borderLeftWidth'));
};

avt_jQuery_1_3_2_av1.fn.getTotalHeight = function(){
	return avt_jQuery_1_3_2_av1(this).height() + parseInt(avt_jQuery_1_3_2_av1(this).css('paddingTop')) + parseInt(avt_jQuery_1_3_2_av1(this).css('paddingBottom')) + parseInt(avt_jQuery_1_3_2_av1(this).css('borderTopWidth')) + parseInt(avt_jQuery_1_3_2_av1(this).css('borderBottomWidth'));
};

function getScrollTop(){
	return self.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;
};

function getScrollLeft(){
	return self.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft;
};

function getWindowHeight(){
	var de = document.documentElement;
	return self.innerHeight || (de && de.clientHeight) || document.body.clientHeight;
};

function getWindowWidth(){
	var de = document.documentElement;
	return self.innerWidth || (de && de.clientWidth) || document.body.clientWidth;
};

/* Utilities to test whether an element will fit in the viewport
	Parameters:
	el = element to position, required
	leftOffset / topOffset = optional parameter if the offset cannot be calculated (i.e., if the object is in the DOM but is set to display: 'none') */
	
function fitHorizontal(el, leftOffset){
	var leftVal = parseInt(leftOffset) || avt_jQuery_1_3_2_av1(el).offset().left;
	return (leftVal + avt_jQuery_1_3_2_av1(el).width() <= getWindowWidth() + getScrollLeft() && leftVal - getScrollLeft() >= 0);
};

function fitVertical(el, topOffset){
	var topVal = parseInt(topOffset) || avt_jQuery_1_3_2_av1(el).offset().top;
	return (topVal + avt_jQuery_1_3_2_av1(el).height() <= getWindowHeight() + getScrollTop() && topVal - getScrollTop() >= 0);
};

/*-------------------------------------------------------------------- 
 * javascript method: "pxToEm"
 * by:
   Scott Jehl (scott@filamentgroup.com) 
   Maggie Wachs (maggie@filamentgroup.com)
   http://www.filamentgroup.com
 *
 * Copyright (c) 2008 Filament Group
 * Dual licensed under the MIT (filamentgroup.com/examples/mit-license.txt) and GPL (filamentgroup.com/examples/gpl-license.txt) licenses.
 *
 * Description: Extends the native Number and String objects with pxToEm method. pxToEm converts a pixel value to ems depending on inherited font size.  
 * Article: http://www.filamentgroup.com/lab/retaining_scalable_interfaces_with_pixel_to_em_conversion/
 * Demo: http://www.filamentgroup.com/examples/pxToEm/	 	
 *							
 * Options:  	 								
 		scope: string or avt_jQuery_1_3_2_av1 selector for font-size scoping
 		reverse: Boolean, true reverses the conversion to em-px
 * Dependencies: avt_jQuery_1_3_2_av1 library						  
 * Usage Example: myPixelValue.pxToEm(); or myPixelValue.pxToEm({'scope':'#navigation', reverse: true});
 *
 * Version: 2.0, 08.01.2008 
 * Changelog:
 *		08.02.2007 initial Version 1.0
 *		08.01.2008 - fixed font-size calculation for IE
--------------------------------------------------------------------*/

Number.prototype.pxToEm = String.prototype.pxToEm = function(settings){
	//set defaults
	settings = avt_jQuery_1_3_2_av1.extend({
		scope: 'body',
		reverse: false
	}, settings);
	
	var pxVal = (this == '') ? 0 : parseFloat(this);
	var scopeVal;
	var getWindowWidth = function(){
		var de = document.documentElement;
		return self.innerWidth || (de && de.clientWidth) || document.body.clientWidth;
	};	
	
	/* When a percentage-based font-size is set on the body, IE returns that percent of the window width as the font-size. 
		For example, if the body font-size is 62.5% and the window width is 1000px, IE will return 625px as the font-size. 	
		When this happens, we calculate the correct body font-size (%) and multiply it by 16 (the standard browser font size) 
		to get an accurate em value. */
				
	if (settings.scope == 'body' && avt_jQuery_1_3_2_av1.browser.msie && (parseFloat(avt_jQuery_1_3_2_av1('body').css('font-size')) / getWindowWidth()).toFixed(1) > 0.0) {
		var calcFontSize = function(){		
			return (parseFloat(avt_jQuery_1_3_2_av1('body').css('font-size'))/getWindowWidth()).toFixed(3) * 16;
		};
		scopeVal = calcFontSize();
	}
	else { scopeVal = parseFloat(avt_jQuery_1_3_2_av1(settings.scope).css("font-size")); };
			
	var result = (settings.reverse == true) ? (pxVal * scopeVal).toFixed(2) + 'px' : (pxVal / scopeVal).toFixed(2) + 'em';
	return result;
};