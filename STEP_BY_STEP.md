# End-to-end validation steps

Follow these numbered steps to validate the ERP flows (suppliers, inventory with low-stock alerts, invoices, and engineering office uploads) end-to-end.

## 1) Prepare the environment
1. Install .NET 7 SDK or later and ensure SQL Server is reachable with the connection string in `appsettings.json`.
2. From the repo root, restore and build:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Apply the database schema:
   ```bash
   dotnet ef database update
   ```

## 2) Run the API
4. Start the API from the project root:
   ```bash
   dotnet run
   ```
5. Open Swagger at `http://localhost:5000/swagger` (or the configured URL) or prepare a REST client.

## 3) Authenticate
6. Call `POST /api/User/login` with valid credentials (see seeded admin in `Models/ERPDBContext`).
7. Copy the returned JWT and use it in `Authorization: Bearer <token>` for all secured endpoints and the SignalR hub.

## 4) Supplier setup
8. Create suppliers with `POST /api/Suppliers` (name, phone, address). Optional: set `OpeningBalance`.
9. Retrieve suppliers via `GET /api/Suppliers` or `GET /api/Suppliers/{id}`; confirm balances reflect any opening amounts.

## 5) Inventory lifecycle with low-stock alerts
10. Create inventory items using `POST /api/Inventory` (name, description, quantity, low stock threshold, prices, optional preferred supplier ID and image).
11. Update items with `PUT /api/Inventory/{id}` and keep quantity at or below the `LowStockThreshold` to trigger automatic low-stock notifications.
12. Connect a SignalR client to `/hubs/notifications` using the JWT to observe `lowStock` messages pushed from step 11.
13. Fetch current low-stock items via `POST /api/Inventory/low-stock` and, if desired, broadcast them manually with `POST /api/Inventory/low-stock/notify`.

## 6) Invoice flows
14. Create purchase/sales invoices with `POST /api/Invoices` (associate supplier/client, project, items, and optional payment schedules or attachments). Verify inventory adjusts and supplier/client balances update.
15. Update invoices with `PUT /api/Invoices/{id}` and confirm inventory/balance corrections.
16. Filter invoices via `POST /api/Invoices/filter` and review due reminders with `GET /api/Invoices/due-reminders`.
17. Notify connected clients about due reminders using `POST /api/Invoices/due-reminders/notify` while the SignalR client remains connected.

## 7) Engineering office uploads
18. Add a project with `POST /api/EngineeringOffice` (project/client details, attachments for drawings or images).
19. Review projects via `GET /api/EngineeringOffice` or `GET /api/EngineeringOffice/{id}` and ensure attachments are present.
20. Update or delete projects with `PUT`/`DELETE /api/EngineeringOffice/{id}` and confirm files are cleaned up as expected.

## 8) Exports and sanity checks
21. Export invoices using `POST /api/Invoices/export` (HTML or CSV) and verify branding options render.
22. Confirm Swagger enforces JWT where applicable and inspect server logs for errors during each step.
23. If `dotnet ef` is unavailable, target a SQL Server already migrated with the latest schema; functional testing can proceed with API + SignalR calls above.
