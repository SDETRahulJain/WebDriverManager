Currently while performing execution either on TFS or on local, there is dependency of chrome browser version which is creating dependency of keeping both version same. Due to system upgrades, version mismatch occurs frequently. 

We need to create one solution which will overcome this dependency. Here I am sharing webdrivermanager service:-

1.It will remove the dependency of installed chrome browser on system.
2.During run time based on conditions, it will download CFT. 
3.W.r.t CFT it will download chromedriver and utilize it for further execution. 
4.It will have handling like download when new version available only else utilize existing binaries on system. 
5.In case of new download, it will take care of memory management via flushing out old binaries once new binaries got successfully download.
