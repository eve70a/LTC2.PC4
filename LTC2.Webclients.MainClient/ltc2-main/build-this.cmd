@echo off

rem npm install

echo npm build...
npm run build

echo copy files...
xcopy dist\*.* ..\..\..\builds\Application\LTC2.Webapps.MainApp\wwwroot\ltc2-main /Y /E
