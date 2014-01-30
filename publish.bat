@ECHO OFF
SETLOCAL
SET NUGET=.nuget\nuget.exe

FOR %%G IN (nuget\*.nupkg) DO (
  %NUGET% push %%G
)