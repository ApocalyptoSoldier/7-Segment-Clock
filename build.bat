@echo off
setlocal EnableDelayedExpansion

REM first find out where the c# compiler is installed (this assumes .net framework 4 is installed
for /f "tokens=3" %%a in ('reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" /v InstallPath ^|findstr /ri "REG_SZ"') do set csc=%%acsc.exe

REM /target:exe is the default, and creates a console app instead of a form
"%csc%" /out:Clock.exe /win32icon:icon.ico /resource:icon.ico /t:winexe Clock.cs