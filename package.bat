@ECHO OFF
SETLOCAL

mkdir nuget

ECHO starting with:
dir nuget\. /B
ECHO.

del nuget\. /Q
ECHO.

SET NUGET=.nuget\nuget.exe
SET MCC=MultiCommandConsole\MultiCommandConsole.csproj
SET MCCS=MultiCommandConsole.Services\MultiCommandConsole.Services.csproj

%NUGET% pack %MCC% -OutputDirectory nuget -Prop Configuration=Release
%NUGET% pack %MCCS% -OutputDirectory nuget -Prop Configuration=Release

ECHO.
ECHO ending with:
dir nuget\. /B