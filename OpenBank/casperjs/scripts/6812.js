//Bank of America

phantom.injectJs('./scripts/common.js');

function login() {
	sign_in = "https://safe.bankofamerica.com/login/sign-in/signOnScreen.go"
	casper.start(sign_in, function() {
	  this.fill('form[action="/login/sign-in/internal/entry/signOn.go"]', {
	    onlineId: user_id
	  }, true);
	});
	
	casper.waitForSelector('body', function() {
	  this.echo("security question check...");
	  challenge_exists = this.evaluate(function() {
	    return __utils__.exists('[for=tlpvt-challenge-answer]');
	  });
	  
	  if(challenge_exists){
	    this.fill('form[action="/login/sign-in/validateChallengeAnswer.go"]',{
	      challengeQuestionAnswer: security_answers[0]
	    }, true) ;
	  }
	  else{
	 	 this.echo("security question NOT asked.");
	  }
	});

	casper.waitForSelector('body', function() {
	  challenge_exists = this.evaluate(function() {
	    return __utils__.exists('[for=tlpvt-challenge-answer]');
	  });
	  
	  if(challenge_exists){
	  	security_question = this.getHTML('[for=tlpvt-challenge-answer]');
	  	security_question = security_question.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
	  	fs.write(output_path + 'challenge_question.txt', security_question, 'w');
	    this.die("Security question failure.", 1);
	  }
	});

	casper.then(function() {
	  this.fill('form[action="/login/sign-in/validatePassword.go"]', {
	    password: password
	  }, true);
	});
}