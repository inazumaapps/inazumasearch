/*--------------------------------------------------------------------------*
 *  
 *  SmoothScroll JavaScript Library V2
 *  
 *  MIT-style license. 
 *  
 *  2007-2011 Kazuma Nishihata 
 *  http://www.to-r.net
 *  
 *--------------------------------------------------------------------------*/
 
new function(){

	var attr ="data-tor-smoothScroll";//for html5 , if you can't use html5 , this value change "class"
	var attrPatt = /noSmooth/;
	var d = document;//document short cut
	
	/*
	 *add Event
	  -------------------------------------------------*/
	function addEvent(elm,listener,fn){
		try{ // IE
			elm.addEventListener(listener,fn,false);
		}catch(e){
			elm.attachEvent(
				"on"+listener
				,function(){
					fn.apply(elm,arguments)
				}
			);
		}
	}

	/*
	 *Start SmoothScroll
	  -------------------------------------------------*/
	function SmoothScroll(a){
		if(d.getElementById(a.rel.replace(/.*\#/,""))){
			var e = d.getElementById(a.rel.replace(/.*\#/,""));
		}else{
			return;
		}
		
		//Move point
		var end=e.offsetTop
		var docHeight = d.documentElement.scrollHeight;
		var winHeight = window.innerHeight || d.documentElement.clientHeight
		if(docHeight-winHeight<end){
			var end = docHeight-winHeight;
		}
		
		//Current Point
		var start=window.pageYOffset || d.documentElement.scrollTop || d.body.scrollTop || 0;
		
		
		var flag=(end<start)?"up":"down";

		function scrollMe(start,end,flag) {
			setTimeout(
				function(){
					if(flag=="up" && start >= end){
						start=start-(start-end)/10-1;
						window.scrollTo(0,start)
						scrollMe(start,end,flag);
					}else if(flag=="down" && start <= end){
						start=start+(end-start)/10+1;
						window.scrollTo(0,start)
						scrollMe(start,end,flag);
					}else{
						scrollTo(0,end);
					}
					return ;
				}
				,5
			);
			
		}

		scrollMe(start,end,flag);
		
	}

	/*
	 *Add SmoothScroll
	  -------------------------------------------------*/
	addEvent(window,"load",function(){
		var anchors = d.getElementsByTagName("a");
		for(var i = 0 ,len=anchors.length; i<len ; i++){
			if(!attrPatt.test(anchors[i].getAttribute(attr)) && 
				anchors[i].href.replace(/\#[a-zA-Z0-9_]+/,"") == location.href.replace(/\#[a-zA-Z0-9_]+/,"")){
				anchors[i].rel = anchors[i].href;
				anchors[i].href = "javascript:void(0)";
				anchors[i].onclick=function(){SmoothScroll(this)}
			}
		}
	});

}