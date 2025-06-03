# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el archivo de solución y restaura dependencias
COPY BoxExpress.Sol.sln ./
COPY BoxExpress.Api/*.csproj ./BoxExpress.Api/
COPY BoxExpress.Application/*.csproj ./BoxExpress.Application/
COPY BoxExpress.Infrastructure/*.csproj ./BoxExpress.Infrastructure/
COPY BoxExpress.Domain/*.csproj ./BoxExpress.Domain/
COPY BoxExpress.Utilities/*.csproj ./BoxExpress.Utilities/

RUN dotnet restore BoxExpress.Sol.sln

# Copia el resto del código
COPY . .

# Publica la aplicación
RUN dotnet publish BoxExpress.Api/BoxExpress.Api.csproj -c Release -o /app/publish --no-restore

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto si lo necesitas (opcional para Render)
EXPOSE 80
EXPOSE 443

# Comando de arranque
ENTRYPOINT ["dotnet", "BoxExpress.Api.dll"]
