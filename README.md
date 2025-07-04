# Présentation du projet

MédiLab est une solution logicielle conçue pour la gestion médicale, orientée microservices et développée dans le cadre de la formation Développeur Backend .NET. L’application répond à des besoins métiers réels : gestion des patients, centralisation des notes médicales, et évaluation automatisée du risque de diabète à partir des données patients et de leurs historiques.

L’objectif principal de MédiLab est de fournir une plateforme moderne, sécurisée et évolutive permettant :

- Le suivi des patients et de leurs informations médicales
- La gestion des notes de santé par les professionnels
- L’évaluation automatique du risque de diabète, basée sur l’analyse de notes et de critères médicaux
- Une séparation claire des responsabilités grâce à une architecture microservices
  
![2025-07-04 16_11_01-- MédiLabo Solutions](https://github.com/user-attachments/assets/8d8b9f2a-d187-4ac0-a1a4-897ecdfbf4d8)

## Présentation de l’architecture (microservices, technologies, etc.)

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
