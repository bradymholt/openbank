phantom.injectJs('scripts/common.js');

casper.start('https://chaseonline.chase.com/Logon.aspx', function() {
	this.fill('form#Started', { 'UserID': casper.cli.get("username"),'Password': casper.cli.get("password")	}, false);
});

casper.then(function() {
	this.click('#logon');
});

var securityCodeEncountered = false;
var securityCodeRequested = false;

casper.then(function() {
	securityCodeEncountered = this.exists('#frmSSOSecAuthInformation');
	if (securityCodeEncountered) {
		this.click('#NextButton');
	}
});

casper.then(function () {
    securityCodeEncountered = this.exists('#oForm');
    if (securityCodeEncountered) {
		this.evaluate(function () {
		 $('form').submit();
		//casper.thenOpen('https://chaseonline.chase.com/public/machidnt/otpdeliverymode.aspx');
        //this.click('#NextButton');
		});
    }
});

casper.then(function() {
	if (securityCodeEncountered) {
		if (casper.cli.get("securitycode").toString() != '' ) {
			//go to Already have code screen
			this.click('#ancHaveIdentificationCode');
		}
		else {
			//request code
			this.echo('Submit request...');
			this.click('#usrCtrlOtp_rdoDelMethod6');
			this.click('#NextButton');
			securityCodeRequested = true;
		}
	}
});

casper.then(function () {
    securityCodeEncountered = this.exists('#oForm');
    if (securityCodeEncountered) {
        //this.fill('#oForm', {}, true); //this.click('#NextButton');
    }
});

casper.then(function(){
	if (securityCodeRequested) {
		fs.write(casper.cli.get("outputpath") + '/error-security-code-needed.txt', 'Activation code needs to be entered.  It was sent.', 'w');
		this.exit();
	}
	else if (securityCodeEncountered) {
		this.fill('form#frmValidateOTP', { 
			'usrCtrlOtp$txtActivationCode': casper.cli.get("securitycode"), 
			'usrCtrlOtp$txtPassword': casper.cli.get("password")  
		}, false);
		
		this.click('#NextButton');
	}
});

casper.then(function() {
	if (securityCodeEncountered) {
		if (this.exists('form#frmValidateOTP')) {
			this.echo('Invalid code!..');
			fs.write(casper.cli.get("outputpath") + '/error-invalid-security-code.txt', 'Invalid Activation Code!', 'w');
			this.exit();
		}
	}
	else {
		this.click('#NextButton');
	}
});

casper.thenOpen('https://chaseonline.chase.com/Secure/OSL.aspx?newstoken=false&LOB=COLLogon&Referer=https%3A%2F%2Fchaseonline.chase.com%2FLogon.aspx&resId=success&');

var accountID = '';
casper.then(function () {
    accountID = (function (aNo) {
        var aID = '0';
        var aNoLastFour = aNo.substring(aNo.length - 4);
        $('.acct_links').each(function (idx) {
            if ($(this).text().indexOf(aNoLastFour) > -1) {
                aID = $(this).attr('href').substring($(this).attr('href').indexOf('?AI=') + 4);
            }
        });
        return aID;
    }, { 'aNo': casper.cli.get("accountnumber").toString() });

    if (accountID == '0') {
        fs.write(casper.cli.get("outputpath") + '/error-account-not-found.txt', 'Account could not be found.', 'w');
        this.exit();
    }
});

casper.thenOpen('https://cards.chase.com/cc/Account/Activity/' + accountID, function(){
	var lastStatementBalance = this.evaluate(function() { return $('td:contains("Balance last statement")').next().text(); });
	lastStatementBalance = lastStatementBalance.replace('$', '-');
	fs.write(casper.cli.get("outputpath") + '/last-statement-balance.txt', lastStatementBalance, 'w');
});

casper.then(function(){
	this.download('https://cards.chase.com/cc/Account/DownloadAccountActivity?ai=' + accountID + '&pageGuid=0.5176098758820444&downloadType=OFX&StatementPeriodQuick=SINCE_LAST_STATEMENT&SortColumn=TransDate&SortOrder=up', 
		casper.cli.get("outputpath") + '/transactions.ofx');
});

casper.run(function() {
   casper.exit();
});