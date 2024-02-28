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

if "%WPDKCONTENTROOT%"=="" goto MISSINGPATH
set CERT_PATH=%WPDKCONTENTROOT%\Tools\Certificates


REM
REM Delete Potentially Installed Bad HAL Cert
REM
certutil -delstore -user MY "30 8c e4 36 9a 39 d5 8a 45 40 f9 f8 28 e9 25 97"

REM Add the Root(s) cert to the root store
certutil -addstore Root "%CERT_PATH%\OEM_Root_CA.cer"
certutil -addstore Root "%CERT_PATH%\OEM_Root_CA2.cer"


REM Import private keys for all signing certs
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_Root_CA.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_Intermediate_Cert.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_Intermediate_FFU_Cert.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_Test_PK_Cert_2013.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_Test_Cert_2013.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_HAL_Extension_Test_Cert_2013.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_App_Test_Cert_2013.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_PP_Test_Cert_2013.pfx" NoRoot
certutil -p "" -user -importpfx "%CERT_PATH%\OEM_PPL_Test_Cert_2013.pfx" NoRoot


REM
REM delete the bad cert again after imports
REM
certutil -delstore -user MY "30 8c e4 36 9a 39 d5 8a 45 40 f9 f8 28 e9 25 97"

:SUCCESSEXIT
exit /B 0;

:MISSINGPATH

echo WPDKCONTENTROOT must be set to the root of the WPDK install.  
echo e.g. C:\Program Files(x86)\Windows Phone Kits\v8.1
goto ERROREXIT

:ERROREXIT

exit /B 1;