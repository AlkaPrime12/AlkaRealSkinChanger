$ErrorActionPreference = "Stop"
$deps = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "..\StickFightColorCustomizer\Deps"
$target = Join-Path $deps "BepInEx.dll"
if (Test-Path $target) {
    Write-Host "BepInEx.dll ya en Deps"
    exit 0
}
$zip = Join-Path $env:TEMP "BepInEx_win_x86_5.4.23.3.zip"
$extract = Join-Path $env:TEMP "BepInEx_extract"
Invoke-WebRequest -Uri "https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.3/BepInEx_win_x86_5.4.23.3.zip" -OutFile $zip
if (Test-Path $extract) { Remove-Item $extract -Recurse -Force }
Expand-Archive $zip $extract -Force
Copy-Item (Join-Path $extract "BepInEx\core\BepInEx.dll") $deps -Force
Write-Host "OK: $target"
