Lancer le projet Java depuis la source

1) Installez le JDK (Java 11+).
2) Ouvrez PowerShell dans ce dossier.
3) Exécutez :
   ./run-jar-from-source.ps1
   (ou ./run-jar-from-source.ps1 -RunOnly si vous avez déjà ApplicationJava.jar)

Le script compile les sources, crée ApplicationJava.jar et l'exécute.
Si des erreurs de compilation surviennent, lisez la sortie pour corriger les classes manquantes.
