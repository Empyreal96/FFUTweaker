@echo off

SETLOCAL

REM -------------------------------------------------------------------------------------
REM
REM This batch file retrieves preboot crash dump file from the device
REM
REM -------------------------------------------------------------------------------------

echo.

if "%1"==""   goto usage
if "%1"=="/?" goto usage
if "%1"=="-?" goto usage
if "%2"==""   goto usage

set CrashDump=%1\CrashDump\DUMP*.tmp
set Output=%2
set Matches=0

REM
REM Verifying that there is only 1 crash dump file
REM

for /f %%H in ('dir /b /a %CrashDump%') do set /a Matches+=1

if "%Matches%"=="0" (
    echo No crash dumps found. Repeat the test and let device reboot to the home screen.
    goto usage
)

if not "%Matches%"=="1" (
    echo %Matches% Crash dump files found. Repeat the test and initiate only 1 reset.
    goto usage
)

REM
REM Verifying that output directory exists
REM

if not exist %Output% (
    echo Output directory does not exist.
    goto usage
)

REM
REM Removing crash dump attributes and copying it
REM

attrib -s -h %CrashDump%

xcopy %CrashDump% %Output%

goto eof

:usage

echo.
echo This batch file retrieves preboot crash dump file from the device
echo.
echo Usage:
echo.
echo GetDumpFile.bat [Main OS drive letter:] [Output directory]
echo.
echo Example: GetDumpFile.bat F: C:\test

:eof

ENDLOCAL