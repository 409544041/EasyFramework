# simple-frame
welcome, this framework is based on my years of project experience summed up.
i hope everyone like it and easy to use,cheers!

2016-09-08 01:22 -create ssh key

2016-09-08 01:51 -summary:
	
1.set user name and email
	$ git config --global user.name "humingx"
  	$ git config --global user.email "humingx@yeah.net"

2.create ssh key 
	$ ssh-keygen -t rsa -C "humingx@yeah.net"
	if don't need password continuous input enter 3 times
	finally,get 2 files: id_rsa and id_rsa.pub

3.login Github and add ssh id_rsa.pub
	copy id_rsa.pub content to Settings>>SSH and GPG keys>>New SSH key
	Title can input authorized_keys

4.test ssh whether successful
	$ ssh -T git@github.com
	if you see :
		The authenticity of host 'github.com (207.97.227.239)' can't be established.
   	 	RSA key fingerprint is 16:27:ac:a5:76:28:2d:36:63:1b:56:4d:eb:df:a6:48.
    		Are you sure you want to continue connecting (yes/no)?
	select: yes
	if you see your user name behind Hi,success!

5.modified .git/config file
	url = https://github.com/...
	change to
	url = git@github.com/...

2016-10-22 14:18 -create unity project ver5.4.1f1

2016-10-22 14:27 -add uniRx plugin ver5.5.0

2016-11-26 16:02 create submodule : 
	-1.remove uniRx plugin ver5.5.0
	-2.create easy plugins repository
	-3.git submodule add  git@github.com:chaolunner/EasyPlugins.git Assets/Plugins
	-4.add uniRx plugin to Plugins folder
