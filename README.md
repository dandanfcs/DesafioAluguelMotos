# API de Locação de Motos

## 📌 Pré-requisitos

Antes de rodar a API, você precisa ter instalados:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [Docker Desktop](https://www.docker.com/products/docker-desktop)  
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou VS Code  
- [PostgreSQL](https://www.postgresql.org/) (ou via Docker)  
- [RabbitMQ](https://www.rabbitmq.com/download.html) (ou via Docker)

---

## 🐳 Serviços externos via Docker

### PostgreSQL

```bash
docker run -d \
  --name postgres-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=123456 \
  -e POSTGRES_DB=MotosAppDb \
  -p 5432:5432 \
  postgres:15
```

### RabbitMQ

```bash
docker run -d \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

- **RabbitMQ UI:** http://localhost:15672  
  - Usuário padrão: `guest`  
  - Senha padrão: `guest`  

---

## 🔧 Dependências do projeto

No `.csproj` da camada de Infra, certifique-se de ter:

```xml
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.19" />
    <PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="2.3.0" />
    <PackageReference Include="Microsoft.AspNetCore.Identity" Version="2.3.1" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.19" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.8">
```

No `.csproj` da camada de WebApi, certifique-se de ter:

```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.8">
     <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
     <PrivateAssets>all</PrivateAssets>
   </PackageReference>
   <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

---

## 🔹 Configuração do `appsettings.json`

```json
{
  "AllowedHosts": "*",
 "ConnectionStrings": {
   "DefaultConnection": "Host=localhost;Port=5432;Database=MotosAppDb;Username=postgres;Password=123456"
 },
 "JwtSettings": {
   "Key": "chave-jwt-desafio-backend-dotnet",
   "Issuer": "MinhaApi",
   "Audience": "MinhaApiUsers"
 },
 "RabbitMq": {
   "HostName": "localhost",
   "UserName": "guest",
   "Password": "guest"
 }
}
```
## 🔹 Configuração do `launchSettings.json`

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:37859",
      "sslPort": 44319
    }
  },
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5065",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7234;http://localhost:5065",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```
---

## ⚡ Rodando as Migrations

Certifique-se que **dotnet-ef** está instalado globalmente:

```bash
dotnet tool install --global dotnet-ef
```

### 1️⃣ Migrations para `AppDbContext` (dados da aplicação)

```bash
dotnet ef migrations add InitAppDb -p Infrastructure -s WebApi --context AppDbContext
dotnet ef database update -p Infrastructure -s WebApi --context AppDbContext
```

### 2️⃣ Migrations para `ApplicationUserDbContext` (Identity)

```bash
dotnet ef migrations add InitIdentity -p Infrastructure -s WebApi --context ApplicationUserDbContext
dotnet ef database update -p Infrastructure -s WebApi --context ApplicationUserDbContext
```

> `-p` = projeto onde está o DbContext (`Infrastructure`)  
> `-s` = projeto startup da API (`WebApi`)  

---

## 🚀 Rodando a API

1. Abra o terminal na raiz do projeto  
2. Execute:

```bash
dotnet run --project WebApi/WebApi.csproj
```

3. A API estará disponível em:

```
https://localhost:7234
http://localhost:5065
```

4. Swagger UI: `https://localhost:5001/swagger`  



