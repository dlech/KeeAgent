# Convience script to find msbuild.exe when it is not in $env:PATH
# from https://github.com/microsoft/vswhere/wiki/Find-MSBuild#powershell

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$path = & "$vswhere" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | select-object -first 1
if ($path) {
    & $path $args
}
