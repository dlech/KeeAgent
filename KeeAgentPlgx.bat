copy ".\KeeAgent\bin\Release\PageantSharp.dll" ".\KeeAgent\"
copy ".\PreBuild\bin\Release\PreBuild.exe" ".\KeeAgent\"
rmdir /s /q ".\KeeAgent\bin"
rmdir /s /q ".\KeeAgent\obj"
"c:\Program Files (x86)\KeePass Password Safe 2\keepass.exe" --plgx-create %CD%\KeeAgent --plgx-prereq-net:4.0 --plgx-prereq-os:Windows --plgx-build-pre:"cmd /c """{PLGX_TEMP_DIR}PreBuild.exe""" {PLGX_TEMP_DIR}"
pause
del ".\KeeAgent\PageantSharp.dll"
del ".\KeeAgent\PreBuild.exe"