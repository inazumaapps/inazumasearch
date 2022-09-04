echo ---- Inazuma Search Build Batch Start ----

REM externals�f�B���N�g�����폜
echo rmdir /S /Q "%TARGETDIR%externals"
rmdir /S /Q "%TARGETDIR%externals"

REM �K�v�ȋ�f�B���N�g���̍쐬
if not exist "%TARGETDIR%plugins" mkdir "%TARGETDIR%plugins"
if not exist "%TARGETDIR%res" mkdir "%TARGETDIR%res"
if not exist "%TARGETDIR%externals\%PLATFORMNAME%" mkdir "%TARGETDIR%externals\%PLATFORMNAME%"

REM plugins�t�H���_�Ƀe�L�X�g���쐬
echo type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\���̃t�H���_�ɂ���.txt"
type %~dp0\_plugins_folder_guide.txt > "%TARGETDIR%plugins\���̃t�H���_�ɂ���.txt"

REM res�̃R�s�[
xcopy /y /e "%PROJECTDIR%res" "%TARGETDIR%res"

REM externals�̃R�s�[
xcopy /y /e "%PROJECTDIR%externals\%PLATFORMNAME%" "%TARGETDIR%externals\%PLATFORMNAME%"

REM Visual C++ �ĔЕz�\�p�b�P�[�W DLL�̃R�s�[
xcopy /y /e "%~dp0\%PLATFORMNAME%\vcruntime2019\msvcp140.dll" "%TARGETDIR%"
xcopy /y /e "%~dp0\%PLATFORMNAME%\vcruntime2019\vcruntime140.dll" "%TARGETDIR%"
if %PLATFORMNAME% == x64 xcopy /y /e "%~dp0\%PLATFORMNAME%\vcruntime2019\vcruntime140_1.dll" "%TARGETDIR%"

echo ---- Inazuma Search Build Batch End ----