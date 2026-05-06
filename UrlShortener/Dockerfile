FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UrlShortener.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# SQLite veritabanı volume mount için
VOLUME ["/app/data"]
ENV ConnectionStrings__Default="Data Source=/app/data/urls.db"

ENTRYPOINT ["dotnet", "UrlShortener.dll"]
