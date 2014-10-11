phantom.injectJs('./scripts/3000.js');

login();

casper.waitForSelector('#cash', function(){
	casper.thenClick("a[href*='ViewMoreServices']");
});

casper.thenClick("a[href*='EXEC_DOWNLOAD']");

casper.then(function() {
    //http://code.google.com/p/phantomjs/issues/detail?id=52
    var download_form_action = this.evaluate(function() {
        return document.querySelector('form[name="DownloadFormBean"]').getAttribute('action');
    });

    var account_primary_key = this.evaluate(function(acct_name){
    	return $('select#Account option:contains("' + acct_name + '")').val();
    }, account_id);

	console.log(download_form_action);

	var startDate = date_start.substring(5,7) + "/" + date_start.substring(8,10) +  "/" + date_start.substring(2,4); //mm/dd/yy
	var endDate = date_end.substring(5,7) + "/" + date_end.substring(8,10) +  "/" + date_end.substring(2,4); //mm/dd/yy
	
    var base64contents = this.base64encode(download_form_action, 'POST', {
        'fromDate': startDate,
        'toDate': endDate,
        'fileFormat': 'quickenOfx',
        'downloadButton' : 'Download',
        'primaryKey' : account_primary_key,
        'hiddenPrimary' : account_primary_key
    });

    var data = decode_base64(base64contents) ;
    if (data.indexOf("no Account Activity information available within the date range") > -1){
    	fs.write(output_path + 'empty_statement.txt', data, 'w');
    } else {
		fs.write(output_path + 'statement.qfx', data, 'w');
	}
});

casper.run();