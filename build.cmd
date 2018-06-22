@ECHO OFF
SETLOCAL
SET SCRIPT_DIR=%~dp0
SET SCRIPT_NAME=%~n0

IF /I [%1] == [install] GOTO install
IF /I [%1] == [compile] GOTO compile
IF /I [%1] == [deps] GOTO deps

:build
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\build\build.proj /fileloggerparameters:logfile=build.log %*
GOTO end

:end
ENDLOCAL
IF NOT %ERRORLEVEL% == 0 EXIT /B 1
