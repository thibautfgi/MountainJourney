# TODO List - Mountain Journey

## Infrastructure
### Fait
- [x] Configuration des conteneurs (BDD MySQL / Site (API/Front) sur Nginx)
- [x] Création du fichier `docker-compose.yml`
- [x] Script pour installer .NET
- [x] Modification du script SQL
- [x] Création et configuration de `default.conf` pour Nginx
- [x] Configuration de Nginx pour servir le front-end et l'API

### À Faire
- [ ] Mise en place des sauvegardes automatiques pour la BDD 
- [ ] Ajouter la surveillance et les alertes (ex. via Prometheus/Grafana)
- [ ] Tester la scalabilité de l'infrastructure

## Développement API
### Fait
- [x] Création des handlers pour l'API
- [x] Mise en place de l'authentification via token
- [x] Documentation de l'API (ex. via Swagger)
- [x] Tests unitaires pour les endpoints principaux

### À Faire
- [x] Ajouter des endpoints pour les fonctionnalités manquantes
- [ ] Optimiser les requêtes SQL pour améliorer les performances
- [ ] Mise en place des tests de charge et de performance
- [?] Implémenter des fonctionnalités de sécurité supplémentaires (ex. rate limiting)

## Front-end du site
### Fait
- [x] Page d'accueil
- [x] Page de profil utilisateur
- [x] Page de connexion et création de compte
- [x] Page de bienvenue avec animation
- [x] Configuration de la barre de recherche et intégration du design

### À Faire
- [ ] Finaliser le design responsive pour toutes les pages
- [ ] Intégrer les fonctionnalités interactives (ex. cartes, filtres de recherche)
- [ ] Ajouter les animations et transitions manquantes
- [ ] Tests utilisateurs pour améliorer l'UX/UI


## Documentation
### Fait
- [x] Maquette du site
- [x] Charte de projet
- [x] Documentation Docker
- [x] Documentation de l'API
- [x] Schéma de l'infrastructure
- [x] MCD
- [x] Trello
- [x] Screen Postman (voir les requêtes API)

### À Faire
- [ ] Maintenir la documentation à jour avec les nouvelles fonctionnalités, ainsi que de l'améliorer

## Divers
### Fait
- [x] Configuration initiale du projet
- [x] Réunions de suivi et de planification
- [x] Mise en place de l'intégration continue

### À Faire
- [ ] Planification des futures itérations et versions
- [ ] Suivi des bugs 
