<#
PowerShell script to compile Java sources and create a runnable JAR.
Place this file in `projets\ApplicationJava` and run from PowerShell.
#>

param(
    [switch]$Force
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

Write-Host "Working directory: $PWD"

# Check javac and jar
if (-not (Get-Command javac -ErrorAction SilentlyContinue)) {
    Write-Error "javac not found on PATH. Install JDK and ensure javac is on PATH."
    exit 1
}
if (-not (Get-Command jar -ErrorAction SilentlyContinue)) {
    Write-Error "jar tool not found on PATH. Install JDK and ensure jar is on PATH."
    exit 1
}

$srcFiles = Get-ChildItem -Path . -Recurse -Filter "*.java" | Select-Object -ExpandProperty FullName
if (-not $srcFiles) {
    Write-Error "Aucun fichier .java trouvé dans le répertoire du projet."
    exit 1
}

# Find the class that contains main
$mainFile = $null
foreach ($f in $srcFiles) {
    $content = Get-Content $f -Raw -ErrorAction SilentlyContinue
    if ($content -match 'public\s+static\s+void\s+main\s*\(') {
        $mainFile = $f
        break
    }
}

# Prepare bin
$binDir = Join-Path $scriptDir 'bin'
if (Test-Path $binDir -PathType Container) {
    if ($Force) { Remove-Item -Recurse -Force $binDir }
} 
New-Item -ItemType Directory -Force -Path $binDir | Out-Null

# Compile
Write-Host "Compiling Java sources..."
& javac -d "$binDir" @($srcFiles) 2>&1 | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) {
    Write-Error "Compilation failed. Fix errors above."
    exit 1
}

# Determine main class fully qualified name (if any)
$mainFQN = $null
if ($mainFile) {
    $mainContent = Get-Content $mainFile -Raw
    $pkg = $null
    if ($mainContent -match '^\s*package\s+([a-zA-Z0-9_.]+)\s*;' ) { $pkg = $matches[1] }
    $className = [IO.Path]::GetFileNameWithoutExtension($mainFile)
    if ($pkg) { $mainFQN = "$pkg.$className" } else { $mainFQN = $className }
    Write-Host "Detected main class: $mainFQN"
} else {
    Write-Warning "Aucune méthode main détectée. JAR sera créé sans Main-Class manifest."
}

# Create manifest if main found
$jarName = 'ApplicationJava.jar'
$manifest = $null
if ($mainFQN) {
    $manifest = Join-Path $scriptDir 'manifest.mf'
    @("Manifest-Version: 1.0","Main-Class: $mainFQN","") | Set-Content -NoNewline -Encoding ASCII $manifest
    Write-Host "Creating JAR with manifest (Main-Class: $mainFQN)"
    & jar cfm $jarName $manifest -C "$binDir" .
    Remove-Item $manifest -Force -ErrorAction SilentlyContinue
} else {
    Write-Host "Creating JAR without explicit Main-Class"
    & jar cf $jarName -C "$binDir" .
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Erreur lors de la création du JAR."
    exit 1
}

Write-Host "JAR créé : $scriptDir\$jarName"
Write-Host "Pour exécuter : java -jar $jarName"

exit 0
