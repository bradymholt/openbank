phantom.injectJs('./scripts/6805.js');

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

    var data = decode_base64(base64contents) ;
    fs.write(output_path + 'statement.qfx', data, 'w');
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
