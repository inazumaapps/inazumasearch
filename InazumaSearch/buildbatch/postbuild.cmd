echo ---- Inazuma Search Build Batch Start ----

REM 必要な空ディレクトリの作成
if not exist "%TARGETDIR%plugins" mkdir "%TARGETDIR%plugins"
if not exist "%TARGETDIR%externals" mkdir "%TARGETDIR%externals"

REM pluginsフォルダにテキストを作成
echo type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\このフォルダについて.txt"
type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\このフォルダについて.txt"

REM externalsのコピー
xcopy /y /e "%PROJECTDIR%externals" "%TARGETDIR%externals"

echo ---- Inazuma Search Build Batch End ----