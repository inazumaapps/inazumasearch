echo ---- Inazuma Search Build Batch Start ----

REM �K�v�ȋ�f�B���N�g���̍쐬
if not exist "%TARGETDIR%plugins" mkdir "%TARGETDIR%plugins"
if not exist "%TARGETDIR%externals" mkdir "%TARGETDIR%externals"

REM plugins�t�H���_�Ƀe�L�X�g���쐬
echo type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\���̃t�H���_�ɂ���.txt"
type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\���̃t�H���_�ɂ���.txt"

REM externals�̃R�s�[
xcopy /y /e "%PROJECTDIR%externals" "%TARGETDIR%externals"

echo ---- Inazuma Search Build Batch End ----