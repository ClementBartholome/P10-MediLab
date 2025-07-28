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
- Frontend : Application ASP.NET MVC qui sert de passerelle utilisateur, consommant les différents microservices via une API Gateway Ocelot (non détaillé ici).

## Technologies principales utilisées 

- .NET 9, ASP.NET Core
- SQL Server / Entity Framework Core (pour les données des patients)
- MongoDB (pour les notes)
- Elasticsearch (pour l’indexation et l’analyse des notes)
- Docker (tous les microservices sont dockerisés)
- Authentification JWT et sécurisation des communications
- Swagger pour la documentation des APIs

## Recommandations Green Code 🌱

Pour réduire l’empreinte environnementale du projet et optimiser la consommation de ressources, voici quelques bonnes pratiques à appliquer :

### 🐳 Optimisation de l'infrastructure

#### Conteneurs Docker
- **Dimensionnement précis** : Allouer uniquement les ressources nécessaires (CPU, mémoire)
- **Images légères** : Utiliser des images Alpine Linux ou des Dockerfiles multi-stage
- *Exemples : Docker stats, cAdvisor pour le monitoring*

#### Hébergement responsable
- **Hébergeurs verts** : Choisir des providers alimentés en énergie renouvelable
- **CDN écologique** : Utiliser un réseau de diffusion de contenu responsable
- *Exemples : Cloudflare (100% renouvelable), Microsoft Azure (neutre en carbone)*

### 🗄️ Optimisation des données

#### Bases de données efficaces
- **Requêtes optimisées** : Privilégier les requêtes filtrées et paginées
- **Indexation intelligente** : Éviter la réindexation systématique (Elasticsearch)
- *Exemples : Entity Framework AsNoTracking() pour les requêtes de lecture, pagination avec Skip/Take*

#### Cache et stockage
- **Cache distribué** : Éviter les requêtes répétitives
- **Compression des données** : Réduire les transferts réseau
- *Exemples : Redis pour le cache, Gzip/Brotli pour la compression*

### 🚀 Optimisation du code

#### Performance 
- **Code asynchrone** : Utiliser async/await pour libérer les threads
- **Gestion mémoire** : Éviter les allocations inutiles
- *Exemples : Memory pooling, Span<T>, profiling avec dotMemory*

#### Frontend optimisé
- **Assets légers** : Minification et bundling des fichiers JS/CSS
- **Chargement différé** : Lazy loading pour les composants lourds
- *Exemples : ASP.NET Core bundling, WebP pour les images*

### 📊 Monitoring et mesure

#### Surveillance de l'impact
- **Métriques de consommation** : Surveiller CPU, mémoire, réseau par microservice
- **Analyse de l'empreinte carbone** : Mesurer l'impact environnemental
- *Exemples : Application Insights, Website Carbon Calculator, Green Web Foundation*

#### Optimisation continue
- **Nettoyage régulier** : Supprimer les dépendances et logs inutiles
- **Ajustement des ressources** : Adapter selon les métriques observées
- *Exemples : Serilog avec niveaux configurables, analyse des packages NuGet*

### ✅ Actions prioritaires

1. **Mesurer la consommation réelle**  de chaque microservice et ajuster les limites Docker en conséquence
2. **Implémenter la pagination** sur toutes les APIs
3. **Configurer un CDN écologique** (Cloudflare) pour les assets statiques
4. **Activer la compression** Gzip/Brotli sur toutes les réponses
5. **Mettre en place le monitoring** de consommation des ressources avec un outil comme Application Insights
6. **Optimiser les index** SQL Server, MongoDB et Elasticsearch selon l'usage réel
