phantom.injectJs('./scripts/common.js');

function login() {
	sign_in = "https://www.wellsfargo.com"
	casper.start(sign_in, function() {
	  this.fill('form#frmSignon', {
	    userid: user_id,
	    password: password
	  }, true);
	});
}