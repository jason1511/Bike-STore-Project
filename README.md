# 🚲 Bike Store Management System

A professional **offline-first desktop management system** built with **C# (.NET 8 WinForms)** and **SQLite**, designed to support day-to-day operations of small to medium bike shops.

The application focuses on **data integrity, usability, and reliability**, providing inventory, sales, and service tracking without requiring an internet connection.

---

## 🔍 Project Overview

This system was developed to solve a real-world business need: managing bike store operations in environments where **simplicity, speed, and offline reliability** are critical.

Key design goals:
- Offline-first operation
- Strong data consistency and validation
- Clear separation between UI and business logic
- Minimal setup and maintenance

---

## ✨ Core Features

### Inventory Management
- Create, update, and remove products
- Products tracked by **Brand, Type, Color, Quantity, and Price**
- Automatic stock merging for identical items
- Autocomplete inputs to ensure consistent data entry
- Normalization of product identifiers for clean records

### Sales Management
- Sales can only be created from available inventory
- Real-time validation prevents overselling
- Automatic stock deduction after completed transactions
- Quantity-based total price calculation
- Searchable transaction history

### Service Logging
- Service records managed independently from inventory
- Supports service cost and technician notes
- Autocomplete for Brand, Type, and Color
- Read-only service logs to preserve audit integrity

### Logs & History
- Sales transaction log
- Service log
- Read-only records for accountability
- Search by brand, type, color, customer, or notes
- Currency formatting using **IDR (Indonesian Rupiah)**

---

## 🛠 Technology Stack

- **Language:** C#
- **Framework:** .NET 8 (WinForms)
- **Database:** SQLite (`Microsoft.Data.Sqlite`)
- **Testing:** MSTest
- **Platform:** Windows Desktop
- **Architecture:** Offline-first, layered design

---

## 🏗 Architecture & Design

The application follows a **layered architecture**:

- **UI Layer (WinForms):** Handles user interaction only
- **Repository Layer:** Enforces business rules and manages database access
- **Data Layer:** SQLite database stored locally on the machine

Critical logic such as **stock validation, transaction safety, and consistency rules** is enforced at the repository level rather than the UI, improving maintainability and reliability.

---

## 🗄 Database Behavior

- Database file (`data.db`) is created automatically on first launch
- No manual setup or configuration required
- Data persists between application runs
- Fully functional without internet connectivity

---

## ▶ Running the Application

1. Download the latest release from **GitHub Releases**
2. Extract the application folder
3. Run `BikeStore.exe`

The database file will be generated automatically on first run.

---

## 📈 Future Enhancements

- Reporting and analytics module
- Role-based access control (Admin / Staff)
- Data export (CSV)
- Optional cloud synchronization

---

## 📄 License

This project is licensed under the **GNU General Public License (GPL)**.
