# CRM Project – Getting Started

This guide explains how to set up the local development environment, configure secrets, start PostgreSQL, run the CRM API and Angular frontend, and execute the automated test suite.

The commands assume you are running them from the **solution root** — the directory containing `Crm.sln` (or `Crm.slnx`), `Directory.Build.props`, and the `src`, `tests` and `infrastructure` directories.

## 1. Prerequisites

Install the following tools:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) — use a version supported by the Angular version in this project
- npm (included with Node.js)
- PostgreSQL
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

The API uses .NET User Secrets for its local database connection strings. User Secrets are stored outside the project directory and should not be committed to Git.

Initialise User Secrets once for the API project:

```bash
dotnet user-secrets init --project src/Crm.Api
```

Set the development database connection string:

```bash
dotnet user-secrets set "ConnectionStrings:CrmDb" \
  "Host=localhost;Port=5432;Database=crm;Username=user;Password=YourOwnLocalPassword" \
  --project src/Crm.Api
```

Set the test database connection string:

```bash
dotnet user-secrets set "ConnectionStrings:CrmTestDb" \
  "Host=localhost;Port=5432;Database=crm_test;Username=user;Password=YourOwnLocalPassword" \
  --project src/Crm.Api
```

Both connection strings should use credentials that can connect to your local PostgreSQL installation.

List the configured secrets when troubleshooting:

```bash
dotnet user-secrets list --project src/Crm.Api
```

The secrets are associated with the API project's `UserSecretsId`. You normally only need to initialise it once per local checkout. If you change the PostgreSQL password, update both connection string secrets to match.

**Important:** Never commit database passwords or other secrets to source control.

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

Run these commands from the solution root unless otherwise stated.

| Task | Command |
|---|---|
| Restore .NET packages | `dotnet restore` |
| Build the solution | `dotnet build` |
| Run all tests | `dotnet test` |
| Run the API | `dotnet run --project src/Crm.Api` |
| List .NET projects | `dotnet sln list` |
| List API secrets | `dotnet user-secrets list --project src/Crm.Api` |
| Install frontend dependencies | `cd src/crm-web && npm install` |
| Run frontend | `cd src/crm-web && npm start` |

## 7. Database Migrations

This project uses Entity Framework Core migrations to manage PostgreSQL database schema changes.

Migrations are managed separately by each module through its own DbContext.

Run all commands from the solution root. Replace `Module` with the module name, e.g. `Customers`.

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

## 8. Automated Testing

The solution contains three automated test projects. They use xUnit and are organised to test different parts of the application independently.

### 8.1 Test project structure

| Test project | Purpose |
|---|---|
| `Crm.Modules.Customers.Domain.Tests` | Tests domain entities, business rules and domain behaviour. |
| `Crm.Modules.Customers.Application.Tests` | Tests application handlers, commands, queries and validators. |
| `Crm.Api.Tests` | Runs integration tests against the API, including HTTP endpoints and PostgreSQL persistence. |

The intention is to keep unit tests independent of infrastructure wherever possible, while integration tests verify that the different application components work together.

### 8.2 Configure the test database

Integration tests use a separate PostgreSQL database named `crm_test`.

This is important because integration tests should never modify the development database.

Create the test database if it does not already exist:

```sql
CREATE DATABASE crm_test;
```

You can execute this through `psql`, DBeaver or another PostgreSQL administration tool.

The test database must be accessible using the credentials configured in the `CrmTestDb` User Secret.

The test project uses `CrmApiFactory` to configure the test host. It explicitly loads the API project's User Secrets and overrides the normal `CrmDb` connection string with the `CrmTestDb` connection string.

This means the test host uses `crm_test` even though the API normally uses `crm`.

### 8.3 Database migrations during integration tests

The `CrmApiFactory` class is responsible for preparing the test database.

When the integration test host starts, it:

1. Loads the `CrmTestDb` connection string from User Secrets.
2. Overrides the API's normal database connection string.
3. Creates the test host.
4. Applies any pending Entity Framework Core migrations using `Database.Migrate()`.

The Customers module's database schema and tables are therefore created automatically when the integration tests run.

Wolverine also initialises its message persistence schema in the test database.

You should see output similar to this when the tests execute:

```text
Test database: crm_test
```

This provides a useful confirmation that the integration tests are connected to the correct database.

**Important:** The test database is persistent. Integration tests may leave records behind between runs. Do not use it for normal development data, and do not configure it to point at the `crm` development database.

### 8.4 Run all tests

From the solution root:

```bash
dotnet test
```

This restores, builds and executes the test projects in the solution.

A successful run should report that all tests have passed.

### 8.5 Run individual test projects

To run only the Customers domain tests:

```bash
dotnet test tests/Crm.Modules.Customers.Domain.Tests
```

To run only the Customers application tests:

```bash
dotnet test tests/Crm.Modules.Customers.Application.Tests
```

To run only the API integration tests:

```bash
dotnet test tests/Crm.Api.Tests
```

### 8.6 Run specific tests

You can filter tests by their fully qualified name.

For example, run tests containing `CreateCustomer`:

```bash
dotnet test --filter "FullyQualifiedName~CreateCustomer"
```

Run tests containing `GetCustomer`:

```bash
dotnet test --filter "FullyQualifiedName~GetCustomer"
```

This is useful when working on a particular feature and you don't need to execute the entire test suite after every small change.
