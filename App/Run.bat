@echo off&setlocal
for %%i in ("%~dp0..") do set "folder=%%~fi"
python %folder%\Gem\Gem.py %~dp0Main.gem
pause