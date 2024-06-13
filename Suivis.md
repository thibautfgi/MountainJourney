test map (evalution de la faisibiliter technique une aprem midi) 28/05
premier jet map avec interaction ui = button, center to paris
mise en place du dmc
mise en place de la db via sql
mise en place de test de la db via mysql workbench
ajout de fake data et test des delete en cascade afin de verifier si tt les relations fonctionne
mise en place model api
controller api
ajout d'une fonctionnalite de test de connection a la db dans l'api
mise en place token api
mise en place user controller
optimisation token api
all controler n model
test with postman
doc api
CORS (Cross-Origin Resource Sharing) policy try to fix
test front mapbox place marqueur
test front aceder api
creation maquette   




# DunKy

## Infrastructure

### Configuration des conteneurs (BDD MySQL / Site (API/Front) sur Nginx)
- Configuration des conteneurs pour le projet en utilisant Docker.
- Mise en place de MySQL pour la base de données.
- Configuration du site, incluant l'API et le front, sur un serveur Nginx.

### Docker-compose
- Création et configuration du fichier `docker-compose.yml` pour orchestrer les différents conteneurs.

### Script pour installer .NET
- Rédaction d'un script pour installer l'environnement .NET nécessaire pour le développement de l'API.

### Modification du script SQL
- Adaptation et modification des scripts SQL pour répondre aux besoins spécifiques du projet.

### Default.conf pour la configuration de Nginx
- Création et configuration du fichier `default.conf` pour Nginx afin de gérer les différents hôtes virtuels et les proxys.

### Configuration Nginx
- Configuration de Nginx pour servir le front-end et l'API.
- Mise en place des proxys inversés et des règles de routage.
- Sécurisation et optimisation des performances de Nginx.

### Dates des réalisations
- **Configuration des conteneurs:** 04/06/2024 - 08/06/2024
- **Docker-compose:** 06/06/2024 - 06/06/2024
- **Script pour installer .NET:** 07/06/2024
- **Modification du script SQL:** 08/06/2024
- **Default.conf pour la configuration de Nginx:** 11/06/2024
- **Configuration Nginx:** 06/06/2024 - 111/05/2024

## Développement API

### Compréhension et aide pour la création de l'API avec token
- Étude et implémentation des handlers pour la gestion des tokens.
- Mise en place des endpoints sécurisés utilisant les tokens.

## Développement Front-End

### Pages développées
- Création des différentes pages du site, incluant:
  - **Page de connexion**
  - **Page de profil utilisateur**
  - **Page de bienvenue**
  - **Page principale d'exploration**
- Intégration des fonctionnalités comme la visibilité du mot de passe, la barre de recherche avec icône, et l'animation de texte, la connexion avec le cookie, changement de l'header en fonction de si l'utilisateur est connecté ou non.

## Documentation

### Charte de projet
- Élaboration de la charte de projet définissant les objectifs, les parties prenantes, les délais, et les livrables.

### Aide du DMC (Document de Mise en Conformité)
- Rédaction du document de mise en conformité pour assurer que le projet respecte les normes et les réglementations en vigueur.

### Documentation Docker
- Création de la documentation détaillant l'utilisation de Docker pour le projet, incluant la configuration des conteneurs et l'utilisation de Docker-compose.

### Schéma infrastructure
- Conception d'un schéma détaillé de l'infrastructure du projet montrant l'architecture des conteneurs et leur interaction.

### Dates des réalisations
- **Charte de projet:** 29/05/2024
- **Aide du DMC:** 01/06/2024
- **Documentation Docker:** 10/06/2024
- **Schéma infrastructure:** 10/06/2024





