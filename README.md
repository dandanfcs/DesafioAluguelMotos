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
  -e POSTGRES_PASSWORD=Senha123 \
  -e POSTGRES_DB=LocacaoDb \
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

No `.csproj` da API, certifique-se de ter:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.*" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.*" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.*" />
<PackageReference Include="RabbitMQ.Client" Version="7.4.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.*" />
```

---

## 🔹 Configuração do `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LocacaoDb;Username=postgres;Password=Senha123",
    "IdentityConnection": "Host=localhost;Port=5432;Database=IdentityDb;Username=postgres;Password=Senha123"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
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
https://localhost:5001
http://localhost:5000
```

4. Swagger UI: `https://localhost:5001/swagger`  

---

## 🔄 Testando integrações

- **RabbitMQ:** verifique se a conexão está funcionando via UI (`http://localhost:15672`)  
- **PostgreSQL:** conecte via PgAdmin ou DBeaver para validar se as tabelas foram criadas  

---

## 💡 Dicas

- Para ambiente de desenvolvimento, use **Docker Compose** para subir PostgreSQL e RabbitMQ juntos  
- Sempre configure **connection strings e credenciais** no `appsettings.Development.json`  
- Use `dotnet ef database update` sempre que adicionar novas migrations  

