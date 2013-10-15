var fs = require('fs');
var args = require('system').args;

phantom.casperPath = args[1]; 
phantom.injectJs(phantom.casperPath + '/bin/bootstrap.js');

var casper = require('casper').create({
	clientScripts: [],  //['includes/jquery-1.7.2.min.js'], 
	verbose: true, logLevel: 'debug'
});

var user_id = casper.cli.get("user_id");
var password = casper.cli.get("password");
var security_answers = (casper.cli.get("security_answers") || "").split();
var account_id = casper.cli.get("account_id");
var date_start = casper.cli.get("date_start");
var date_end = casper.cli.get("date_end");
var output_path = casper.cli.get("output_path") + '/';

casper.on('load.finished', function(resource) {
	var fileNamePrefix = new Date().getTime();
	
	this.capture(casper.cli.get("output_path") + '/' + fileNamePrefix + '-screenshot.png', {
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
	
	fs.write(output_path + fileNamePrefix + '-content.html', innerHTML, 'w');
});