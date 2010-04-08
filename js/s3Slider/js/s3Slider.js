/* ------------------------------------------------------------------------
	s3Slider
	
	Developped By: Boban Karišik -> http://www.serie3.info/
        CSS Help: Mészáros Róbert -> http://www.perspectived.com/
	Version: 1.0
	
	Copyright: Feel free to redistribute the script/modify it, as
			   long as you leave my infos at the top.
------------------------------------------------------------------------- */


(function($){  

    $.fn.s3Slider = function(vars) {       
        
        var element     = this;
        var timeOut     = (vars.timeOut != undefined) ? vars.timeOut : 4000;
        var current     = null;
        var timeOutFn   = null;
        var faderStat   = true;
        var mOver       = false;
        var items       = this.find(".slider1Image"); //$("#" + element[0].id + "Content ." + element[0].id + "Image");
        var itemsSpan   = this.find(".slideText"); //$("#" + element[0].id + "Content ." + element[0].id + "Image span");
            
        items.each(function(i) {
    
            $(items[i]).mouseover(function() {
               mOver = true;
            });
            
            $(items[i]).mouseout(function() {
                mOver   = false;
                fadeElement(true);
            });
            
        });
        
        items.css("visibility", "visible").css("display", "none");
        
        var fadeElement = function(isMouseOut) {
            var thisTimeOut = (isMouseOut) ? (timeOut/2) : timeOut;
            thisTimeOut = (faderStat) ? 10 : thisTimeOut;
            if(items.length > 0) {
                timeOutFn = setTimeout(makeSlider, thisTimeOut);
            } else {
                console.log("Poof..");
            }
        }
        
        var makeSlider = function() {
            current = (current != null) ? current : items[(items.length-1)];
            var currNo      = avt_jQuery_1_3_2_av3.inArray(current, items) + 1
            currNo = (currNo == items.length) ? 0 : (currNo - 1);
            var newMargin   = $(element).width() * currNo;
            var img = $(items[currNo]).find("img")[0];
            if(faderStat == true) {
                if(!mOver) {
                    $(items[currNo]).parents(".sliderContent:first").animate({height:img.height,width:img.width}, "fast");
                    if ($(itemsSpan[currNo]).hasClass("topSlideText") || $(itemsSpan[currNo]).hasClass("bottomSlideText")) {
                        var w = img.width;
                        try {
                            var p = parseInt($(itemsSpan[currNo]).css("padding-left"));
                            w = w-p;
                        } catch (e) {}
                        
                        try {
                            var p = parseInt($(itemsSpan[currNo]).css("padding-left"));
                            w = w-p;
                        } catch (e) {}
                        
                        $(itemsSpan[currNo]).css("width", w);
                        $(itemsSpan[currNo]).css("height", "auto");
                        
                    } else {
                        if ($(itemsSpan[currNo]).hasClass("leftSlideText") || $(itemsSpan[currNo]).hasClass("rightSlideText")) {
                            $(itemsSpan[currNo]).css("height",img.height - parseInt($(itemsSpan[currNo]).css("padding-top")) - parseInt($(itemsSpan[currNo]).css("padding-bottom")));
                        }
                    }
                    
                    //alert($(itemsSpan[currNo]).css("width"));
                    $(items[currNo]).fadeIn((timeOut/6), function() {
                        if($(itemsSpan[currNo]).css('bottom') < 4) {
                            $(itemsSpan[currNo]).slideUp((timeOut/6), function() {
                                faderStat = false;
                                current = items[currNo];
                                if(!mOver) {
                                    fadeElement(false);
                                }
                            });
                        } else {
                            $(itemsSpan[currNo]).slideDown((timeOut/6), function() {
                                faderStat = false;
                                current = items[currNo];
                                if(!mOver) {
                                    fadeElement(false);
                                }
                            });
                        }
                    });
                }
            } else {
                if(!mOver) {
                    if($(itemsSpan[currNo]).css('bottom') < 4) {
                        $(itemsSpan[currNo]).slideDown((timeOut/6), function() {
                            $(items[currNo]).fadeOut((timeOut/6), function() {
                                faderStat = true;
                                current = items[(currNo+1)];
                                if(!mOver) {
                                    fadeElement(false);
                                }
                            });
                        });
                    } else {
                        $(itemsSpan[currNo]).slideUp((timeOut/6), function() {
                        $(items[currNo]).fadeOut((timeOut/6), function() {
                                faderStat = true;
                                current = items[(currNo+1)];
                                if(!mOver) {
                                    fadeElement(false);
                                }
                            });
                        });
                    }
                }
            }
        }
        
        makeSlider();

    };  

})(avt_jQuery_1_3_2_av3);  