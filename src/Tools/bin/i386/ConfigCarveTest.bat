@echo off

SETLOCAL

REM -------------------------------------------------------------------------------------
REM
REM This batch file configures device for the carve out test
REM
REM -------------------------------------------------------------------------------------

set RegKeys=%TEMP%\\ConfigCrashDump.reg

if "%1"==""   goto usage
if "%1"=="/?" goto usage
if "%1"=="-?" goto usage
if "%2"==""   goto usage

set Device=%1

set Platform=%2

REM
REM Assigning device specific cookie address
REM

if "%Platform%"=="8660" (
    set Cookie=2A05F00C
    goto PlatVerified
)

if "%Platform%"=="8960" (
    set Cookie=2A03F00C
    goto PlatVerified
)

echo "%Platform%" is not a supported platform
goto usage

:PlatVerified

REM -------------------------------------------------------------------------------------
REM
REM Verifying that SYSTEM and BCD files exist
REM
REM -------------------------------------------------------------------------------------

if not exist %Device%\Windows\System32\config\SYSTEM (
    echo Could not find %Device%\Windows\System32\config\SYSTEM
    goto usage
)

if not exist %Device%\EFIESP\efi\microsoft\boot\BCD (
    echo Could not find %Device%\EFIESP\efi\microsoft\boot\BCD
    goto usage
)

REM -------------------------------------------------------------------------------------
REM
REM Importing all necessary registry keys
REM
REM -------------------------------------------------------------------------------------

echo Windows Registry Editor Version 5.00                                   > %RegKeys%
echo.                                                                      >> %RegKeys%
echo [HKEY_LOCAL_MACHINE\minwin-system\ControlSet001\Control\CrashControl] >> %RegKeys%
echo "WpAbortKeyScanCode"=dword:00005000                                   >> %RegKeys%
echo "WpDisableDebuggerCheck"=dword:00000001                               >> %RegKeys%
echo "WpAbortTimeout"=dword:00000005                                       >> %RegKeys%
echo "WpAbortKeyName"="<volume down>"                                      >> %RegKeys%
echo "DedicatedDumpFile"="C:\\CrashDump\\DedicatedDump.sys"                >> %RegKeys%
echo "AutoReboot"=dword:00000001                                           >> %RegKeys%
echo "WpSettingsFilePath"="\\DosDevices\\C:\\CrashDump\\WpDmp.settings"    >> %RegKeys%
echo "AlwaysKeepMemoryDump"=dword:00000001                                 >> %RegKeys%
echo "Overwrite"=dword:00000001                                            >> %RegKeys%
echo "WpDisablePrebootCrashDump"=dword:00000000                            >> %RegKeys%
echo "WpMemoryCaptureModeAddr"=dword:%Cookie%                              >> %RegKeys%


reg load hklm\minwin-system %Device%\Windows\System32\config\SYSTEM

reg delete HKLM\minwin-system\ControlSet001\Control\CrashControl /f

reg import %RegKeys%

reg unload hklm\minwin-system

del %RegKeys%

REM -------------------------------------------------------------------------------------
REM
REM Configuring BCD settings
REM
REM -------------------------------------------------------------------------------------

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /delete {012cdeb8-68fe-4fb9-be98-dbf20d98c261} /cleanup

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /create {012cdeb8-68fe-4fb9-be98-dbf20d98c261} /application osloader /d "Preboot Crash Dump Application"

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /set {012cdeb8-68fe-4fb9-be98-dbf20d98c261} device boot

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /set {012cdeb8-68fe-4fb9-be98-dbf20d98c261} inherit {bootloadersettings}

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /set {012cdeb8-68fe-4fb9-be98-dbf20d98c261} path \windows\system32\boot\wpdmp.efi

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /set {012cdeb8-68fe-4fb9-be98-dbf20d98c261} custom:22000501 \WpDmp.settings

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /set {012cdeb8-68fe-4fb9-be98-dbf20d98c261} custom:22000502 \LOGs\WpDmp.log

bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /set {bootmgr} bootsequence {012cdeb8-68fe-4fb9-be98-dbf20d98c261} /addfirst

REM
REM Only 8660 needs to have cookie address set in BCD
REM

if "%Platform%"=="8660" bcdedit /store %Device%\EFIESP\efi\microsoft\boot\BCD /set {012cdeb8-68fe-4fb9-be98-dbf20d98c261} custom:25000500 0x000000002A05F00C

REM
REM Deleting any stale crash dump files
REM

del /F /A /Q %Device%\CrashDump\DUMP*.tmp

goto eof

:usage

echo.
echo This batch file configures device for a carve out test
echo.
echo Usage:
echo.
echo ConfigCarveTest.bat [Main OS drive letter:] [Platform]
echo.
echo Supported platforms:
echo 8660
echo 8960
echo.
echo Example: ConfigCarveTest.bat F: 8660

:eof

ENDLOCAL