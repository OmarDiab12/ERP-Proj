# Testing Guide

This project is an ASP.NET Core API with SignalR. Use the steps below to validate the recent supplier, inventory, invoicing, engineering office, and notification flows.

## Prerequisites
- .NET 7 SDK or later installed locally.
- SQL Server instance reachable by the connection string in `appsettings.json`.
- Optional: A REST client such as `curl`, Postman, or the built-in Swagger UI at `/swagger`.

## Build & database
1. Restore and build:
   ```bash
   dotnet restore
   dotnet build
   ```
2. Apply migrations to create/update the database schema:
   ```bash
   dotnet ef database update
   ```

## Authentication
- Obtain a JWT by calling `POST /api/User/login` with valid credentials (see seeded admin in `Models/ERPDBContext`).
- Include the token in the `Authorization: Bearer <token>` header for protected endpoints.

## Suppliers
- **Create/Update**: `POST /api/Suppliers` and `PUT /api/Suppliers/{id}` with name, phone, and address. Verify balances update when linked invoices are created.
- **Get**: `GET /api/Suppliers` or `GET /api/Suppliers/{id}` to confirm stored data and running balance.

## Inventory
- **Create/Update**: `POST /api/Inventory` and `PUT /api/Inventory/{id}` with product details and optional image. Ensure preferred supplier IDs exist.
- **Low stock**: `GET /api/Inventory/low-stock` to confirm items at or below thresholds are returned.

## Invoices
- **Create purchase/sales invoices**: `POST /api/Invoices` with items, attachments (multipart), related supplier/client, and optional payment schedules. Confirm stock adjustments and supplier balance changes.
- **Edit**: `PUT /api/Invoices/{id}` to verify updates reconcile inventory and balances.
- **Filter**: `POST /api/Invoices/filter` with type/status/date filters to validate querying.
- **Reminders**: `GET /api/Invoices/due-reminders` to fetch upcoming/overdue installments or invoice dues.
- **Exports**: `POST /api/Invoices/export` to receive HTML (print/PDF-style) or CSV output with branding options.

## Engineering Office
- **Create project**: `POST /api/EngineeringOffice` with client/project details and attachments. Confirm stored file references.
- **Update/Delete**: `PUT` and `DELETE` endpoints to verify attachment cleanup and data updates.
- **List/Detail**: `GET /api/EngineeringOffice` and `GET /api/EngineeringOffice/{id}` to confirm retrieval.

## Real-time notifications (SignalR)
1. Start the API (e.g., `dotnet run`).
2. Connect a client to `/hubs/notifications` using a JWT-bearing access token.
3. Trigger reminders via `POST /api/Invoices/due-reminders/notify`; observe broadcast payloads on the connected client.

## Manual sanity checks
- Verify Swagger UI loads and secured endpoints require JWT.
- Check logs for repository/service error messages during CRUD operations.
- Ensure file uploads land in the configured storage path and delete flows remove unused files.

## Notes
- If `dotnet ef` is unavailable, you can target a SQL Server that already has the schema created by the latest migrations.
- No automated tests exist yet; validation is manual via the API and SignalR client.
