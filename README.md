# 🚲 Bike Store Management System

A desktop-based **Bike Store Management System** built with **C# (.NET 8 WinForms)** and **SQLite**, designed for offline-first inventory, sales, and service management.

This application is intended for small to medium bike shops that require a simple, reliable, and locally stored system without internet dependency.

---

## ✨ Features

### Inventory Management
- Add, edit, and delete products
- Products tracked by **Brand, Type, Color, Quantity, and Price**
- Automatic stock merging for identical items
- Autocomplete suggestions to maintain data consistency
- Uppercase normalization for all product identifiers

### Sales Management
- Sales created only from available inventory
- Prevents overselling with real-time stock validation
- Automatic stock deduction after successful sale
- Quantity-based total price calculation
- Transaction log with live search

### Service Logging
- Log bike services independently from inventory
- Service cost and notes support
- Autocomplete for Brand, Type, and Color
- Read-only service log for audit purposes

### Logs & History
- Transaction Log (Sales)
- Service Log
- Read-only design
- Searchable by brand, type, color, customer, or notes
- IDR (Indonesian Rupiah) currency formatting

---

## 🛠 Tech Stack

- **Language:** C#
- **Framework:** .NET 8 (WinForms)
- **Database:** SQLite (Microsoft.Data.Sqlite)
- **Testing:** MSTest
- **Platform:** Windows (offline-first desktop application)

---

## 🏗 Architecture Overview

The application follows a simple layered structure:

- **UI Layer (WinForms):** Inventory, Sales, Service, and Log forms
- **Repository Layer:** Handles all database access and business logic
- **Data Storage:** SQLite database stored locally on the machine

Critical business rules such as stock validation and transaction safety are enforced at the repository level rather than the UI.

---

## 🗄 Database Behavior

- The database file (`data.db`) is created automatically on first run
- No manual database setup is required
- Data is stored locally and persists between application runs
- The application functions fully offline

---

## ▶ How to Run

1. Download the latest release from **GitHub Releases**
2. Extract the folder
3. Run `BikeStore.exe`

> The database file (`data.db`) will be generated automatically on first launch.

---
