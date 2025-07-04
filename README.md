# Présentation du projet

MédiLabo Solutions est une solution logicielle conçue pour la gestion médicale. L'objectif principal est de fournir une plateforme moderne, sécurisée et évolutive permettant :

- Le suivi des patients et de leurs informations médicales
- La gestion des notes de santé par les professionnels
- L’évaluation automatique du risque de diabète, basée sur l’analyse de notes et de critères médicaux
- Une séparation claire des responsabilités grâce à une architecture microservices
  
![2025-07-04 16_11_01-- MédiLabo Solutions](https://github.com/user-attachments/assets/8d8b9f2a-d187-4ac0-a1a4-897ecdfbf4d8)

## Présentation de l’architecture

L’architecture de MédiLab repose sur plusieurs microservices indépendants, chacun dédié à une responsabilité spécifique :

- PatientsAPI : Service REST en .NET Core, responsable de la gestion CRUD des patients (stockage SQL Server, EF Core).
- NotesAPI : Microservice pour la gestion des notes médicales des patients, basé sur .NET et MongoDB pour le stockage NoSQL.
- DiabeteRiskAPI : Microservice d’évaluation du risque de diabète, qui centralise les données patients et notes, et utilise Elasticsearch pour l’indexation et l’analyse.
- Frontend : Application ASP.NET MVC qui sert de passerelle utilisateur, consommant les différents microservices via une API Gateway (non détaillé ici).

## Technologies principales utilisées :

- .NET 9, ASP.NET Core
- Entity Framework Core (pour SQL Server)
- MongoDB (pour les notes)
- Elasticsearch (pour l’indexation et l’analyse du texte médical)
- Docker (tous les microservices sont dockerisés)
- Authentification JWT et sécurisation des communications
- Swagger / OpenAPI pour la documentation automatique des APIs

## Recommandations Green Code 🌱

Pour réduire l’empreinte environnementale du projet et optimiser la consommation de ressources, voici quelques bonnes pratiques à appliquer :

- Éviter le surprovisionnement : Allouer uniquement les ressources nécessaires dans les conteneurs Docker (CPU, mémoire). Adapter les Dockerfiles et les fichiers de configuration.
- Minification et bundling : Utiliser la minification des fichiers JS/CSS et le bundling pour réduire la taille des assets côté client (voir la documentation ASP.NET Core sur le bundling/minification).
- Nettoyage du code : Supprimer les dépendances et packages inutilisés dans tous les microservices.
- Limiter les logs en production : Réduire le niveau de logs pour éviter une consommation excessive de disque et de bande passante.
- Requêtes optimisées : Privilégier les requêtes filtrées et paginées pour éviter de charger inutilement des données volumineuses en mémoire.
- Indexation Elasticsearch : S’assurer que l’indexation n’est lancée que si nécessaire, éviter de réindexer systématiquement toutes les notes.
- Lazy loading : Charger les composants lourds, images et scripts à la demande.
- Images optimisées : Utiliser des images compressées et adaptées à la résolution de l’écran.
