@echo off
setlocal enabledelayedexpansion

SET DryRun=0

if "%PkgVersion%" == "" (
    ECHO "Missing required [PkgVersion] environment variable"
    GOTO :error
)

pushd %~dp0%

CALL :ExecuteCmd dotnet restore
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd dotnet version -f src\Mechavian.Extensions.DependencyInjection\Mechavian.Extensions.DependencyInjection.csproj patch
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd dotnet build -c Release
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd dotnet test -c Release
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd dotnet pack -c Release
IF !ERRORLEVEL! NEQ 0 goto error

popd

::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
goto end

:: Execute command routine that will echo out when error
:ExecuteCmd
setlocal
set _CMD_=%*
echo :: %_CMD_%
if %DryRun% == 1 (
    exit /b %ERRORLEVEL%
)

call %_CMD_%
if "%ERRORLEVEL%" NEQ "0" echo Failed exitCode=%ERRORLEVEL%, command=%_CMD_%
echo.
exit /b %ERRORLEVEL%

:EchoHead
echo ::::::::::::::::::::::::::::::::::::::::::::::::::::::
echo :: %*
echo :: ---------------------------
echo.
exit /b 0

:error
endlocal
echo An error has occurred.
call :exitSetErrorLevel
call :exitFromFunction 2>nul

:exitSetErrorLevel
exit /b 1

:exitFromFunction
()

:end
endlocal
echo Finished successfully.
