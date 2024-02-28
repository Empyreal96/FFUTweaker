@echo off
setlocal
set IMGAPP_ARGS=

if [%1] == [/?] goto Usage
if [%1] == [-?] goto Usage
if [%2] == [] goto Usage
if [%3] == [] goto Usage
if [%4] == [] goto Usage
if [%5] == [] goto Usage

set _CUSTOMIZATIONDIR=%1
set IMGAPP_ARGS=%1\Flash.FFU %2 %3 +StrictSettingPolicies +SkipImageCreation
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

REM Customization file and version are required
if [%1] == [] goto Usage
if [%2] == [] goto Usage

set _CUSTOMIZATIONPARAMETER=OEMCustomizationXML
if /i "%~x1"==".ppkg" set _CUSTOMIZATIONPARAMETER=OEMCustomizationPPKG
set IMGAPP_ARGS=%IMGAPP_ARGS% /%_CUSTOMIZATIONPARAMETER%:%1 /OEMVersion:%2

if [%3] NEQ [] goto Usage

REM call ImageApp with the specified parameters
call ImageApp %IMGAPP_ARGS%

if errorlevel 1 goto Error

echo OEM Customizations successfully created at %_CUSTOMIZATIONDIR%
endlocal
exit /b 0


:Usage
echo Usage: CustomizationGen OutputDirectory OEMInputXML MSPackageRoot [CPUType] OEMCustomizationXML OEMCustomizationVer
echo    OutputDirectory....... Required, path to the OEM Customization packages to be created. 
echo    OEMInputXML........... Required, path to the OEM Input XML file.
echo    MSPackageRoot......... Required, path to the Microsoft Package Root
echo    [CPUType]              Optional CPU type override [X86, AMD64, ARM or ARM64] 
echo    OEMCustomizationXML... Required, path to the OEM customization XML file
echo    OEMCustomizationVer... Required, version for OEM customization package
echo    [/?].................. Displays this usage string. 
echo    Example:
echo        CustomizationGen c:\OEMCustomization OEMInput.xml "c:\Program Files (x86)\Windows Phone Kits\8.1\MSPackages" OEMCustomization.XML 8.0.0.1

endlocal
exit /b 1

:Error
echo "ImageApp %IMGAPP_ARGS%" failed with error %ERRORLEVEL%
endlocal
exit /b 1
