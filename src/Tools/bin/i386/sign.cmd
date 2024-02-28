@REM
@REM Copyright (c) Microsoft Corporation.  All rights reserved.
@REM
@REM
@REM Use of this sample source code is subject to the terms of the Microsoft
@REM license agreement under which you licensed this sample source code. If
@REM you did not accept the terms of the license agreement, you are not
@REM authorized to use this sample source code. For the terms of the license,
@REM please see the license agreement between you and Microsoft or, if applicable,
@REM see the LICENSE.RTF on your install media or the root of your tools installation.
@REM THE SAMPLE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
@REM
@echo off

goto START

:ERROR_USAGE
@echo Unrecognized command
set ERROR_CODE=1
goto :USAGE

:USAGE
@echo.
@echo Signs the exe/dll provided at the command-line.
@echo.
@echo.
@echo sign [/-?] filename [filename2] [filename3]
@echo.
@echo usage:
@echo.
@echo sign [-wscv ^| -hal ^| -msapp ^| -app ^| -app2 ^| -app3 ^| -msappx ^| -appx2 ^| -appx3 ^| -pp ^| -ppl ^| -pk ^| -ext ^| -uap1 ^| -uap2 ^| -uap3 ^| -pkg] [-ph] ^<file1^> ^<file2^> [..]  -- signs file1, file2, etc.
@echo.
@echo Signing related switches
@echo.
@echo -wscv     sign using default WP EKU for Authenticode signing. This is the default value
@echo.
@echo -hal      sign using WP HAL EKU
@echo.
@echo -msapp    sign using WP EKU for 1st Party MS Apps. Cannot be combined with -oem
@echo.
@echo -app2     sign WP app for 2nd Party. Cannot be combined with -oem
@echo.
@echo -app3     sign WP app for 3rd Party. Cannot be combined with -oem
@echo.
@echo -msappx   sign 1st Party(MS) Appx Apps
@echo.
@echo -appx2    sign 2nd Party Appx Apps
@echo.
@echo -appx3    sign 3rd Party Appx Apps
@echo.
@echo -ph       sign using page hashes
@echo.
@echo -pk       sign using pregenerated OEM test PK certificate.
@echo.
@echo -app      sign WP app for 2nd Party. Can only be used in -oem is specified
@echo.
@echo -ext      sign WP Extensibility. Sign DLLs for WP Extensibility scenarios.
@echo.
@echo -uap1     sign 1st party (MS) UAP apps
@echo.
@echo -uap2     sign 2nd party UAP apps
@echo.
@echo -uap3     sign 3rd party UAP apps
@echo.
@echo -pkg      sign package
@echo.
@REM
@REM  Do not broadcast -oem or -wpb options, but these options do exist
@REM
@REM -oem      sign using OEM certificates generated using makeoemcerts.cmd. Can be combined with -wscv -hal -app -ext.
@REM
@REM -wpb      sign using new certs for WPB. Can be combined with -wscv -wscvpp -wscvppl -wscvtcbpp -wscvtcbppl -msapp -app2 -app3
@REM
@REM -testonly sign only using test signatures when running under razzle tool set
@REM
@REM -nosigninfo suppress generation of signinfo files if applicable
@REM
@REM -platman  sign platform manifest files with p7 signature
@echo.
@echo Examples:
@echo.
@echo      sign.cmd file.dll - sign file.dll with the default WP EKU
@echo.
@echo      sign.cmd -hal halext.dll - sign hallext.dll with the WP HAL EKU
@echo.
@echo      sign.cmd -ph file.dll - sign file.dll with default WP EKU and page hashes
@echo.
@echo      sign.cmd -pk file.dll - sign file.dll with pregenerated OEM test Platform Key (PK) certificate
@echo.

goto :EOF

:START

REM Before we do anything, check for rsaenh.dll and warn the user
if exist rsaenh.dll (
@echo ---- WARNING ----
@echo It looks like you are running this command from your flat release folder.
@echo If the signing fails, try "cd .." and run it again.
@echo ---- WARNING ----
)
setlocal

REM Cause phone lab builds to use the windows build signing infrastructure rather than signtool directly
if defined RAZZLETOOLPATH (
    REM Set this to 1 in order to enable resign removal support on phone.
    set GENERATE_SIGNINFO_FILES=1
) else (
    set GENERATE_SIGNINFO_FILES=
)

if defined GENERATE_SIGNINFO_FILES_OVERRIDE (
    set GENERATE_SIGNINFO_FILES=
)

set PAGE_HASH=

REM If null to internal microsoft defaults, slightly complicated compare since variable can have quotes and/or spaces which confuses the if statement
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN=/a /u 1.3.6.1.4.1.311.76.5.10 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256 /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.4167089.4524209"

REM Now check the HAL signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_HAL%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_HAL=/a /u 1.3.6.1.4.1.311.76.5.20 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.914282.15961069"

REM check the MSAPP signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_MSAPP%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_MSAPP=/a /u 1.3.6.1.4.1.311.76.5.100 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.13332913.12970957"

REM check the APP2 signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_APP2%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_APP2=/a /u 1.3.6.1.4.1.311.76.5.200 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.10947806.4174759"

REM check the APP3 signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_APP3%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_APP3=/a /u 1.3.6.1.4.1.311.76.5.300 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.3398013.13891304"



REM check the MSAPPX signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_MSAPPX%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_MSAPPX=/a /u 1.3.6.1.4.1.311.76.5.100 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.5263787.14645125"

REM check the APPX2 signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_APPX2%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_APPX2=/a /u 1.3.6.1.4.1.311.76.5.200 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.4032570.5175205"

REM check the APPX3 signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_APPX3%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_APPX3=/a /u 1.3.6.1.4.1.311.76.5.300 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.5599124.2478086"


REM check the UAP1 signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_UAP1%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_UAP1=/a /u 1.3.6.1.4.1.311.76.5.100 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256 /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.1849285.5821617"

REM check the UAP2 signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_UAP2%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_UAP2=/a /u 1.3.6.1.4.1.311.76.5.200 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256 /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.11735367.14840284"

REM check the UAP3 signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_UAP3%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_UAP3=/a /u 1.3.6.1.4.1.311.76.5.300 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256 /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.7439081.16496653"

REM check the PKG signature string
set SIGN_NULL=1
for %%i in (%SIGNTOOL_SIGN_PKG%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" set SIGNTOOL_SIGN_PKG=%SIGNTOOL_SIGN%

REM By default use the standard cert
set CERTIFICATE=%SIGNTOOL_SIGN%

:GETPARAMS

if "%~1"==""   goto USAGE
if "%~1"=="/?" goto USAGE
if "%~1"=="-?" goto USAGE
if "%~1"=="-help" goto USAGE

rem Parse parameters
if /I "%~1"=="-tcb" (
    echo Sign.CMD Ignoring legacy argument %1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/tcb" (
    echo Sign.CMD Ignoring legacy argument %1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/ph" (
    set PAGE_HASH=/ph
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-ph" (
    set PAGE_HASH=/ph
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/wscv" (
    set CERTIFICATEFLAG=wscv
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-wscv" (
    set CERTIFICATEFLAG=wscv
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-wscvpp" (
    set CERTIFICATEFLAG=wscvpp
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-wscvppl" (
    set CERTIFICATEFLAG=wscvppl
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-wscvtcbpp" (
    set CERTIFICATEFLAG=wscvtcbpp
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-wscvtcbppl" (
    set CERTIFICATEFLAG=wscvtcbppl
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/hal" (
    set CERTIFICATEFLAG=hal
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-hal" (
    set CERTIFICATEFLAG=hal
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/pk" (
    set CERTIFICATEFLAG=pk
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-pk" (
    set CERTIFICATEFLAG=pk
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/msapp" (
    set CERTIFICATEFLAG=msapp
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-msapp" (
    set CERTIFICATEFLAG=msapp
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/msappx" (
    set CERTIFICATEFLAG=msappx
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-msappx" (
    set CERTIFICATEFLAG=msappx
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/oem" (
    set SIGN_OEM=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-oem" (
    set SIGN_OEM=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/wpb" (
    set SIGN_WPB=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-wpb" (
    set SIGN_WPB=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/app" (
    set CERTIFICATEFLAG=app
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-app" (
    set CERTIFICATEFLAG=app
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/app2" (
    set CERTIFICATEFLAG=app2
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-app2" (
    set CERTIFICATEFLAG=app2
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/app3" (
    set CERTIFICATEFLAG=app3
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-app3" (
    set CERTIFICATEFLAG=app3
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/appx2" (
    set CERTIFICATEFLAG=appx2
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-appx2" (
    set CERTIFICATEFLAG=appx2
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/appx3" (
    set CERTIFICATEFLAG=appx3
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-appx3" (
    set CERTIFICATEFLAG=appx3
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/pp" (
    set CERTIFICATEFLAG=pp
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-pp" (
    set CERTIFICATEFLAG=pp
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/ppl" (
    set CERTIFICATEFLAG=ppl
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-ppl" (
    set CERTIFICATEFLAG=ppl
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/ext" (
    set CERTIFICATEFLAG=ext
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-ext" (
    set CERTIFICATEFLAG=ext
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/uap1" (
    set CERTIFICATEFLAG=uap1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-uap1" (
    set CERTIFICATEFLAG=uap1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/uap2" (
    set CERTIFICATEFLAG=uap2
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-uap2" (
    set CERTIFICATEFLAG=uap2
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/uap3" (
    set CERTIFICATEFLAG=uap3
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-uap3" (
    set CERTIFICATEFLAG=uap3
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/pkg" (
    set CERTIFICATEFLAG=pkg
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-pkg" (
    set CERTIFICATEFLAG=pkg
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/testonly" (
    set TEST_ONLY_SIGNATURE=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-testonly" (
    set TEST_ONLY_SIGNATURE=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/test" (
    set TEST_ONLY_SIGNATURE=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-test" (
    set TEST_ONLY_SIGNATURE=1
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/nosigninfo" (
    set SIGNINFO_GENERATION=/nosigninfo
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-nosigninfo" (
    set SIGNINFO_GENERATION=/nosigninfo
    shift
    goto :GETPARAMS
)
if /I "%~1"=="/platman" (
    set CERTIFICATEFLAG=platman
    shift
    goto :GETPARAMS
)
if /I "%~1"=="-platman" (
    set CERTIFICATEFLAG=platman
    shift
    goto :GETPARAMS
)


set FILELIST=

if "%SIGN_OEM%"=="1" goto :UseOEMTestCertificates
if "%SIGN_WPB%"=="1" goto :UseWPBCerts

if "%CERTIFICATEFLAG%"=="wscv" set CERTIFICATE=%SIGNTOOL_SIGN%
if "%CERTIFICATEFLAG%"=="wscvpp" goto USAGE
if "%CERTIFICATEFLAG%"=="wscvppl" goto USAGE
if "%CERTIFICATEFLAG%"=="wscvtcbpp" goto USAGE
if "%CERTIFICATEFLAG%"=="wscvtcbppl" goto USAGE
if "%CERTIFICATEFLAG%"=="hal" set CERTIFICATE=%SIGNTOOL_SIGN_HAL%
if "%CERTIFICATEFLAG%"=="msapp" set CERTIFICATE=%SIGNTOOL_SIGN_MSAPP%
if "%CERTIFICATEFLAG%"=="app2" set CERTIFICATE=%SIGNTOOL_SIGN_APP2%
if "%CERTIFICATEFLAG%"=="app3" set CERTIFICATE=%SIGNTOOL_SIGN_APP3%
if "%CERTIFICATEFLAG%"=="msappx" set CERTIFICATE=%SIGNTOOL_SIGN_MSAPPX%
if "%CERTIFICATEFLAG%"=="appx2" set CERTIFICATE=%SIGNTOOL_SIGN_APPX2%
if "%CERTIFICATEFLAG%"=="appx3" set CERTIFICATE=%SIGNTOOL_SIGN_APPX3%
if "%CERTIFICATEFLAG%"=="uap1" set CERTIFICATE=%SIGNTOOL_SIGN_UAP1%
if "%CERTIFICATEFLAG%"=="uap2" set CERTIFICATE=%SIGNTOOL_SIGN_UAP2%
if "%CERTIFICATEFLAG%"=="uap3" set CERTIFICATE=%SIGNTOOL_SIGN_UAP3%
if "%CERTIFICATEFLAG%"=="pkg" set CERTIFICATE=%SIGNTOOL_SIGN_PKG%

if "%CERTIFICATEFLAG%"=="app" goto USAGE
if "%CERTIFICATEFLAG%"=="ext" goto USAGE
goto :SIGNLOOP


:UseOEMTestCertificates

REM Ensure the crosscert variables are both set and give error messages if wrong
if defined CROSS_CERT_ISSUER (
    if not defined CROSS_CERT_SUBJECTNAME (
        echo Must set both CROSS_CERT_ISSUER and CROSS_CERT_SUBJECTNAME
        set ERRORLEVEL=1
        goto :SIGN_REPORT
    )
) else (
    if defined CROSS_CERT_SUBJECTNAME (
        echo Must set both CROSS_CERT_ISSUER and CROSS_CERT_SUBJECTNAME
        set ERRORLEVEL=1
        goto :SIGN_REPORT
    )
)

REM If null to internal microsoft defaults, slightly complicated compare since variable can have quotes and/or spaces which confuses the if statement
REM Further complicated by the support for IoT cross cert signing

set SIGN_NULL=1
for %%i in (%SIGNTOOL_OEM_SIGN%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" (
    if defined CROSS_CERT_ISSUER (
        set SIGNTOOL_OEM_SIGN=/s my /i "%CROSS_CERT_ISSUER%" /n "%CROSS_CERT_SUBJECTNAME%" /fd SHA256
    ) else (
        set SIGNTOOL_OEM_SIGN=/a /s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM Test Cert 2013 (TEST ONLY)" /fd SHA256
    )
)

set SIGN_NULL=1
for %%i in (%SIGNTOOL_OEM_SIGN_HAL%) do if not "%%i"=="" set SIGN_NULL=0

if "%SIGN_NULL%"=="1" (
    set SIGNTOOL_OEM_SIGN_HAL=/a /s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM HAL Extension Test Cert 2013 (TEST ONLY)" /fd SHA256
)

REM reset default to OEM cert
set CERTIFICATE=%SIGNTOOL_OEM_SIGN%

REM If running in the windows build lab do not enable timestamps
if defined RAZZLETOOLPATH (
    set SIGN_WITH_TIMESTAMP=0
) else (
    if not "%SIGN_WITH_TIMESTAMP%"=="0" set SIGN_WITH_TIMESTAMP=1
)

if "%CERTIFICATEFLAG%"=="wscv" set CERTIFICATE=%SIGNTOOL_OEM_SIGN%
if "%CERTIFICATEFLAG%"=="hal" set CERTIFICATE=%SIGNTOOL_OEM_SIGN_HAL%
if "%CERTIFICATEFLAG%"=="app" set CERTIFICATE=/s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM App Test Cert 2013 (TEST ONLY)" /fd SHA256
REM
REM Legacy/old "ext" is translated as "Windows Phone OEM Test Cert 2013 (TEST ONLY)"
REM
if "%CERTIFICATEFLAG%"=="ext" set CERTIFICATE=/s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM Test Cert 2013 (TEST ONLY)" /fd SHA256

if "%CERTIFICATEFLAG%"=="pp" set CERTIFICATE=/s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM PP Test Cert 2013 (TEST ONLY)" /fd SHA256
if "%CERTIFICATEFLAG%"=="ppl" set CERTIFICATE=/s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM PPL Test Cert 2013 (TEST ONLY)" /fd SHA256
if "%CERTIFICATEFLAG%"=="pk" set CERTIFICATE=/s my /i "Windows Phone Intermediate FFU Cert 2013" /n "Windows Phone OEM Test Platform Key Cert 2013 (TEST ONLY)" /fd SHA256
if "%CERTIFICATEFLAG%"=="msapp" goto USAGE
if "%CERTIFICATEFLAG%"=="app2" goto USAGE
if "%CERTIFICATEFLAG%"=="app3" goto USAGE
if "%CERTIFICATEFLAG%"=="msappx" goto USAGE
if "%CERTIFICATEFLAG%"=="appx2" goto USAGE
if "%CERTIFICATEFLAG%"=="appx3" goto USAGE
REM the pkg scenario for OEM is the same as default/wscv scenario
if "%CERTIFICATEFLAG%"=="pkg" set CERTIFICATE=/s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM Test Cert 2013 (TEST ONLY)" /fd SHA256
goto :SIGNLOOP

:UseWPBCerts
REM IMG2PKG uses WPB certificates as part of resign

set CERTIFICATE=/s my /i "Windows Phone Intermediate 2013" /n "Windows Phone OEM Test Cert 2013 (TEST ONLY)" /fd SHA256
REM If running in the windows build lab do not enable timestamps
if defined RAZZLETOOLPATH (
    set SIGN_WITH_TIMESTAMP=0
) else (
    if not "%SIGN_WITH_TIMESTAMP%"=="0" set SIGN_WITH_TIMESTAMP=1
)

if "%CERTIFICATEFLAG%"=="wscv" set CERTIFICATE=/a /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.4167089.4524209"
if "%CERTIFICATEFLAG%"=="wscvpp" set CERTIFICATE=/a /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.12544760.14163807"
if "%CERTIFICATEFLAG%"=="wscvppl" set CERTIFICATE=/a /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.6217864.7387975"
if "%CERTIFICATEFLAG%"=="wscvtcbpp" set CERTIFICATE=/a /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.9112938.3721623"
if "%CERTIFICATEFLAG%"=="wscvtcbppl" set CERTIFICATE=/a /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.7834976.7919331"
if "%CERTIFICATEFLAG%"=="hal" goto USAGE
if "%CERTIFICATEFLAG%"=="app" goto USAGE
if "%CERTIFICATEFLAG%"=="ext" goto USAGE
if "%CERTIFICATEFLAG%"=="msapp" set CERTIFICATE=/a /u 1.3.6.1.4.1.311.76.5.100 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.13332913.12970957"
if "%CERTIFICATEFLAG%"=="msappx" set CERTIFICATE=/a /u 1.3.6.1.4.1.311.76.5.100 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.5263787.14645125"
if "%CERTIFICATEFLAG%"=="app2" set CERTIFICATE=/a /u 1.3.6.1.4.1.311.76.5.200 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.10947806.4174759"
if "%CERTIFICATEFLAG%"=="appx2" set CERTIFICATE=/a /u 1.3.6.1.4.1.311.76.5.200 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.4032570.5175205"
if "%CERTIFICATEFLAG%"=="app3" set CERTIFICATE=/a /u 1.3.6.1.4.1.311.76.5.300 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.3398013.13891304"
if "%CERTIFICATEFLAG%"=="appx3" set CERTIFICATE=/a /u 1.3.6.1.4.1.311.76.5.300 /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /n "Microsoft Corporation" /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.5599124.2478086"
REM the pkg scenario for WPB is the same as default/wscv scenario
if "%CERTIFICATEFLAG%"=="pkg" set CERTIFICATE=/a /r "Microsoft Testing Root Certificate Authority 2010" /fd SHA256  /s my /c "1.3.6.1.4.1.311.21.8.7587021.751874.11030412.6202749.3702260.207.4167089.4524209"
goto :SIGNLOOP

rem Sign files specified at the command-line.

:SIGNLOOP

    if "%~1"=="" goto :FILELISTCOMPLETE
    if "%GENERATE_SIGNINFO_FILES%"=="1" (
        set FILELIST=%FILELIST% -f "%~1"
    ) else if "%SIGN_USE_SIGNER%"=="1" (
        set FILELIST=%FILELIST% -f "%~1"
    ) else (
        set FILELIST=%FILELIST% "%~1"
    )
    shift

    goto :SIGNLOOP

:FILELISTCOMPLETE

if "%SIGN_WITH_TIMESTAMP%"=="1" set TIMESERVER=/t http://timestamp.verisign.com/scripts/timestamp.dll

REM If in the Msft dev environment make sure we use the proper version of signtool.exe
REM If not found then find one in the users path
if exist %CORESYSTEM_TOOLS%\x86\signtool.exe set SIGNTOOL_EXE=%CORESYSTEM_TOOLS%\x86\signtool.exe
if not exist %CORESYSTEM_TOOLS%\x86\signtool.exe set SIGNTOOL_EXE=signtool.exe

if "%GENERATE_SIGNINFO_FILES%"=="1" (
    goto :SIGNER_SIGNING
) else if "%SIGN_USE_SIGNER%"=="1" (
    goto :SIGNER_SIGNING
) else (
    echo %SIGNTOOL_EXE% sign /v %Certificate% %PAGE_HASH% %TIMESERVER% %TESTFILEIDARG% %FILELIST%
    %SIGNTOOL_EXE% sign /v %Certificate% %PAGE_HASH% %TIMESERVER% %TESTFILEIDARG% %FILELIST%
)


:SIGN_REPORT

if "%ERRORLEVEL%"=="0" (
    @echo signed: %FILELIST%
) else (
    @echo %SIGNTOOL_EXE% : fatal error : Signing failed with %ERRORLEVEL% on %*
)
:END

endlocal & set RC=%ERRORLEVEL%
echo Sign.Cmd RC=%RC%
exit /B %RC%

:SIGNER_SIGNING

if "%ENABLE_PRS_SIGN_DEVICE_PRODUCTION%"=="1" (
    if "%ENABLE_PRS_SIGN_DEVICE_FLIGHT%"=="1" (
        echo ERROR: Cannot have both ENABLE_PRS_SIGN_DEVICE_PRODUCTION and ENABLE_PRS_SIGN_DEVICE_FLIGHT set in the POD.
        set ERROR_CODE=1
        goto :END
    )
)

REM If nothing is passed as a CERTIFICATEFLAG then assume the default scenario of wsvc
if "%CERTIFICATEFLAG%"=="" (
	if defined PAGE_HASH (
		set SCENARIO_NAME=WscvWpPageHash	
	) else (
		set SCENARIO_NAME=WscvWpFileHash	
	)
) 

REM Convert from phone scenario name to windows scenario name
if "%CERTIFICATEFLAG%"=="wscv" (
	if defined PAGE_HASH (
		set SCENARIO_NAME=WscvWpPageHash	
	) else (
		set SCENARIO_NAME=WscvWpFileHash	
	)
) 

REM WPB translations that would be needed for Img2Pkg to function properly with resign
REM wscvtcbpp -> ProtectedProcessLightWPFileHash
REM wscvtcbppl ->TCBProtectedProcessLightWPFileHash

if "%CERTIFICATEFLAG%"=="wscvpp" goto ERROR_USAGE
if "%CERTIFICATEFLAG%"=="wscvppl" goto ERROR_USAGE
if "%CERTIFICATEFLAG%"=="wscvtcbpp" goto ERROR_USAGE
if "%CERTIFICATEFLAG%"=="wscvtcbppl" goto ERROR_USAGE

if "%CERTIFICATEFLAG%"=="hal" set SCENARIO_NAME=HalWP

if "%CERTIFICATEFLAG%"=="msapp" set SCENARIO_NAME=UapFirstParty
if "%CERTIFICATEFLAG%"=="app2" set SCENARIO_NAME=UapSecondParty
if "%CERTIFICATEFLAG%"=="app3" set SCENARIO_NAME=UapThirdParty
if "%CERTIFICATEFLAG%"=="msappx" set SCENARIO_NAME=UapFirstParty
if "%CERTIFICATEFLAG%"=="appx2" set SCENARIO_NAME=UapSecondParty
if "%CERTIFICATEFLAG%"=="appx3" set SCENARIO_NAME=UapThirdParty
if "%CERTIFICATEFLAG%"=="uap1" set SCENARIO_NAME=UapFirstParty
if "%CERTIFICATEFLAG%"=="uap2" set SCENARIO_NAME=UapSecondParty
if "%CERTIFICATEFLAG%"=="uap3" set SCENARIO_NAME=UapThirdParty
if "%CERTIFICATEFLAG%"=="pkg" set SCENARIO_NAME=WindowsSystemComponentDualSigned
if "%CERTIFICATEFLAG%"=="platman" set SCENARIO_NAME=PlatformManifest

REM Translate the OEM usage to Windows Phone OEM scenario directly for signing of OEM.
REM This should work for both -oem and -oem -pkg usages
if "%SIGN_OEM%"=="1" set SCENARIO_NAME=WpOem

if "%CERTIFICATEFLAG%"=="app" goto USAGE
if "%CERTIFICATEFLAG%"=="ext" goto USAGE

if "%TEST_ONLY_SIGNATURE%"=="1" (
    REM Test signing
    set SIGNER_SIG_TYPE=-type TestSignOnly
) ELSE if "%ENABLE_PRS_SIGN_DEVICE_PRODUCTION%"=="1" (
    REM Production certificates
    set SIGNER_SIG_TYPE=-type ReleaseSignAlways
) ELSE if "%ENABLE_PRS_SIGN_DEVICE_FLIGHT%"=="1" (
    SET SIGNER_SIG_TYPE=-type FlightSignAlways

    IF "%ENABLE_PRS_SIGN_DEVICE_PACKAGES%"=="1" (
        IF "%CERTIFICATEFLAG%"=="pkg" (
            SET SIGNER_SIG_TYPE=-type ReleaseSignAlways
        )
    )
) ELSE if "%ENABLE_PRS_SIGN_DEVICE%"=="1" (
    REM Preproduction certificates
    REM Signer.exe uses the build config to determine if the signing is preproduction or production prs signign
    set SIGNER_SIG_TYPE=-type ReleaseSignAlways
) ELSE (
    REM Test signing
    set SIGNER_SIG_TYPE=-type TestSignOnly
)

SETLOCAL EnableDelayedExpansion

REM UrtRun.cmd will not update the env if it has been set.  MsBuild on phone sets these for its needs.
REM We clear them in a SETLOCAL to allow UrtRun to establish the proper values for signer.exe
SET COMPLUS_DEFAULTVERSION=
SET COMPLUS_INSTALLROOT=
SET COMPLUS_NOGUIFROMSHIM=
SET COMPLUS_ONLYUSELATESTCLR=
SET COMPLUS_VERSION=

echo %RazzleToolPath%\urtrun.cmd 4.0 signer.exe authenticode %SIGNER_SIG_TYPE% %SIGNINFO_GENERATION% -s %SCENARIO_NAME% %FILELIST%
%RazzleToolPath%\urtrun.cmd 4.0 signer.exe authenticode %SIGNER_SIG_TYPE% %SIGNINFO_GENERATION% -s %SCENARIO_NAME% %FILELIST%
ENDLOCAL

goto :SIGN_REPORT
