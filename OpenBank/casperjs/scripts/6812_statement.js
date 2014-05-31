phantom.injectJs('./scripts/6812.js');

login();

casper.waitForSelector('a[id=\"'+account_id+'\"]', function(){
	casper.thenClick('a[id=\"'+account_id+'\"]') ;
});

casper.thenClick('a[name="export_trans_nav_top"]') ;

casper.then(function() {
    //http://code.google.com/p/phantomjs/issues/detail?id=52
    var download_form_action = this.evaluate(function() {
        return document.querySelector('form[name="transactionDownloadForm"]').getAttribute('action');
    });

	console.log(download_form_action);

	var startDate = date_start.substring(5,7) + "/" + date_start.substring(8,10) +  "/" + date_start.substring(0,4); //mm/dd/yyyy
	var endDate = date_end.substring(5,7) + "/" + date_end.substring(8,10) +  "/" + date_end.substring(0,4); //mm/dd/yyyy
	
    var base64contents = this.base64encode(download_form_action, 'POST', {
        selectedTransPeriod: '',
        downloadTransactionType: 'customRange',
        'searchBean.searchMoreOptionsPanelUsed': 'false',
        'searchBean.timeFrameStartDate': startDate,
        'searchBean.timeFrameEndDate': endDate,
        formatType: 'qfx'  
    });

    var data = decode_base64(base64contents);
    if (data.indexOf("requested to download has no posted transactions") > -1){
    	fs.write(output_path + 'empty_statement.txt', data, 'w');
    } else {
		fs.write(output_path + 'statement.qfx', data, 'w');
	}
});

casper.run();
