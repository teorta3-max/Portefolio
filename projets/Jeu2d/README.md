Jeu2D - MonoGame DesktopGL (Windows)
===================================

Contenu:
- Jeu2D.sln
- Jeu2D.csproj
- Program.cs
- Game1.cs (le code du jeu)
- Content/ (musique et effets sonores synthétisés, Content.mgcb placeholder)

Installation (Windows):
1. Installer .NET SDK (https://dotnet.microsoft.com/download) - choisissez la version 8.0 ou plus récente.
2. Installer MonoGame (DesktopGL) depuis https://www.monogame.net/downloads/
3. Ouvrir la solution `Jeu2D.sln` dans Visual Studio 2022/2023.
4. Si nécessaire, restaurez les packages NuGet (Visual Studio le proposera automatiquement).
5. Ouvrir le `Content/Content.mgcb` avec MonoGame Pipeline Tool, ajouter les fichiers audio si besoin et build.
6. Lancer le projet (F5). Contrôles: A/D = gauche/droite, Space = saut, LeftShift = dash, K = tir.

Remarque: Les fichiers audio inclus ont été synthétisés automatiquement (boucle d'ambiance et SFX). Vous pouvez les remplacer par vos propres fichiers dans Content/ et rebuild via Pipeline.
