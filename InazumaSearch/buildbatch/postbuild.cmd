echo ---- Inazuma Search Build Batch Start ----

REM externalsディレクトリを削除
echo rmdir /S /Q "%TARGETDIR%externals"
rmdir /S /Q "%TARGETDIR%externals"

REM 必要な空ディレクトリの作成
if not exist "%TARGETDIR%plugins" mkdir "%TARGETDIR%plugins"
if not exist "%TARGETDIR%externals\%PLATFORMNAME%" mkdir "%TARGETDIR%externals\%PLATFORMNAME%"

REM pluginsフォルダにテキストを作成
echo type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\このフォルダについて.txt"
type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\このフォルダについて.txt"

REM externalsのコピー
xcopy /y /e "%PROJECTDIR%externals\%PLATFORMNAME%" "%TARGETDIR%externals\%PLATFORMNAME%"

echo ---- Inazuma Search Build Batch End ----