# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base

# Instalar tzdata y establecer la zona horaria
RUN apk add --no-cache tzdata

ENV TZ=America/Bogota

WORKDIR /app
EXPOSE 80

# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Etapa de publicación
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish
# # Copiar archivo JSON de credenciales de Firebase
# COPY fithub-connect-plus-firebase-adminsdk.json /app/firebase/

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS final

# Instalar tzdata en la etapa final
RUN apk add --no-cache tzdata

ENV TZ=America/Bogota

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
