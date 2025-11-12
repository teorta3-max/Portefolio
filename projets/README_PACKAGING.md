Packaging helper scripts

This folder contains two helper scripts to build/package the projects so the download links in the portfolio work correctly.

1) ApplicationJava/build-java.ps1
- Usage: open PowerShell in `projets\ApplicationJava` and run:
  ./build-java.ps1
- What it does: compiles .java sources into `bin`, detects the class containing `public static void main`, produces `ApplicationJava.jar` in the same folder and adds a Main-Class manifest if found.
- Requirements: JDK with `javac` and `jar` on PATH.

2) Jeu2D/build-jeu2d.ps1
- Usage: open PowerShell in `projets\Jeu2D` and run:
  ./build-jeu2d.ps1
- What it does: runs `dotnet publish -c Release -r win-x64` to produce a publish folder, then zips it to `Jeu2D.zip`.
- Requirements: .NET SDK (8.0 recommended) and MonoGame dependencies as needed.

After running the scripts, ensure the produced artifacts are present in the paths referenced by the HTML pages:
- `projets\ApplicationJava\ApplicationJava.jar`
- `projets\Jeu2D\Jeu2D.zip`

Then open your site via a local HTTP server (see portfolio page notes) to test downloads.

Helper: quick deploy of a built JAR
---------------------------------
If you build the Java project with an IDE (which may place the JAR in `bin/`, `out/` or `target/`), use the helper script to copy the JAR into the web path expected by the site:

From `projets\ApplicationJava`:

```
./deploy-jar.ps1               # will search common locations and copy the JAR
./deploy-jar.ps1 -JarPath "C:\path\to\ApplicationJava.jar"  # copy a specific file
./deploy-jar.ps1 -Force       # overwrite existing ApplicationJava.jar
```

This keeps the web link `projets/ApplicationJava/ApplicationJava.jar` valid without manual copying.