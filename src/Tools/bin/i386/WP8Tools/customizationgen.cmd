@echo off
setlocal
set IMGAPP_ARGS=

if [%1] == [/?] goto Usage
if [%1] == [-?] goto Usage
if [%2] == [] goto Usage
if [%3] == [] goto Usage
if [%4] == [] goto Usage
if [%5] == [] goto Usage

set IMGAPP_ARGS=%1\Flash.FFU %2 %3 +StrictSettingPolicies +SkipImageCreation

set _CUSTOMIZATIONPARAMETER=OEMCustomizationXML
if /i "%~x4"==".ppkg" set _CUSTOMIZATIONPARAMETER=OEMCustomizationPPKG
set IMGAPP_ARGS=%IMGAPP_ARGS% /%_CUSTOMIZATIONPARAMETER%:%4 /OEMVersion:%5

if [%6] NEQ [] goto Usage

REM call ImageApp with the specified parameters
call ImageApp %IMGAPP_ARGS%

if errorlevel 1 goto Error

echo OEM Customizations successfully created
goto End


:Usage
echo Usage: CustomizationGen OutputDirectory OEMInputXML MSPackageRoot OEMCustomizationXML OEMCustomizationVer
echo    OutputDirectory....... Required, path to the OEM Customization packages to be created. 
echo    OEMInputXML........... Required, path to the OEM Input XML file.
echo    MSPackageRoot......... Required, path to the Microsoft Package Root
echo    OEMCustomizationXML... Required, path to the OEM customization XML file
echo    OEMCustomizationVer... Required, version for OEM customization package
echo    [/?].................. Displays this usage string. 
echo    Example:
echo        CustomizationGen c:\OEMCustomization OEMInput.xml "c:\Program Files (x86)\Windows Phone Kits\8.1\MSPackages" OEMCustomization.XML 8.0.0.1

exit /b 1

:Error
echo "ImageApp %IMGAPP_ARGS%" failed with error %ERRORLEVEL%
exit /b 1

:End
endlocal
exit /b 0