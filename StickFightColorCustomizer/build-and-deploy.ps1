# Compila AlkaSkin (MelonLoader) y copia a Mods
$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" 2>$null | Select-Object -First 1
if (-not $msbuild) { $msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" }

& $msbuild (Join-Path $here "StickFightColorCustomizer.csproj") /p:Configuration=Release /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$gameMods = "C:\Program Files (x86)\Steam\steamapps\common\StickFightTheGame\Mods\AlkaRealSkinChanger.dll"
if (Test-Path $gameMods) {
    Write-Host "OK: MelonLoader DLL -> $gameMods" -ForegroundColor Green
    Get-Item $gameMods | Format-List Name, Length, LastWriteTime
}
