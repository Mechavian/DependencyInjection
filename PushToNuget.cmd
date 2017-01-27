@echo off
setlocal enabledelayedexpansion

SET DryRun=1

if "%NugetApiKey%" == "" (
    ECHO "Missing required [NugetApiKey] environment variable"
    GOTO :error
)

if "%NugetExe%" == "" (
    ECHO "Missing required [NugetExe] environment variable"
    GOTO :error
)

if "%PkgVersion%" == "" (
    ECHO "Missing required [PkgVersion] environment variable"
    GOTO :error
)

if "%NugetOutDir%" == "" (
    SET NugetOutDir="%TEMP:"=%\NugetPackages"
)

pushd %~dp0%

CALL :ExecuteCmd %NugetExe% restore Mechavian.Extensions.DependencyInjection.sln
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd msbuild /t:Rebuild /p:Configuration=Release Mechavian.Extensions.DependencyInjection.sln
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd packages\xunit.runner.console.2.1.0\tools\xunit.console.exe test\Mechavian.Extensions.DependencyInjection.Tests\bin\Release\Mechavian.Extensions.DependencyInjection.Tests.dll
IF !ERRORLEVEL! NEQ 0 goto error

if not exist %NugetOutDir% md %NugetOutDir%
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd %NugetExe% pack src\Mechavian.Extensions.DependencyInjection\Mechavian.Extensions.DependencyInjection.csproj -OutputDirectory %NugetOutDir% -Version %PkgVersion% -Symbols -IncludeReferencedProjects
IF !ERRORLEVEL! NEQ 0 goto error

CALL :ExecuteCmd %NugetExe% push "%NugetOutDir:"=%\Mechavian.Extensions.DependencyInjection.%PkgVersion%.nupkg" -ApiKey %NugetApiKey%
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
