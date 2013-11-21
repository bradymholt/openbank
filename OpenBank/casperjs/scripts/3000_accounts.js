phantom.injectJs('./scripts/3000.js');

login();

casper.waitForSelector('#cash', function(){
	casper.then(function() {
	  var accounts = this.evaluate(function(){
	     account_links = document.querySelectorAll('#cash a.account');
	     return Array.prototype.map.call(account_links, function(e) {
	        return { description: e.innerText, account_id: e.innerText, account_type: "CHECKING" };
	     });
	  });

	  fs.write(output_path + 'accounts.json', JSON.stringify(accounts), 'w');
	});
});

casper.run();
