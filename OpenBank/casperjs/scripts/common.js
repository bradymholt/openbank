var fs = require('fs');
var args = require('system').args;

phantom.casperPath = args[1]; 
phantom.injectJs(phantom.casperPath + '/bin/bootstrap.js');

var casper = require('casper').create({
	clientScripts: [],  //['includes/jquery-1.7.2.min.js'], 
	verbose: true, 
	logLevel: 'debug',
	pageSettings: { 
        webSecurityEnabled: false,
        userAgent: 'Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.2 Safari/537.36' 
    }
});

var user_id = casper.cli.get("user_id");
var password = casper.cli.get("password");
var security_answers = casper.cli.raw.get("security_answers");
    console.log(security_answers);
var account_id = casper.cli.get("account_id");
var date_start = casper.cli.get("date_start"); //YYYY-MM-DD format
var date_end = casper.cli.get("date_end");     //YYYY-MM-DD format
var output_path = casper.cli.get("output_path") + '/';

casper.on('load.finished', function(resource) {
	var fileNamePrefix = new Date().getTime();
	
	this.echo('saving: ' + output_path + fileNamePrefix + '-screenshot.png');
	this.capture(output_path + fileNamePrefix + '-screenshot.png', {
		top: 0,
		left: 0,
		width: 1024,
		height: 768
	});
	
	var href = this.evaluate(function() {
		return document.location.href;
	});

	var innerHTML = this.evaluate(function(){
		return document.body.innerHTML;
	});
	
	innerHTML = '<!-- ' + href + ' -->\n\n' + innerHTML;
	
	this.echo('saving: ' + output_path + fileNamePrefix + '-content.html');
	fs.write(output_path + fileNamePrefix + '-content.html', innerHTML, 'w');
});

//decode_base64 from http://stackoverflow.com/questions/2820249/base64-
//encoding-and-decoding-in-client-side-javascript
function decode_base64(s) {
  var e={},i,k,v=[],r='',w=String.fromCharCode;
  var n=[[65,91],[97,123],[48,58],[43,44],[47,48]];
  for(z in n){for(i=n[z][0];i<n[z][1];i++){v.push(w(i));}}
  for(i=0;i<64;i++){e[v[i]]=i;}
  for(i=0;i<s.length;i+=72){
  var b=0,c,x,l=0,o=s.substring(i,i+72);
    for(x=0;x<o.length;x++){
      c=e[o.charAt(x)];b=(b<<6)+c;l+=6;
      while(l>=8){r+=w((b>>>(l-=8))%256);}
    }
  } 
  return r;
}