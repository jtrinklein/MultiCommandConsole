@ECHO OFF
SETLOCAL

mkdir nuget

ECHO starting with:
dir nuget\. /B
ECHO.

del nuget\. /Q
ECHO.

SET NUGET=.nuget\nuget.exe
SET OP=MultiCommandConsole\MultiCommandConsole.csproj

%NUGET% pack %OP% -OutputDirectory nuget -Prop Configuration=Release

ECHO.
ECHO ending with:
dir nuget\. /B