# Compila MelonLoader + BepInEx y despliega al juego
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "=== AlkaSkin MelonLoader ===" -ForegroundColor Cyan
& (Join-Path $root "StickFightColorCustomizer\build-and-deploy.ps1")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "=== AlkaSkin BepInEx ===" -ForegroundColor Cyan
& (Join-Path $root "StickFightColorCustomizer.BepInEx\build-bepinex.ps1")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "Listo. Usa SOLO uno de los loaders en el juego (ver INSTALL-LOADERS.md)." -ForegroundColor Green
