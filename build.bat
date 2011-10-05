@echo off
if "%1" == "" goto Usage
goto Build

:Usage
echo usage: build [TARGET]
echo where: target = one of "test", "release", "clean"
goto End

:Build
%systemroot%\Microsoft.net\framework\v4.0.30319\MSBuild.exe Nate.msbuild /t:%1

:End
