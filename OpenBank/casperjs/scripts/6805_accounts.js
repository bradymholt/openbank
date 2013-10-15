var args = require('system').args;

phantom.casperPath = args[1]; 
phantom.injectJs(phantom.casperPath + '/bin/bootstrap.js');

var casper = require('casper').create({ 
	verbose: true, 
	logLevel: 'debug'
});

var online_id = casper.cli.get("user_id");
var online_password = casper.cli.get("password");
var from_date = casper.cli.get("from_date");
var to_date = casper.cli.get("to_date");
var security_answers = casper.cli.get("security_answers").split();
var output_path = casper.cli.get("output_path") + '/';

sign_in = "https://safe.bankofamerica.com/login/sign-in/signOnScreen.go"
casper.start(sign_in, function() {
  this.fill('form[action="/login/sign-in/internal/entry/signOn.go"]', {
    onlineId: online_id
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
    password: online_password
  }, true);
}) ;

casper.then(function() {
  var accounts = this.evaluate(function(){
     links = document.querySelectorAll('.account-row .image-account a');
     return Array.prototype.map.call(links, function(e) {
        return { description: e.getAttribute('id'), account_id: e.getAttribute('id'), account_type: "CHECKING" };
     });
  });

  require('fs').write(output_path + 'accounts.json', JSON.stringify(accounts), 'w');
});

casper.run();
