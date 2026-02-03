# Bike Store Management System

A desktop-based bike / e-bike store management system built with **C# (.NET 8 WinForms)** and **SQLite**, designed for offline-first use in a small retail environment.

This project focuses on **real-world inventory handling**, including **FIFO stock logic**, sales tracking, service records, and responsive desktop UI.

---

## ✨ Features

### 📦 Inventory Management
- Product identity management (Brand / Type / Color)
- Batch-based stock receiving
- FIFO (First-In, First-Out) stock handling
- Real-time quantity tracking per batch
- Inventory search and filtering

### 💰 Sales Management
- Sales processing with manual pricing
- Automatic FIFO stock deduction
- Customer name recording
- Validation to prevent overselling
- Transaction history with profit calculation

### 🧾 Transaction Log
- Sales summary view
- FIFO sale line breakdown per transaction
- Revenue, cost, and profit calculation
- Searchable transaction history
- Split view for sales and sale lines

### 🔧 Service Management
- Service entry for bikes/e-bikes
- Service cost and notes
- Autocomplete for brand/type/color
- Service log with searchable history

### 🖥️ User Interface
- Responsive WinForms layout (no overlapping controls)
- Proper fullscreen and resize support
- High-DPI friendly scaling
- Consistent UI across all forms
- Keyboard-friendly inputs

---

## 🧠 FIFO Stock Handling (Core Logic)

Stock is managed using a **FIFO (First-In, First-Out)** approach:
- Each received batch is stored as a separate stock lot
- Sales consume stock starting from the oldest available batch
- Partial batch consumption is supported
- Accurate cost and profit calculation per sale

This mirrors how real-world inventory systems operate.

---

## 🛠️ Tech Stack

- **Language:** C#
- **Framework:** .NET 8 (WinForms)
- **Database:** SQLite
- **Architecture:** Repository pattern (data access separation)
- **Platform:** Windows desktop

---
