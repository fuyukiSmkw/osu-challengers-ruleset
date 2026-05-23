@echo off
setlocal EnableDelayedExpansion

:: ===== Configuration =====
set "RULESET_NAME=osu.Game.Rulesets.oCrs.dll"

echo.
echo ============================================
echo osu!Challengers Ruleset Uninstaller
echo ============================================
echo.

:: ===== Find osu! folder =====
set "INI_FILE=%AppData%\osu\storage.ini"
if exist "%INI_FILE%" (
    for /f "usebackq tokens=1,* delims==" %%A in (`findstr /i "^FullPath" "%INI_FILE%"`) do (
        set "VAL=%%B"
        set "VAL=!VAL:"=!" & REM remove quotes
        for /f "tokens=* delims= " %%C in ("!VAL!") do set "OSU_DATA_PATH=%%C"
    )
    if defined OSU_DATA_PATH (
        echo Found migrated osu folder path: !OSU_DATA_PATH!
    )
)
if not defined OSU_DATA_PATH (
    set "OSU_DATA_PATH=%AppData%\osu"
    if not exist "!OSU_DATA_PATH!" (
        echo [Info] Folder !OSU_DATA_PATH! does not exist.
        echo osu! lazer seems not installed or already removed. Nothing to do.
        goto :End
    )
    echo Using default osu folder path: !OSU_DATA_PATH!
)

:: ===== Check rulesets folder and target file =====
set "RULESET_DIR=%OSU_DATA_PATH%\rulesets"
set "TARGET_FILE=%RULESET_DIR%\%RULESET_NAME%"

if exist "%TARGET_FILE%" (
    echo.
    echo Found ruleset file: %TARGET_FILE%
    echo Removing...
    
    del "%TARGET_FILE%"
    
    if exist "%TARGET_FILE%" (
        echo [Error] Failed to delete the file. 
        echo Try running this script as Administrator or check if osu! lazer is still running.
        pause>nul|set/p =Press any key to quit...
        exit /b 1
    ) else (
        echo.
        echo Uninstallation complete. %RULESET_NAME% has been removed.
    )
) else (
    echo.
    echo [Info] Ruleset file not found in: %RULESET_DIR%
    echo Nothing to do.
)

:End
echo.
pause>nul|set/p =Press any key to quit...
exit /b 0