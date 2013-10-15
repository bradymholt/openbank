phantom.injectJs('./scripts/common.js');

function login() {
	sign_in = "https://safe.bankofamerica.com/login/sign-in/signOnScreen.go"
	casper.start(sign_in, function() {
	  this.fill('form[action="/login/sign-in/internal/entry/signOn.go"]', {
	    onlineId: user_id
	  }, true);
	});

	casper.then(function() {
	  for(var i=0;i< security_answers.length;i++){
		  challenge_exists = this.evaluate(function() {
		    return __utils__.exists('[for=tlpvt-challenge-answer]');
		  });
		  
		  if(challenge_exists){
		    security_question = this.getHTML('[for=tlpvt-challenge-answer]') ;
		    this.echo(security_question);
		    this.fill('form[action="/login/sign-in/validateChallengeAnswer.go"]',{
		      challengeQuestionAnswer: security_answers[i]
		    }, true) ;
		  }
		  else{
		  	break;
		  }
	  }
	});

	casper.then(function() {
	  this.fill('form[action="/login/sign-in/validatePassword.go"]', {
	    password: password
	  }, true);
	});
 }