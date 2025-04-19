@echo off
cls
echo Fazendo limpeza ...
echo.
if exist "bin" rd /s /q "bin" || rmdir /s /q "bin"
if exist "obj" rd /s /q "obj" || rmdir /s /q "obj"

timeout /t 1 /nobreak >nul

if exist "SpooferHWID.exe" del /f /q "SpooferHWID.exe"

timeout /t 1 /nobreak >nul

echo Compilando SpooferHWID...
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:SelfContained=true /p:IncludeNativeLibrariesForSelfExtract=true /p:WarningLevel=0 /nologo

if exist "bin\Release\net9.0\win-x64\publish\SpooferHWID.exe" (
    copy /Y "bin\Release\net9.0\win-x64\publish\SpooferHWID.exe" ".\" >nul
    echo.
    echo ========= COMPILACAO CONCLUIDA =========
    echo SpooferHWID.exe copiado pra pasta atual.
    echo ========================================
    echo.
) else (
    echo ERRO: COMPILACAO FALHOU!
    pause & exit /b 1
)

pause