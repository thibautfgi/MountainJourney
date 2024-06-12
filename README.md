<h1 align="center">MountainJourney</h1>

<p align="center">
  <img src="/SITE_MJ/Front/Tools/Image/logo.png" alt="MountainJourney Logo" width="200">
</p>

## Description

MountainJourney est une application web pour gérer et afficher des informations sur les randonnées en montagne. Elle inclut une API pour gérer les utilisateurs et les données des randonnées, ainsi qu'un frontend pour l'affichage de ces informations.

## Prérequis

- Docker
- Docker Compose
- Postman

## Installation

Clonez le dépôt et naviguez dans le répertoire du projet :

```bash
git clone <https://github.com/DuunKy/MontainJourney.git>
cd MontainJourney


``` 

## Lancer l'application localement
### Lancer l'API localement
Pour lancer l'API en local, exécutez la commande suivante :

```bash
dotnet run --project SITE_MJ/API_MJ/
```

## Utilisation avec Postman
Vous pouvez tester les endpoints de l'API en utilisant Postman avec le lien suivant :

[Workspace Postman](https://blue-comet-541359.postman.co/workspace/Mountain-Journey~ef3aa6a7-6af4-474c-9a98-4e77433ef51b/overview)

## Jeton d'API
Utilisez le jeton d'API suivant pour l'authentification :

```bash
abcdef123456
```


>**Attention:**
Le token Mapbox est caché dans un fichier .gitignore. Pour tout accès à ce projet et demande de clef, veuillez contacter un administrateur.

## Lancer l'application avec Docker Compose
Pour lancer l'application avec Docker Compose, exécutez la commande suivante :

```bash
docker-compose up --build
```
Cette commande va :



## Arrêter les services
Pour arrêter les services Docker, utilisez la commande :

```bash
docker-compose down
```

> ℹ️ **Info:** Dans la documentation tout est expliqué pour le Docker.

>**Attention:** Il se peut que git modifie la séquence de fin de ligne afin d'éviter les sh éxecutable, histoire de sécurité. Si il ne trouve pas votre .sh il se peut que vous devez modifier l'encodage CRLF -> LF.

## Auteurs
 - ThibautFgi
 - DunKy

