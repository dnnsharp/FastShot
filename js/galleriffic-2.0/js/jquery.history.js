/*
 * avt_jQuery_1_3_2_av3 history plugin
 * 
 * sample page: http://www.mikage.to/avt_jQuery_1_3_2_av3/avt_jQuery_1_3_2_av3_history.html
 *
 * Copyright (c) 2006-2009 Taku Sano (Mikage Sawatari)
 * Licensed under the MIT License:
 *   http://www.opensource.org/licenses/mit-license.php
 *
 * Modified by Lincoln Cooper to add Safari support and only call the callback once during initialization
 * for msie when no initial hash supplied.
 */


avt_jQuery_1_3_2_av3.extend({
	historyCurrentHash: undefined,
	historyCallback: undefined,
	historyIframeSrc: undefined,
	
	historyInit: function(callback, src){
		avt_jQuery_1_3_2_av3.historyCallback = callback;
		if (src) avt_jQuery_1_3_2_av3.historyIframeSrc = src;
		var current_hash = location.hash.replace(/\?.*$/, '');
		
		avt_jQuery_1_3_2_av3.historyCurrentHash = current_hash;
		// if ((avt_jQuery_1_3_2_av3.browser.msie) && (avt_jQuery_1_3_2_av3.browser.version < 8)) {
		if (avt_jQuery_1_3_2_av3.browser.msie) {
			// To stop the callback firing twice during initilization if no hash present
			if (avt_jQuery_1_3_2_av3.historyCurrentHash == '') {
			avt_jQuery_1_3_2_av3.historyCurrentHash = '#';
		}
		
			// add hidden iframe for IE
			avt_jQuery_1_3_2_av3("body").prepend('<iframe id="avt_jQuery_1_3_2_av3_history" style="display: none;"'+
				(avt_jQuery_1_3_2_av3.historyIframeSrc ? ' src="'+avt_jQuery_1_3_2_av3.historyIframeSrc+'"' : '')
				+'></iframe>'
			);
			var ihistory = avt_jQuery_1_3_2_av3("#avt_jQuery_1_3_2_av3_history")[0];
			var iframe = ihistory.contentWindow.document;
			iframe.open();
			iframe.close();
			iframe.location.hash = current_hash;
		}
		else if (avt_jQuery_1_3_2_av3.browser.safari) {
			// etablish back/forward stacks
			avt_jQuery_1_3_2_av3.historyBackStack = [];
			avt_jQuery_1_3_2_av3.historyBackStack.length = history.length;
			avt_jQuery_1_3_2_av3.historyForwardStack = [];
			avt_jQuery_1_3_2_av3.lastHistoryLength = history.length;
			
			avt_jQuery_1_3_2_av3.isFirst = true;
		}
		if(current_hash)
			avt_jQuery_1_3_2_av3.historyCallback(current_hash.replace(/^#/, ''));
		setInterval(avt_jQuery_1_3_2_av3.historyCheck, 100);
	},
	
	historyAddHistory: function(hash) {
		// This makes the looping function do something
		avt_jQuery_1_3_2_av3.historyBackStack.push(hash);
		
		avt_jQuery_1_3_2_av3.historyForwardStack.length = 0; // clear forwardStack (true click occured)
		this.isFirst = true;
	},
	
	historyCheck: function(){
		// if ((avt_jQuery_1_3_2_av3.browser.msie) && (avt_jQuery_1_3_2_av3.browser.version < 8)) {
		if (avt_jQuery_1_3_2_av3.browser.msie) {
			// On IE, check for location.hash of iframe
			var ihistory = avt_jQuery_1_3_2_av3("#avt_jQuery_1_3_2_av3_history")[0];
			var iframe = ihistory.contentDocument || ihistory.contentWindow.document;
			var current_hash = iframe.location.hash.replace(/\?.*$/, '');
			if(current_hash != avt_jQuery_1_3_2_av3.historyCurrentHash) {
			
				location.hash = current_hash;
				avt_jQuery_1_3_2_av3.historyCurrentHash = current_hash;
				avt_jQuery_1_3_2_av3.historyCallback(current_hash.replace(/^#/, ''));
				
			}
		} else if (avt_jQuery_1_3_2_av3.browser.safari) {
			if(avt_jQuery_1_3_2_av3.lastHistoryLength == history.length && avt_jQuery_1_3_2_av3.historyBackStack.length > avt_jQuery_1_3_2_av3.lastHistoryLength) {
				avt_jQuery_1_3_2_av3.historyBackStack.shift();
			}
			if (!avt_jQuery_1_3_2_av3.dontCheck) {
				var historyDelta = history.length - avt_jQuery_1_3_2_av3.historyBackStack.length;
				avt_jQuery_1_3_2_av3.lastHistoryLength = history.length;
				
				if (historyDelta) { // back or forward button has been pushed
					avt_jQuery_1_3_2_av3.isFirst = false;
					if (historyDelta < 0) { // back button has been pushed
						// move items to forward stack
						for (var i = 0; i < Math.abs(historyDelta); i++) avt_jQuery_1_3_2_av3.historyForwardStack.unshift(avt_jQuery_1_3_2_av3.historyBackStack.pop());
					} else { // forward button has been pushed
						// move items to back stack
						for (var i = 0; i < historyDelta; i++) avt_jQuery_1_3_2_av3.historyBackStack.push(avt_jQuery_1_3_2_av3.historyForwardStack.shift());
					}
					var cachedHash = avt_jQuery_1_3_2_av3.historyBackStack[avt_jQuery_1_3_2_av3.historyBackStack.length - 1];
					if (cachedHash != undefined) {
						avt_jQuery_1_3_2_av3.historyCurrentHash = location.hash.replace(/\?.*$/, '');
						avt_jQuery_1_3_2_av3.historyCallback(cachedHash);
					}
				} else if (avt_jQuery_1_3_2_av3.historyBackStack[avt_jQuery_1_3_2_av3.historyBackStack.length - 1] == undefined && !avt_jQuery_1_3_2_av3.isFirst) {
					// back button has been pushed to beginning and URL already pointed to hash (e.g. a bookmark)
					// document.URL doesn't change in Safari
					if (location.hash) {
						var current_hash = location.hash;
						avt_jQuery_1_3_2_av3.historyCallback(location.hash.replace(/^#/, ''));
					} else {
						var current_hash = '';
						avt_jQuery_1_3_2_av3.historyCallback('');
					}
					avt_jQuery_1_3_2_av3.isFirst = true;
				}
			}
		} else {
			// otherwise, check for location.hash
			var current_hash = location.hash.replace(/\?.*$/, '');
			if(current_hash != avt_jQuery_1_3_2_av3.historyCurrentHash) {
				avt_jQuery_1_3_2_av3.historyCurrentHash = current_hash;
				avt_jQuery_1_3_2_av3.historyCallback(current_hash.replace(/^#/, ''));
			}
		}
	},
	historyLoad: function(hash){
		var newhash;
		hash = decodeURIComponent(hash.replace(/\?.*$/, ''));
		
		if (avt_jQuery_1_3_2_av3.browser.safari) {
			newhash = hash;
		}
		else {
			newhash = '#' + hash;
			location.hash = newhash;
		}
		avt_jQuery_1_3_2_av3.historyCurrentHash = newhash;
		
		// if ((avt_jQuery_1_3_2_av3.browser.msie) && (avt_jQuery_1_3_2_av3.browser.version < 8)) {
		if (avt_jQuery_1_3_2_av3.browser.msie) {
			var ihistory = avt_jQuery_1_3_2_av3("#avt_jQuery_1_3_2_av3_history")[0];
			var iframe = ihistory.contentWindow.document;
			iframe.open();
			iframe.close();
			iframe.location.hash = newhash;
			avt_jQuery_1_3_2_av3.lastHistoryLength = history.length;
			avt_jQuery_1_3_2_av3.historyCallback(hash);
		}
		else if (avt_jQuery_1_3_2_av3.browser.safari) {
			avt_jQuery_1_3_2_av3.dontCheck = true;
			// Manually keep track of the history values for Safari
			this.historyAddHistory(hash);
			
			// Wait a while before allowing checking so that Safari has time to update the "history" object
			// correctly (otherwise the check loop would detect a false change in hash).
			var fn = function() {avt_jQuery_1_3_2_av3.dontCheck = false;};
			window.setTimeout(fn, 200);
			avt_jQuery_1_3_2_av3.historyCallback(hash);
			// N.B. "location.hash=" must be the last line of code for Safari as execution stops afterwards.
			//      By explicitly using the "location.hash" command (instead of using a variable set to "location.hash") the
			//      URL in the browser and the "history" object are both updated correctly.
			location.hash = newhash;
		}
		else {
		  avt_jQuery_1_3_2_av3.historyCallback(hash);
		}
	}
});


