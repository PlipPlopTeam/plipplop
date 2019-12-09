@echo off

set filePath=%LOCALAPPDATA%\Unity\Editor\Editor.log
set buildPath="S:\SIG\J002\09_PROJETS\PLIP_PLOP\BUILDS\."

echo =====================
echo Initializing...

FOR /F "TOKENS=1 DELIMS=/" %%A IN ('date /t') DO SET dd=%%A
FOR /F "TOKENS=1,2 eol=/ DELIMS=/ " %%A IN ('date /t') DO SET mm=%%B
FOR /F "TOKENS=2,3 DELIMS=/ " %%A IN ('date /t') DO SET yyyy=%%B
FOR /f "tokens=1-2 delims=/:" %%a IN ('time /t') do (set mytime=%%ah%%b)
SET date=%yyyy%.%mm%.%dd%_%mytime%

echo Creating required folders...
mkdir ".TEMP"
mkdir "AUTOBUILD" 
mkdir "AUTOBUILD/%date%" 
copy "*.sln" ".TEMP/." /Q
copy "*.csproj" ".TEMP/." /Q
cd ".TEMP"
mklink /J "Assets" "../Assets"
mklink /J "ProjectSettings" "../ProjectSettings"

echo =====================
set unityPath="C:\Program Files\Unity\Hub\Editor\2019.2.8f1\Editor\Unity.exe"
IF NOT EXIST %unityPath% set unityPath="C:\Program Files\Unity\Hub\Editor\2019.2.6f1\Editor\Unity.exe"
IF NOT EXIST %unityPath% GOTO NOUNITY
echo Building using %unityPath%...

%unityPath% -projectPath . -batchmode -buildWindows64Player "../AUTOBUILD/%date%/Build.exe" -quit

IF ERRORLEVEL 1 (
	color 0c
	echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	echo !!                                 !!
	echo !! ERROR WHILE BUILDING            !!
	echo !! Details in the log file below   !!
	echo !!                                 !!
	echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	REM Tailing the log file
	echo =====================
	echo Log file: 
	for /F "usebackq tokens=* delims=" %%i in (%filePath%.txt) do @echo %%i
	
	pause 
	exit
)

color 0a
echo =====================
echo Sending to share...
mkdir "%buildPath%/%date%"
xcopy /E /Q "../AUTOBUILD/%date%" "%buildPath%/%date%/."

echo =====================
echo Finished, cleaning up
rd "Assets" /Q
rd "ProjectSettings" /Q
cd ..
del ".TEMP" /Q

echo =====================

pause
exit

:NOUNITY
color 0c
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo !!                                 !!
echo !! ERROR WHILE BUILDING            !!
echo !! Could not find unity install    !!
echo !!                                 !!
echo !! Make sure you have installed    !!
echo !! either 2019.2.8f1 or 2019.2.6f1 !!
echo !! from unity Hub                  !!
echo !!                                 !!
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo.
pause
exit
