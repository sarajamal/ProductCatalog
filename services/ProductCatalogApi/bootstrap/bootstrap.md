# Bootstrap — ProductCatalogApi

## Prerequisites
- .NET 8 SDK (`dotnet --version` → 8.x)
- SQL Server (LocalDB / Express / Developer)
- Node.js 20+ and npm

## 1. Clone
```bash
git clone https://github.com/sarajamal/ProductCatalog.git
cd ProductCatalog
```

## 2. Connection string (user secrets)
The project already has a `UserSecretsId`. Set the connection string once per machine:
```bash
cd src/ProductCatalogApi
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=ProductCatalogDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets list
```
Shape reference: `appsettings.Development.example.json`.

## 3. Database
The database `ProductCatalogDb` must contain the `Categories` and `Products` tables. There are no EF migrations in the repo yet (see GAP-002). Ask the owner for the current script or create migrations in a `feature/` branch.

## 4. Angular dependencies
```bash
cd src/product-catalog-client
npm install
```

## 5. Run
- `scripts\start-dev.bat` — API + client
- or API only: `scripts\start-api.bat`
- Swagger: http://localhost:5233/swagger · Client: http://localhost:4200

## 6. Build check
```bash
dotnet build ProductCatalog.sln
cd src/product-catalog-client && npm run build
```
