phantom.injectJs('./scripts/6812.js');

login();

casper.waitForSelector('.account-row', function(){
	casper.then(function() {
	  var accounts = this.evaluate(function(){
	     links = document.querySelectorAll('.account-row .image-account a');
	     return Array.prototype.map.call(links, function(e) {
	        return { description: e.getAttribute('id'), account_id: e.getAttribute('id'), account_type: "CHECKING" };
	     });
	  });

	  fs.write(output_path + 'accounts.json', JSON.stringify(accounts), 'w');
	});
});

casper.run();
