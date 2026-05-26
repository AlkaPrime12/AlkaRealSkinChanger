# Copia el DLL compilado a Mods (cerrá Stick Fight antes)
$ErrorActionPreference = "Stop"
$src = Join-Path $PSScriptRoot "bin\Release\StickFightColorCustomizer.dll"
$dst = "C:\Program Files (x86)\Steam\steamapps\common\StickFightTheGame\Mods\StickFightColorCustomizer.dll"
$backup = Join-Path $PSScriptRoot "StickFightColorCustomizer-INSTALAR.dll"

if (-not (Test-Path $src)) {
    Write-Host "Compilá primero: .\build-and-deploy.ps1" -ForegroundColor Red
    exit 1
}

Copy-Item $src $backup -Force
Write-Host "Backup en escritorio del proyecto: $backup" -ForegroundColor Cyan

try {
    Copy-Item $src $dst -Force
    Write-Host "OK instalado en Mods" -ForegroundColor Green
    Get-Item $dst | Format-List Name, Length, LastWriteTime
} catch {
    Write-Host "NO se pudo copiar (¿juego abierto?). Copiá manualmente:" -ForegroundColor Yellow
    Write-Host "  Desde: $src"
    Write-Host "  Hacia: $dst"
    Write-Host "  O usá: $backup"
}
