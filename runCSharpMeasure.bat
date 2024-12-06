@echo off
echo Starting the process...
echo Cleaning up old files that might ruin the process...
call cd output
for /f "delims=" %%a in ('.\timecmd cleanup.bat') do set cleanup_time=%%a
call cd ..
echo Cleaning up done...

echo Creating the Circom file...
for /f "delims=" %%a in ('.\timecmd dotnet run') do set dotnet_build_time=%%a
call cd output
echo Done...
echo.
echo ======= Times =======
echo Cleanup Time: %cleanup_time%
echo Dotnet Build Time: %dotnet_build_time%
echo ====================
echo.