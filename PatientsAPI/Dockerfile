FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["P10 - MédiLabo - Patients API/P10 - MédiLabo - Patients API.csproj", "P10 - MédiLabo - Patients API/"]
RUN dotnet restore "P10 - MédiLabo - Patients API/P10 - MédiLabo - Patients API.csproj"
COPY . .
WORKDIR "/src/P10 - MédiLabo - Patients API"
RUN dotnet build "./P10 - MédiLabo - Patients API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./P10 - MédiLabo - Patients API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "P10 - MédiLabo - Patients API.dll"]
