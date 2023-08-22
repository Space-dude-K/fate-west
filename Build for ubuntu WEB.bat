SET sln="E:\work\Projects\Fate\wc3-fate-west\wc3-fate-west.sln"
SET targetFolder=E:\work\Projects\Fate\wc3-fate-west\Publish\
SET cfgsDir=E:\work\Projects\Fate\wc3-fate-west\Build cfgs\

echo off

del /s /q "%targetFolder%*.*"

rem dotnet build ProjectName.csproj --runtime ubuntu.xx.xx-x64
dotnet publish %sln% -c Release -o %targetFolder% -r ubuntu.20.04-x64

ROBOCOPY /IS "%cfgsDir% " "%targetFolder% "

pause