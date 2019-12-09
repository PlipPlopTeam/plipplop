@echo off

set filePath=%LOCALAPPDATA%\Unity\Editor\Editor.log

echo =====================
echo Initializing...

FOR /F "TOKENS=1,2 eol=/ DELIMS=/ " %%A IN ('date /t') DO SET mm=%%B
FOR /F "TOKENS=1,2 DELIMS=/ eol=/" %%A IN ('date /t') DO SET dd=%%B
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
echo Building...
"C:\Program Files\Unity\Hub\Editor\2019.2.8f1\Editor\Unity.exe" -batchmode -projectPath . -buildWindows64Player "../AUTOBUILD/%date%/Build.exe" -nographics  -quit

IF ERRORLEVEL 1 (
	echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	echo !!                                 !!
	echo !! ERROR WHILE BUILDING            !!
	echo !! Details in the log file below   !!
	echo !!                                 !!
	echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
)

echo =====================
echo Finished, cleaning up
rd "Assets" /Q
rd "ProjectSettings" /Q
cd ..
del ".TEMP" /Q

REM Tailing the log file
echo =====================
echo Log file: 
for /F "usebackq tokens=* delims=" %%i in (%filePath%.txt) do @echo %%i

echo =====================
pause