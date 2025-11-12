<#
Deploy helper: find a built JAR and copy it to the project folder so the web link works.
Usage:
  - From the project folder: .\deploy-jar.ps1
  - Or provide a path: .\deploy-jar.ps1 -JarPath "C:\path\to\ApplicationJava.jar"
#>
param(
    [string]$JarPath,
    [switch]$Force
)

$cwd = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $cwd

function CopyJar($src) {
    $dest = Join-Path $cwd 'ApplicationJava.jar'
    if (Test-Path $dest) {
        if ($Force) { Remove-Item $dest -Force }
        else { Write-Host "Destination $dest already exists. Use -Force to overwrite."; return }
    }
    Copy-Item -Path $src -Destination $dest -Force
    Write-Host "Copied: $src -> $dest"
}

if ($JarPath) {
    if (Test-Path $JarPath) { CopyJar $JarPath; exit 0 }
    else { Write-Error "JarPath not found: $JarPath"; exit 1 }
}

# Try common locations
$candidates = @()
$candidates += Get-ChildItem -Path "$cwd\bin" -Recurse -Filter "*.jar" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName
$candidates += Get-ChildItem -Path "$cwd\out" -Recurse -Filter "*.jar" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName
$candidates += Get-ChildItem -Path "$cwd\target" -Recurse -Filter "*.jar" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName
$candidates += Get-ChildItem -Path "$cwd" -Filter "*.jar" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName

if (-not $candidates) {
    Write-Error "No JAR found in common locations. Build the project first or provide -JarPath."
    exit 1
}

# Prefer ApplicationJava.jar or the largest one
$preferred = $candidates | Where-Object { $_ -like '*ApplicationJava*.jar' }
if (-not $preferred) { $preferred = $candidates | Sort-Object { -File (Get-Item $_).Length } -Descending | Select-Object -First 1 }
CopyJar $preferred
exit 0
