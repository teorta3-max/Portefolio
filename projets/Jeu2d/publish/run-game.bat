@echo off
REM Run the game and write console output to jeu-log.txt in this folder.
set LOGFILE=%~dp0jeu-log.txt
n
"%~dp0Jeu2D.exe" > "%LOGFILE%" 2>&1
echo Game exited. Log saved to %LOGFILE%
pause
