var fs = require('fs');

var casper = require('casper').create({clientScripts:  ['includes/jquery-1.7.2.min.js'], 
	verbose: true, logLevel: 'debug'
});

casper.on('load.finished', function(resource) {
	var fileNamePrefix = new Date().getTime();
	
    if (casper.cli.has("debug")) {
		this.capture(casper.cli.get("outputpath") + '/' + fileNamePrefix + '-screenshot.png', {
			top: 100,
			left: 100,
			width: 1000,
			height: 800
		});
		
		var href = this.evaluate(function() {
			return document.location.href;
		});
	
		var innerHTML = this.evaluate(function(){
			return document.body.innerHTML;
		});
		
		innerHTML = '<!-- ' + href + ' -->\n\n' + innerHTML;
		
		fs.write(casper.cli.get("outputpath") + '/' + fileNamePrefix + '-content.html', innerHTML, 'w');
	}
});