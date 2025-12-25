🚲 Bike Store Management System

A desktop-based Bike Store Management System built with C# (.NET 8 WinForms) and SQLite, designed for small to medium bike shops to manage inventory, sales, and service records in an offline-first environment.
📌 Features
🧾 Inventory Management

Add, edit, and delete bike products

Products tracked by Brand, Type, Color, Quantity, Price

Automatic stock merging when adding identical products

Autocomplete suggestions for Brand / Type / Color

IDR currency formatting

Uppercase normalization to ensure data consistency

💰 Sales Management

Create sales directly from available inventory

Inventory-aware dropdown selection (prevents selling unavailable items)

Automatic stock deduction after sale

Prevents overselling

Auto-calculated total price based on quantity

Transaction log with searchable history

🛠️ Service Logging

Log bike services (non-inventory affecting)

Autocomplete suggestions for Brand / Type / Color

Fixed quantity (1 service per log entry)

Service cost and notes supported

Dedicated service log (read-only)

📊 Logs & Reporting

Transaction Log

Service Log

Live search filtering

IDR currency formatting

Read-only, audit-friendly design

🧑‍💻 Tech Stack

Language: C#

Framework: .NET 8 (WinForms)

Database: SQLite (local file-based)

ORM: None (raw SQL for clarity & control)

Testing: MSTest (repository-level tests)

Bike-STore-Project/
│
├── Bike_STore_Project/          # Main WinForms application
│   ├── Forms/                   # Inventory, Sales, Service, Logs
│   ├── Repositories/            # ProductRepository, etc.
│   ├── Models/                  # Product model
│   ├── Database.cs              # SQLite initialization & connection
│   └── MainMenuControl.cs
│
├── Bike_STore_Project.Tests/    # Unit tests (MSTest)
│
├── data.db                      # SQLite database (local)
└── README.md

🗄️ Database Schema (Simplified)
products
Column	Type
id	INTEGER
brand	TEXT
type	TEXT
color	TEXT
quantity	INTEGER
price	REAL
sales
Column	Type
id	INTEGER
brand	TEXT
type	TEXT
color	TEXT
quantity	INTEGER
price	REAL
customer_name	TEXT
date_time	TEXT
services
Column	Type
id	INTEGER
brand	TEXT
type	TEXT
color	TEXT
quantity	INTEGER
service_cost	REAL
notes	TEXT
date_time	TEXT
🧪 Testing

Repository-level tests using MSTest

Tests run against isolated temporary SQLite databases

Covers:

Insert

Update

Delete

Search filtering

Stock logic

To run tests:

dotnet test

🔐 Design Principles

Offline-first (no internet required)

Data integrity first

Separation of concerns

UI (Forms)

Data access (Repositories)

Fail-safe operations

No overselling

Transaction-based updates

User error prevention

Autocomplete

Dropdown constraints

Read-only logs

🌏 Localization

Currency formatted for Indonesian Rupiah (IDR)

No decimal fractions for prices

Culture-aware formatting

🚧 Future Improvements (Planned)

Printable transaction & service reports

Discount handling

Customer management

Role-based access

Cloud sync (optional)

Export reports (PDF / CSV)

👤 Author

Jason Leonard
Bachelor of Information and Communication Technology (Software Technology)

📄 License

This project is for educational and internal business use.
