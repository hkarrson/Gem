@echo off&setlocal
for %%i in ("%~dp0..") do set "folder=%%~fi"
%folder%\Gem\bin\Debug\netcoreapp3.1\win10-x64\Gem.exe %~dp0Main.gem
pause