@echo off
FOR /f "tokens=3" %%G IN ('git status ^|find "On branch"') DO set branch=%%G

:: If we are on the master branch, verify we want to commit and push directly to it
if %branch%==master goto :verify
if %branch%==main goto :verify
goto :commit

:verify
set /P v=You are currently on the master branch, continue [Y/N]? 

if /I "%v%" EQU "Y" goto :commit
if /I "%v%" EQU "N" goto :end

:: If we've made it this far, the input was invalid and we need to try again
goto :verify

:commit
git add .
git commit -a -m "Blindly commiting %date% %time%"
git push
goto :end

:end
pause
exit
