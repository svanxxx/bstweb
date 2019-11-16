#Set-ExecutionPolicy RemoteSigned
#Set-ExecutionPolicy Unrestricted
#Set-ExecutionPolicy bypass

$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$OpenAppPath = "$ScriptDir\vncstarter.exe"

$fileType = (cmd /c "assoc .vnc") 
$fileType = $fileType.Split("=")[-1]  
cmd /c "ftype $fileType=""$OpenAppPath"" ""%1""" 

Write-Host "Press any key to continue ..."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")