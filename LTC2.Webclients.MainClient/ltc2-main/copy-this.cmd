@echo off

echo copy files...
xcopy dist\*.* ..\..\..\builds\Application\LTC2.Webapps.MainApp\wwwroot\ltc2-main /Y /E
