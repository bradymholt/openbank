phantom.injectJs('./scripts/3000.js');

login();

casper.waitForSelector('#cash', function(){
	 this.echo("waiting for #cash...");
	casper.thenClick("a[href*='ViewMoreServices']");
});

casper.thenClick("a[href*='EXEC_DOWNLOAD']");

var download_form_action = '';
var account_primary_key = '';

casper.then(function() {
    //http://code.google.com/p/phantomjs/issues/detail?id=52
    download_form_action = this.evaluate(function() {
        return document.querySelector('form#accountActivityDownloadModel').getAttribute('action');
    });

    account_primary_key = this.evaluate(function(acct_name){
    	var primaryKey = $('select#primaryKey option:contains("' + acct_name + '")').val();
    	$('select#primaryKey').val(primaryKey).change();
    	return primaryKey;
    }, account_id);
});

casper.thenClick("input[name='Select']");

casper.then(function() {
	var startDate = date_start.substring(5,7) + "/" + date_start.substring(8,10) +  "/" + date_start.substring(2,4); //mm/dd/yy
	var endDate = date_end.substring(5,7) + "/" + date_end.substring(8,10) +  "/" + date_end.substring(2,4); //mm/dd/yy
	this.echo("using date range: " + startDate + "-" + endDate);

	this.echo("downloading statement from " + download_form_action + "...");
    var base64contents = this.base64encode(download_form_action, 'POST', {
   		'primaryKey' : account_primary_key,
        'fromDate': startDate,
        'toDate': endDate,
        'fileFormat': 'quickenOfx',
        'Download' : 'Download'
    });
	
	this.echo("decoding statement..");
	var data = decode_base64(base64contents) ;
    if (data.length == 0 || data.indexOf("no Account Activity information available within the date range") > -1){
    	fs.write(output_path + 'empty_statement.txt', data, 'w');
    } else {
		fs.write(output_path + 'statement.qfx', data, 'w');
	}
});

casper.run();