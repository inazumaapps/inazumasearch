echo ---- Inazuma Search Build Batch Start ----

REM externalsディレクトリを削除
echo rmdir /S /Q "%TARGETDIR%externals"
rmdir /S /Q "%TARGETDIR%externals"

REM 必要な空ディレクトリの作成
if not exist "%TARGETDIR%plugins" mkdir "%TARGETDIR%plugins"
if not exist "%TARGETDIR%res" mkdir "%TARGETDIR%res"
if not exist "%TARGETDIR%externals\%PLATFORMNAME%" mkdir "%TARGETDIR%externals\%PLATFORMNAME%"

REM pluginsフォルダにテキストを作成
echo type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\このフォルダについて.txt"
type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\このフォルダについて.txt"

REM resのコピー
xcopy /y /e "%PROJECTDIR%res" "%TARGETDIR%res"

REM externalsのコピー
xcopy /y /e "%PROJECTDIR%externals\%PLATFORMNAME%" "%TARGETDIR%externals\%PLATFORMNAME%"

REM Visual C++ 再頒布可能パッケージ DLLのコピー
xcopy /y /e "%~dp0\%PLATFORMNAME%\vcruntime2019\msvcp140.dll" "%TARGETDIR%"
xcopy /y /e "%~dp0\%PLATFORMNAME%\vcruntime2019\vcruntime140.dll" "%TARGETDIR%"
if %PLATFORMNAME% == x64 xcopy /y /e "%~dp0\%PLATFORMNAME%\vcruntime2019\vcruntime140_1.dll" "%TARGETDIR%"

echo ---- Inazuma Search Build Batch End ----