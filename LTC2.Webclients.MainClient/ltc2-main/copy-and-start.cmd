@echo off

echo copy files...
xcopy dist\*.* ..\..\..\builds\Application\LTC2.Webapps.MainApp\wwwroot\ltc2-main /Y /E

cd ..\..\..\builds\Application\LTC2.Desktopclients.WindowsClient
LTC2.Desktopclients.WindowsClient.exe
cd ..\..\..\sources\LTC2.Webclients.MainClient\ltc2-main
