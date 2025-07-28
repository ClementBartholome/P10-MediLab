FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NotesAPI.csproj", "."]
RUN dotnet restore "NotesAPI.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "NotesAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
WORKDIR "/src"
RUN dotnet publish "NotesAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotesAPI.dll"]