# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /source

COPY ["src/ej2-documenteditor-server/ej2-documenteditor-server.csproj", "./ej2-documenteditor-server/ej2-documenteditor-server.csproj"]
COPY ["src/ej2-documenteditor-server/NuGet.Config", "./ej2-documenteditor-server/"]

RUN dotnet restore "./ej2-documenteditor-server/ej2-documenteditor-server.csproj"
COPY . .

WORKDIR "/source/src"
RUN dotnet build -c Release -o /app

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal-arm64v8 AS final

WORKDIR /app
COPY --from=publish /app .

EXPOSE 80

ENV SYNCFUSION_LICENSE_KEY=""
ENV SPELLCHECK_DICTIONARY_PATH=""
ENV SPELLCHECK_JSON_FILENAME=""
ENV SPELLCHECK_CACHE_COUNT=""

ENTRYPOINT ["dotnet", "ej2-documenteditor-server.dll"]
