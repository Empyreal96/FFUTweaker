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

setlocal


REM
REM CreateOemCerts.cmd will not create new certificate, instead, it just
REM install new "OEM Test Certificates" that was created by Microsoft, shipped
REM in AK. So AK should be installed and installoemcerts.cmd called once.
REM Subsequent call of installoemcerts.cmd does nothing.
REM 
REM OEM should make sure %WPDKCONTENTROOT% and %SIGN_OEM" be set properly.
REM

call installoemcerts.cmd

if %ERRORLEVEL% GTR 0 (  
  goto ERROREXIT
) 

exit /B 0; 

:ERROREXIT  
exit /B 1; 