var args = require('system').args;

phantom.casperPath = args[1]; 
phantom.injectJs(phantom.casperPath + '/bin/bootstrap.js');

var casper = require('casper').create({ 
	verbose: true, 
	logLevel: 'debug'
});


var online_id = casper.cli.get("user_id");
var online_password = casper.cli.get("password");
var account_id = casper.cli.get("account_id");
var from_date = casper.cli.get("from_date");
var to_date = casper.cli.get("to_date");
var security_answers = casper.cli.get("security_answers").split();
var output_path = casper.cli.get("output_path") + '/';

casper.on('load.finished', function(resource) {
	var fileNamePrefix = new Date().getTime();
	
    if (casper.cli.has("debug")) {
		this.capture(output_path + '/' + fileNamePrefix + '-screenshot.png', {
			top: 100,
			left: 100,
			width: 1000,
			height: 800
		});
		
		var href = this.evaluate(function() {
			return document.location.href;
		});
	
		var innerHTML = this.evaluate(function(){
			return document.body.innerHTML;
		});
		
		innerHTML = '<!-- ' + href + ' -->\n\n' + innerHTML;
		
		fs.write(output_path + '/' + fileNamePrefix + '-content.html', innerHTML, 'w');
	}
});

sign_in = "https://safe.bankofamerica.com/login/sign-in/signOnScreen.go"
casper.start(sign_in, function() {
  this.fill('form[action="/login/sign-in/internal/entry/signOn.go"]', {
    onlineId: online_id
  }, true);
});

casper.then(function() {
  //for(var i=0;i< security_answers.length;i++){
	  challenge_exists = this.evaluate(function() {
	    return __utils__.exists('[for=tlpvt-challenge-answer]');
	  });
	  
	  if(challenge_exists){
	    security_question = this.getHTML('[for=tlpvt-challenge-answer]') ;
	    this.echo(security_question);
	    this.fill('form[action="/login/sign-in/validateChallengeAnswer.go"]',{
	      challengeQuestionAnswer: security_answers[0]
	    }, true) ;
	  }
	  //else{
	  //	break;
	  //}
  //}
});

casper.then(function() {
  this.fill('form[action="/login/sign-in/validatePassword.go"]', {
    password: online_password
  }, true);
}) ;

casper.thenClick('a[id=\"'+account_id+'\"]') ;

casper.thenClick('a[name="export_trans_nav_top"]') ;

casper.then(function() {
    //http://code.google.com/p/phantomjs/issues/detail?id=52
    var download_form_action = this.evaluate(function() {
        return document.querySelector('form[name="transactionDownloadForm"]').getAttribute('action');
    });

	console.log(download_form_action);
    var base64contents = this.base64encode(download_form_action, 'POST', {
        selectedTransPeriod: '',
        downloadTransactionType: 'customRange',
        "searchBean.timeFrameStartDate": from_date,
        "searchBean.timeFrameEndDate": to_date,
        formatType: 'qfx'
    });

    var data = decode_base64(base64contents) ;
    require('fs').write(output_path + 'statement.qfx', data, 'w');
});

casper.run();

//decode_base64 from http://stackoverflow.com/questions/2820249/base64-
//encoding-and-decoding-in-client-side-javascript
function decode_base64(s) {
  var e={},i,k,v=[],r='',w=String.fromCharCode;
  var n=[[65,91],[97,123],[48,58],[43,44],[47,48]];
  for(z in n){for(i=n[z][0];i<n[z][1];i++){v.push(w(i));}}
  for(i=0;i<64;i++){e[v[i]]=i;}
  for(i=0;i<s.length;i+=72){
  var b=0,c,x,l=0,o=s.substring(i,i+72);
    for(x=0;x<o.length;x++){
      c=e[o.charAt(x)];b=(b<<6)+c;l+=6;
      while(l>=8){r+=w((b>>>(l-=8))%256);}
    }
  } 
  return r;
}
