<#
Compile and run Java project from sources (simple helper).
Usage: run-jar-from-source.ps1 [-RunOnly]
- Without -RunOnly: compiles sources and builds ApplicationJava.jar
- With -RunOnly: runs existing ApplicationJava.jar if present
#>
param(
    [switch]$RunOnly
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

if (-not (Get-Command java -ErrorAction SilentlyContinue)) { Write-Error "Java (java) not found on PATH. Install JRE/JDK."; exit 1 }

if ($RunOnly) {
    if (-not (Test-Path 'ApplicationJava.jar')) { Write-Error "ApplicationJava.jar not found. Build first or remove -RunOnly."; exit 1 }
    Write-Host "Running ApplicationJava.jar..."
    java -jar ApplicationJava.jar
    exit 0
}

if (-not (Get-Command javac -ErrorAction SilentlyContinue)) { Write-Error "javac not found on PATH. Install JDK."; exit 1 }

Write-Host "Compiling Java sources..."
if (-not (Test-Path bin)) { New-Item -ItemType Directory -Path bin | Out-Null }
$src = Get-ChildItem -Recurse -Filter *.java | Select-Object -ExpandProperty FullName
if (-not $src) { Write-Error "Aucun fichier .java trouvé."; exit 1 }
javac -d bin $src
if ($LASTEXITCODE -ne 0) { Write-Error "Compilation échouée."; exit 1 }

# Detect main class
$mainFile = Get-ChildItem -Recurse -Filter *.java | Where-Object { (Get-Content $_ -Raw) -match 'public\s+static\s+void\s+main\s*\(' } | Select-Object -First 1
$mainFQN = $null
if ($mainFile) {
    $content = Get-Content $mainFile -Raw
    if ($content -match 'package\s+([a-zA-Z0-9_.]+)\s*;') { $pkg = $matches[1] } else { $pkg = $null }
    $className = [System.IO.Path]::GetFileNameWithoutExtension($mainFile.FullName)
    if ($pkg) { $mainFQN = "$pkg.$className" } else { $mainFQN = $className }
    Write-Host "Detected main class: $mainFQN"
}

Write-Host "Creating JAR..."
if ($mainFQN) { jar cfe ApplicationJava.jar $mainFQN -C bin . } else { jar cf ApplicationJava.jar -C bin . }
if ($LASTEXITCODE -ne 0) { Write-Error "Erreur lors de la création du JAR."; exit 1 }

Write-Host "Built ApplicationJava.jar"
Write-Host "Running..."
java -jar ApplicationJava.jar

exit 0
