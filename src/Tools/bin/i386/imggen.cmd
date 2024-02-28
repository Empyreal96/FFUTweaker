@echo off
setlocal
set IMGAPP_ARGS=

if [%1] == [/?] goto Usage
if [%1] == [-?] goto Usage
if [%2] == [] goto Usage
if [%3] == [] goto Usage

set _IMAGEFILE=%1
set IMGAPP_ARGS=%1 %2 %3 +StrictSettingPolicies
shift
shift
shift

if [%1] NEQ [] (
  REM CPU type override
  if [%1] == [x86] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
  if [%1] == [X86] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
  if [%1] == [arm] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
  if [%1] == [ARM] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
  if [%1] == [arm64] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
  if [%1] == [ARM64] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
  if [%1] == [amd64] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
  if [%1] == [AMD64] set IMGAPP_ARGS=%IMGAPP_ARGS% /CPUType:%1 && shift
)

if [%1] NEQ [] (
  if [%2] == [] goto Usage
  set IMGAPP_ARGS=%IMGAPP_ARGS% /OEMCustomizationXML:%1 /OEMVersion:%2
)

REM call ImageApp with the specified parameters
call ImageApp %IMGAPP_ARGS%

if %ERRORLEVEL% neq 0 goto :Error


REM call DevNodeClean

if "%ProgramW6432%"=="" (
   REM run x86
   call DeviceNodeCleanup.x86.exe
) ELSE (
   REM run x64
   call DeviceNodeCleanup.x64.exe
)

echo Image successfully created at %_IMAGEFILE%
exit /b 0


:Usage
echo Usage: ImgGen OutputFile OEMInputXML MSPackageRoot [CPUType] OEMCustomizationXML OEMCustomizationVer
echo    OutputFile............ Required, path to the image to be created. 
echo    OEMInputXML........... Required, path to the OEM Input XML file.
echo    MSPackageRoot......... Required, path to the Microsoft Package Root
echo    [CPUType]              Optional CPU type override [X86, AMD64, ARM or ARM64] 
echo    OEMCustomizationXML    Optional, path to the OEM customization XML file
echo    OEMCustomizationVer    Required if OEMCustomizationXML is specified, version for OEM customization package
echo    [/?].................. Displays this usage string. 
echo    Example:
echo        ImgGen Flash.ffu OEMInput.xml "c:\Program Files (x86)\Windows Phone Kits\8.1\MSPackages" OEMCustomization.XML 8.0.0.1
exit /b 1

:Error
echo "ImageApp %IMGAPP_ARGS%" failed with error %ERRORLEVEL%
exit /b 1
