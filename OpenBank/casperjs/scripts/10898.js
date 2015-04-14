//Chase

phantom.injectJs('./scripts/common.js');

function login() {
	sign_in = "https://chaseonline.chase.com/Logon.aspx"
	casper.start(sign_in, function() {
	  this.fill('form[action="Logon.aspx"]', {
	    "UserID": user_id,
	    "Password": password
	  }, false);
	});

	casper.then(function() {
		this.click("#logon");
	});
	
	casper.waitForSelector('body', function() {
	  this.echo("security code check...");

	  challenge_exists = this.evaluate(function() {
	     return __utils__.exists('#frmSSOSecAuthInformation');
	  });
	
	  if(challenge_exists){
	  	this.echo("security code prompted!");

	  	if (security_answers == "") {
	      this.click('#NextButton');

	      casper.wait(2000, function() {
			//allow time redirect
		  });
      	  
      	  casper.waitForSelector('body', function() {
		    challenge_exists = this.evaluate(function() {
		    	return __utils__.exists('#frmOTPDeliveryMode');
		  	});
		  
		  	if(challenge_exists){
				this.fill('#frmOTPDeliveryMode', {
			   	 "usrCtrlOtp$rdoDelMethod": "rdoDelMethod6"
				}, false);

				casper.then(function() {
					this.click("#NextButton");
				});

				casper.then(function() {
			  		security_question = "An email has been sent with a security code.  Please enter this code and continue.";
			  		fs.write(output_path + 'challenge_question.txt', security_question, 'w');
			    	this.die("Security code failure.", 1);
			    });
			 }
		   });
	    }
	    else {
	    	this.click("#ancHavActivationCode");

	    	casper.wait(2000, function() {
				//allow time redirect
		  	});

	    	casper.waitForSelector('#frmValidateOTP', function() {
		    	this.fill('#frmValidateOTP', {
				   	 "usrCtrlOtp$txtActivationCode": security_answers,
				   	 "usrCtrlOtp$txtPassword": password
					}, false);
			});

			casper.then(function() {
				this.echo('submit security code...');
				this.click("#NextButton");
			});
	    }
	  }
	  else{
	 	 this.echo("security code NOT asked.");
	  }
	});

    


}