# Compila AlkaSkin (BepInEx) — requiere BepInEx 5.x instalado en el juego
$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$game = "C:\Program Files (x86)\Steam\steamapps\common\StickFightTheGame"
$depsBep = Join-Path (Split-Path -Parent $here) "StickFightColorCustomizer\Deps\BepInEx.dll"
if (-not (Test-Path $depsBep)) {
    & (Join-Path $here "fetch-bepinex-deps.ps1")
}

$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" 2>$null | Select-Object -First 1
if (-not $msbuild) { $msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" }

& $msbuild (Join-Path $here "StickFightColorCustomizer.BepInEx.csproj") /p:Configuration=Release /v:minimal /nologo /p:StickFightDir="$game"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$plugin = Join-Path $game "BepInEx\plugins\AlkaSkin\AlkaSkin.dll"
if (Test-Path $plugin) {
    Write-Host "OK: BepInEx plugin -> $plugin" -ForegroundColor Green
    Get-Item $plugin | Format-List Name, Length, LastWriteTime
}
