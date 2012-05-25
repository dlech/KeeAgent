copy ".\KeeAgent\bin\Release\PageantSharp.dll" ".\KeeAgent\"
rmdir /s /q ".\KeeAgent\bin"
rmdir /s /q ".\KeeAgent\obj"
"c:\Program Files (x86)\KeePass Password Safe 2\keepass.exe" --plgx-create %CD%\KeeAgent --plgx-prereq-net:4.0 --plgx-prereq-os:Windows
pause
del ".\KeeAgent\PageantSharp.dll"