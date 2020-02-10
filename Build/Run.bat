@echo off&setlocal
for %%i in ("%~dp0..") do set "folder=%%~fi"
java -jar Gem.jar %folder%\App\Main.gem
pause