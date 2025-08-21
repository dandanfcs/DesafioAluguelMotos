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

### PostgreSQL Comando ajustado Windows PowerShell
```bash
docker run -d \
  --name postgres-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=123456 \
  -e POSTGRES_DB=MotosAppDb \
  -p 5432:5432 \
  postgres:15
```

### PostgreSQL Comando para Windows PowerShell
```bash
docker run -d --name postgres-db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=123456 -e POSTGRES_DB=MotosAppDb -p 5432:5432 postgres:15
```

### RabbitMQ Comando para Linux/macOS:
```bash
docker run -d \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```
### RabbitMQ Comando para Windows PowerShell
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

- **RabbitMQ UI:** http://localhost:15672  
  - Usuário padrão: `guest`  
  - Senha padrão: `guest`  

---

## 🔧 Dependências do projeto

No `.csproj` da camada de Application, certifique-se de ter:

```xml
<PackageReference Include="Microsoft.AspNetCore.Http.Features" Version="2.3.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.8" />
```

No `.csproj` da camada de Infra, certifique-se de ter:

```xml
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.19" />
<PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="2.3.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity" Version="2.3.1" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.19" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.8">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.4" />
<PackageReference Include="RabbitMQ.Client" Version="6.8.1" />
```

No `.csproj` da camada de WebApi, certifique-se de ter:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.8">
	<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
	<PrivateAssets>all</PrivateAssets>
</PackageReference>
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
<PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="7.0.0" />
```

---

## 🔹 Configuração do `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
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
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app_log.txt",
          "rollingInterval": "Day"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
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
No Visual studio 2022 a opção como na imagem abaixo: 
### Ferramentas -> Gerenciador de pacotes Nugget -> Console do Gerenciador de pacotes Nugget

<img width="761" height="447" alt="image" src="https://github.com/user-attachments/assets/7fe72e74-878b-4259-8e6a-201db4fbb7f7" />

Após isso, execute os comandos abaixo:
```bash
dotnet ef migrations add InitAppDb -p Infrastructure/Infrastructure.csproj -s WebApi/WebApi.csproj --context AppDbContext
dotnet ef database update -p Infrastructure -s WebApi --context AppDbContext
```
Após isso, execute os outros comandos abaixo:
### 2️⃣ Migrations para `ApplicationUserDbContext` (Identity)

```bash
dotnet ef migrations add InitIdentity -p Infrastructure -s WebApi --context ApplicationUserDbContext
dotnet ef database update -p Infrastructure -s WebApi --context ApplicationUserDbContext
```

> `-p` = projeto onde está o DbContext (`Infrastructure`)  
> `-s` = projeto startup da API (`WebApi`)  

---

### Definindo Projeto de inicialização
Certifique-se que o projeto de inicialização é o WebApi, caso não seja clique com botão direito em cima do projeto e clique em "Definir projeto de inicialização" como na imagem abaixo:

<img width="540" height="1088" alt="image" src="https://github.com/user-attachments/assets/e375d918-d41f-4398-b1d7-1e62949ca5d3" />


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

4. Swagger UI: `https://localhost:7234/swagger`  



