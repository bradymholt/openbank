//Chase 

phantom.injectJs('./scripts/10898.js');

login();

casper.waitForSelector("form", function(){
	casper.then(function(){
		var startDate = date_start.substring(5,7) + "%2F" + date_start.substring(8,10) +  "%2F" + date_start.substring(0,4); //mm/dd/yyyy
		var endDate = date_end.substring(5,7) + "%2F" + date_end.substring(8,10) + "%2F" + date_end.substring(0,4); //mm/dd/yyyy
	    var url = 'https://cards.chase.com/cc/Account/DownloadAccountActivity?ai=' + account_id + '&pageGuid=0.9619725034572184&downloadType=OFX&StatementPeriodAdvanced=SINCE_LAST_STATEMENT&SearchPeriodType=1&SearchPeriodType=1&DateLo=' + startDate + '&DateHi=' + endDate + '&TransactionType=&MerchantName=&AmountLo=&AmountHi=&SortColumn=TransDate&SortOrder=up';
		this.echo('Statement Download Url: ' + url);
		this.download(url, output_path + 'statement.qfx');
	});
});

casper.run(function() {
   casper.exit();
});
