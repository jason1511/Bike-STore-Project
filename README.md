# Bike Store Desktop Management System

A Windows desktop management application for an electric bicycle retailer. It replaces handwritten stock, invoice, and service records with a local-first workflow backed by SQLite.

## Current features

- C# WinForms on .NET 8
- Local SQLite database (`data.db`)
- Unified single-window admin dashboard with role-aware navigation
- Overview cards, recent activity, seven-day sales, and stock charts
- Administrator and staff login with PBKDF2 password hashing
- Website-compatible bicycle catalogue with per-colour stock variants
- Dedicated Tambah Stok flow for existing or new colours
- FIFO stock batches and automatic stock-movement history
- Multi-item invoices with customer and payment details
- Optional bicycle frame numbers per invoice line
- Daily sequential invoice numbers and A5 print preview
- Searchable invoice history with administrator-only editing, voiding, deletion, and stock restoration
- Searchable service history with status filters, editing, deletion, workflow status, and A4 print preview
- Date-range sales, service, profit, payment, top-product, and stock reports with landscape printing
- User management and administrator-only audit activity

## Roles

### Staff (`USER`)

- View catalogue, inventory, invoices, and service history
- Add and edit catalogue entries
- Receive and correct stock batches
- Create and print invoices and service records

### Administrator (`ADMIN`)

- All staff capabilities
- Delete eligible catalogue entries and untouched stock batches
- Void invoices and restore stock
- Update service status
- Manage brands and users
- View reports, stock movements, and audit activity

## Inventory model

Every stock receipt creates a `stock_lots` row. Invoice sales consume the oldest remaining lots first, ordered by receipt date and lot ID. Each allocation is stored in `sale_lines`, preserving the cost snapshot needed for gross-profit reporting.

The newer admin tables are additive. Existing desktop databases are upgraded automatically during startup without deleting existing products, sales, services, stock lots, or users.

## Run locally

Requirements:

- Windows 10 or later
- .NET 8 SDK

```powershell
git clone https://github.com/jason1511/Bike-STore-Project.git
cd Bike-STore-Project
dotnet restore
dotnet run
```

On a new database, the application creates a starter administrator account. Review the local seed configuration and change its password before operational use.

## Main local tables

- `products`, `brands`
- `stock_lots`, `stock_movements`
- `invoices`, `invoice_items`, `invoice_sequences`
- `sales`, `sale_lines`
- `services`
- `users`, `audit_log`

## Future cloud phase

The desktop workflows intentionally use SQLite first. After they are verified in-store, the repository layer can be replaced by calls to the Bike Store website's Cloudflare Worker API so the desktop and browser interfaces share the same Cloudflare D1 data.
