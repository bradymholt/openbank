//Chase 

phantom.injectJs('./scripts/10898.js');

login();

casper.waitForSelector('#FORM1', function(){
	casper.then(function() {
	  var accounts = this.evaluate(function(){
	     account_links = document.querySelectorAll('a.acct_links');
	     return Array.prototype.map.call(account_links, function(e) {
	        return { description: e.innerText, account_id: e.getAttribute('href').substring(e.getAttribute('href').indexOf('AI=') + 3), account_type: "CREDITCARD" };
	     });
	  });

	  fs.write(output_path + 'accounts.json', JSON.stringify(accounts), 'w');
	});
});

casper.run();


