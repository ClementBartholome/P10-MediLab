# Étape 1 : Image de base pour exécuter l'application ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Utilisateur non-root (doit être défini dans l'environnement d'exécution)
USER $APP_UID
WORKDIR /app
# Expose les ports utilisés par l'application
EXPOSE 8080
EXPOSE 8081

# Étape 2 : Image de build pour compiler l'application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copie le fichier projet pour restaurer les dépendances
COPY ["Gateway/Gateway.csproj", "Gateway/"]
RUN dotnet restore "Gateway/Gateway.csproj"
# Copie le reste du code source
COPY . .
WORKDIR "/src/Gateway"
# Compile le projet
RUN dotnet build "./Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Étape 3 : Build de l'application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Étape 4 : Image finale pour exécuter l'app
FROM base AS final
WORKDIR /app
# Copie les fichiers publiés depuis l'étape précédente
COPY --from=publish /app/publish .
# Commande de démarrage du conteneur
ENTRYPOINT ["dotnet", "Gateway.dll"]
