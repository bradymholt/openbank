#!/bin/bash

cd ~/dev/OpenBank
echo "git pull..."
git pull
echo "building..."
xbuild OpenBank.sln /p:Configuration=Release
echo "copy files..."
rsync -rvuz --delete ./OpenBank/bin/Release/ bude@geekytidbits.com:openbank
echo "stopping daemon..."
ssh bude@geekytidbits.com 'sudo service openbank stop'
echo "starting daemon..."
rsync ./script/openbank bude@geekytidbits.com:openbank
ssh bude@geekytidbits.com 'sudo cp ~/openbank/openbank /etc/init.d/'
ssh bude@geekytidbits.com 'sudo service openbank start'
echo "Deploy Successful!"
