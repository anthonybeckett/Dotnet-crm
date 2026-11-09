# CRM Project – Getting Started

This guide explains how to set up the local development environment, configure secrets, start PostgreSQL, and run the CRM API and Angular frontend.

The commands assume you are running them from the **solution root** — the directory containing `Crm.sln` (or `Crm.slnx`), `Directory.Build.props`, and the `src` and `infrastructure` directories.

## 1. Prerequisites

Install the following tools:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) — use a version supported by the Angular version in this project
- npm (included with Node.js)
- Postgres
- Git 

Check the installations:

```bash
dotnet --version
node --version
npm --version
```

If the Angular CLI is not installed globally:

```bash
npm install --global @angular/cli
```

## 2. Restore and build the .NET solution

From the solution root:

```bash
dotnet restore
dotnet build
```

## 3. Initialise .NET User Secrets

The API uses .NET User Secrets for its local database connection string. User Secrets are stored outside the project directory and should not be committed to Git.

Initialise User Secrets once for the API project:

```bash
dotnet user-secrets init --project src/Crm.Api
```

Set the connection string. 

```bash
dotnet user-secrets set "ConnectionStrings:CrmDb" \
  "Host=localhost;Port=5432;Database=crm;Username=user;Password=YourOwnLocalPassword" \
  --project src/Crm.Api
```

List the configured secret names and values when troubleshooting:

```bash
dotnet user-secrets list --project src/Crm.Api
```

The secret is associated with the API project's `UserSecretsId`. You normally only need to initialise it once per local checkout. If you change the PostgreSQL password, update the connection string secret to match.

> **Important:** The API currently has the connection string configured as a secret, but the database context and database integration are added in a later development step. Starting the API does not yet prove that it can query PostgreSQL.

## 4. Run the CRM API

From the solution root:

```bash
dotnet run --project src/Crm.Api
```

The terminal will show the local HTTP and/or HTTPS address that ASP.NET Core is listening on. Keep this terminal open while using the API.

In another terminal, check the health endpoint. Replace the port with the one shown by `dotnet run`:

```bash
curl http://localhost:5000/health
```

Expected response:

```json
{"status":"Healthy"}
```

Check the root endpoint:

```bash
curl http://localhost:5000/
```

Expected response:

```json
{
  "application": "CRM API",
  "version": "0.1.0",
  "status": "Running"
}
```

The OpenAPI document is exposed in Development at:

```text
http://localhost:5000/openapi/v1.json
```

Use the port printed by your application.

Stop the API with `Ctrl+C`.

## 5. Run the Angular frontend

Open a separate terminal and change to the Angular project directory:

```bash
cd src/crm-web
```

Install dependencies (repeat this after pulling changes that update `package-lock.json`):

```bash
npm install
```

Start the Angular development server:

```bash
npm start
```

Open the address shown in the terminal, usually:

```text
http://localhost:4200
```

## 6. Useful maintenance commands

Run any dotnet commands in the `api` directory and any node commands in the `ui` directory.

| Task | Command |
|---|---|
| Restore .NET packages | `dotnet restore` |
| Build the solution | `dotnet build` |
| Run the API | `dotnet run --project src/Crm.Api` |
| List .NET projects | `dotnet sln list` |
| List API secrets | `dotnet user-secrets list --project src/Crm.Api` |
| Install frontend dependencies | `npm install` |
| Run frontend | `npm start` |

## 7. Database Migrations

This project uses Entity Framework Core migrations to manage PostgreSQL database schema changes.

Migrations are managed separately by each module through its own DbContext.

Run all commands from the solution root. Replace `Module` with the module name e.g `Customers`

Create a migration:

```bash
dotnet ef migrations add MigrationName \
  --project src/Crm.Modules.Module.Infrastructure \
  --startup-project src/Crm.Api \
  --context ModuleDbContext \
  --output-dir Persistence/Migrations
```

Apply migrations:

```bash
dotnet ef database update \
  --project src/Crm.Modules.Module.Infrastructure \
  --startup-project src/Crm.Api \
  --context ModuleDbContext
```

List migrations:

```bash
dotnet ef migrations list \
  --project src/Crm.Modules.Module.Infrastructure \
  --startup-project src/Crm.Api \
  --context ModuleDbContext
```

Generate SQL script:

```bash
dotnet ef migrations script \
  --project src/Crm.Modules.Module.Infrastructure \
  --startup-project src/Crm.Api \
  --context ModuleDbContext \
  --output migrations.sql
```

Roll back migrations:

```bash
dotnet ef database update PreviousMigrationName \
  --project src/Crm.Modules.Module.Infrastructure \
  --startup-project src/Crm.Api \
  --context ModuleDbContext
```
---
