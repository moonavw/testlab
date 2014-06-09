@echo off
REM tool for managing service, support: deploy, install/uninstall, start/stop/query, kill/clean
REM require inifile: for list of machines (one line for one machine)
REM require robocopy.exe for copying files
REM require psexec.exe for execute cmd on remote machine
REM require username and password when using psexec.exe

set installutil=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe
set source=AgentService\bin\Release
set servicename=TestLabAgentService
set serviceimg=TestLab.AgentService.exe
set targetFolder=AutomationTools
set inifile=%1
set action=%2
set "args=%~3"
if exist %inifile% (
	FOR /F "eol=; tokens=*" %%i IN (%inifile%) DO (
		echo %%i
		call :%action% %%i %args%
	)
) else (
	call :%action% %inifile% %args%
)
goto :EOF

:deploy
set machine=%1
set destination=\\%machine%\d$\%targetFolder%\%servicename%
robocopy.exe %source% %destination% /S /A-:R /Z /W:3 /NP /XD logs /xf *.InstallLog *.InstallState
goto :eof

:stop
set machine=%1
sc \\%machine% Stop %servicename%
goto :eof

:start
set machine=%1
sc \\%machine% Start %servicename%
goto :eof

:query
set machine=%1
sc \\%machine% Query %servicename%
goto :eof

:kill
set machine=%1
taskkill /IM %serviceimg% /F /S %machine%
goto :eof

:clean
set machine=%1
del \\%machine%\d$\%targetFolder%\%servicename%\logs\*.* /f /q
goto :eof

:install
set machine=%1
set username=%2
set password=%3
psexec \\%machine% -u %username% -p %password% -s -h -i -d "%installutil%" /username=%username% /password=%password% "d:\%targetFolder%\%servicename%\%serviceimg%"
goto :eof

:uninstall
set machine=%1
set username=%2
set password=%3
psexec \\%machine% -u %username% -p %password% -s -h -i -d "%installutil%" /u "d:\%targetFolder%\%servicename%\%serviceimg%"
goto :eof