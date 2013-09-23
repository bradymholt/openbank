OpenBank
========

A friendly REST service wrapper for OFX Servers.

##Quick Start
1. Download latest release zip file from [https://github.com/bradyholt/OpenBank/releases](https://github.com/bradyholt/OpenBank/releases)  
2. Unzip and run *OpenBank.exe* from command line.  If using mono, run *mono OpenBank.exe* 
3. OpenBank should fire up and now be listening on port 1234, by default.
4. In a browser, navigate to http://localhost:1234/ to ensure OpenBank if running and to get more info.

##Get Transactions Example
To get a list of transactions for a bank account, you'll want to use the **statement** (POST) resource.  The required parameters for this resource are:
- ofx_url
- fid
- org
- user_id
- password
- bank_id
- account_id
- account_type
- date_start
- date_end

If you have a checking account at Chase, you would use the directory at [OFX Home](http://www.ofxhome.com) to get Chase's OFX information
including the ofx_url, fid, and org.  The direct link for Chase's info is [http://www.ofxhome.com/index.php/institution/view/636](http://www.ofxhome.com/index.php/institution/view/636).  
The bank_id is usually your routing number and account_type should be one of the following: CHECKING, SAVING, MONEYMRKT, CREDITCARD, OTHER.  date_start and date_end 
parameters should be in YYYYMMDD format.  account_id is your account number.

Using wget, you can download transactions for your Chase account, for date range 9/1/2013 to 9/15/2013, and save as JSON file with the following command:

    wget --header="Accept: application/json" --post-data="ofx_url=https://ofx.chase.com&fid=10898
      &org=B1&user_id=YOUR_USERNAME_HERE&password=YOUR_PASSWORD_HERE&bank_id=111000614&account_id=816555555
      &account_type=CHECKING&date_start=20130901&date_end=20130915" http://localhost:1234/statement -O statement.json
