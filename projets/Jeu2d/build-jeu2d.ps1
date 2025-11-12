<#
PowerShell script to build the MonoGame/.NET project and create a ZIP for distribution.
Place this file in `projets\Jeu2D` and run from PowerShell.
#>
param(
    [switch]$Force
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir
Write-Host "Working directory: $PWD"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet CLI not found on PATH. Install .NET SDK (8.0 recommended)."
    exit 1
}

# Build Release
$buildCmd = 'dotnet publish -c Release -r win-x64 --self-contained true -o "' + "$scriptDir\publish" + '" .\Jeu2D.csproj'
Write-Host "Building the project (Release) as self-contained Windows app..."
$build = Invoke-Expression $buildCmd 2>&1
$build | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed. Check output above."
    exit 1
}

# Create ZIP
$zipPath = Join-Path $scriptDir 'Jeu2D.zip'
if (Test-Path $zipPath) {
    if ($Force) { Remove-Item $zipPath -Force }
    else { Write-Host "Existing $zipPath will be overwritten (use -Force to remove first)." }
}

Write-Host "Creating ZIP archive..."
if (Test-Path "$scriptDir\publish") {
    Compress-Archive -Path "$scriptDir\publish\*" -DestinationPath $zipPath -Force
    Write-Host "Archive created: $zipPath"
} else {
    Write-Error "Publish output not found."
    exit 1
}

Write-Host "Done. Place $zipPath in your web server path so the link on the site can download it."
exit 0
