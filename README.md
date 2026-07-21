<div align="center">

<img src="RealEstateProject/wwwroot/images/logo.png" alt="AqarX Logo" width="140"/>

# AqarX — Real Estate Management Platform

**A full-featured, role-based real estate web platform built with ASP.NET Core MVC**

Connecting **Owners**, **Tenants**, and **Admins** in one integrated ecosystem for listing, discovering, booking, and managing properties.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blueviolet?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0-informational?style=flat-square)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Bootstrap](https://img.shields.io/badge/UI-Bootstrap%205-7952B3?style=flat-square&logo=bootstrap)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-Educational-lightgrey?style=flat-square)]()

</div>

---

## 📖 Table of Contents

- [About the Project](#-about-the-project)
- [Key Features](#-key-features)
- [User Roles & Permissions](#-user-roles--permissions)
- [System Architecture](#-system-architecture)
- [Domain Model / Database Design](#-domain-model--database-design)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Default Admin Account](#-default-admin-account)
- [Screenshots](#-screenshots)
- [Roadmap](#-roadmap)
- [Author](#-author)

---

## 📌 About the Project

**AqarX** is a graduation/training project built as part of the **Digital Egypt Pioneers Initiative (Rowad Masr Al-Raqmeya) — Round 4**, under the mentorship of **Eng. Islam Helmy**.

AqarX is a real estate marketplace that digitizes the full property rental/sale lifecycle: **owners** publish and manage property listings, **tenants** search, filter, save alerts, and book properties, and **admins** oversee the entire platform — approving listings, managing users, subscriptions, promotions, and service partners.

The platform is built end-to-end with **ASP.NET Core MVC (.NET 8)**, using the **Repository-free EF Core Code-First** approach, **ASP.NET Core Identity** for authentication with **role-based authorization**, and a relational **SQL Server** database modeling a real-world real estate business domain (owners, tenants, properties, bookings, subscriptions, promotions, ratings, service partners, alerts, and notifications).

---

## ✨ Key Features

### 🏠 Property Management
- Full **CRUD** for property listings (type, purpose — sale/rent, price, location, conditions)
- Multiple **media (images)** per property
- **Publish / Listing status** workflow (Draft → Pending → Published / Available → Booked)
- Owner-scoped **"My Properties"** dashboard
- Advanced **search & filtering** by property type, purpose, location, and price range

### 👥 Role-Based Access Control
- Three core roles: **Admin**, **Owner**, **Tenant**, enforced via ASP.NET Core Identity + `[Authorize(Roles = "...")]`
- Each role has a tailored dashboard and permitted actions

### ✅ Property Approval Workflow
- Admins review and **approve/reject** newly submitted properties before they go live, with a full approval history log (`PropertyApproval`)

### 🔔 Smart Alerts & Notifications
- Tenants create **saved search alerts** (property type, purpose, location, price range)
- An automated **`AlertMatchingService`** scans every newly published property against active alerts and instantly notifies matching tenants — no duplicate notifications for the same property
- In-app **notification bell** (via a reusable `NotificationBellViewComponent`) with read/unread state

### 💳 Subscription & Monetization Engine
- Configurable **Subscription Plans** (max units, allowed unit types, media limits, featured search, monthly reports, dedicated account manager)
- Owners subscribe with billing cycles, discounts, and payment status tracking
- **Promotions** module to feature/boost specific listings for a paid time window

### ⭐ Ratings & Trust System
- Users can rate and review each other (owner ↔ tenant), building a trust layer across the platform

### 🧰 Service Partners Marketplace
- Third-party **service partners** (e.g., maintenance, moving, legal, cleaning) registered with a category and commission rate
- Tenants submit **service requests** routed to the right partner, with status tracking

### 📅 Booking System
- Tenants book available properties with payment status tracking and booking history

### 📊 Role-Aware Dashboard
- A single dashboard that adapts its statistics and data per role:
  - **Admin:** total properties, active subscriptions platform-wide, average rating across the system
  - **Owner:** their own properties, active subscriptions, average rating received
  - **Tenant:** available/published properties, personal average rating

### 🔐 Authentication & Identity
- Full account system (register, login, external providers scaffolding) via **ASP.NET Core Identity**
- Automatic **role & default Admin account seeding** on first run

---

## 🔑 User Roles & Permissions

| Module | Admin | Owner | Tenant |
|---|:---:|:---:|:---:|
| Browse / Search Properties | ✅ | ✅ | ✅ |
| Create / Edit / Delete own Properties | ✅ | ✅ | ❌ |
| Approve / Reject Properties | ✅ | ❌ | ❌ |
| Manage Users | ✅ | ❌ | ❌ |
| Manage Owners / Tenants records | ✅ | ❌ | ❌ |
| Manage Subscription Plans | ✅ | ❌ | ❌ |
| Subscribe to a Plan | ✅ | ✅ | ❌ |
| Manage Promotions | ✅ | ✅ (own) | ❌ |
| Manage Service Partners | ✅ | ❌ | ❌ |
| Submit Service Requests | ✅ | ❌ | ✅ |
| Create Search Alerts | ❌ | ❌ | ✅ |
| Book a Property | ❌ | ❌ | ✅ |
| Rate other Users | ✅ | ✅ | ✅ |
| View Personalized Dashboard | ✅ | ✅ | ✅ |

---

## 🏗 System Architecture

AqarX follows the classic **ASP.NET Core MVC** layered architecture:

```
┌──────────────────────────────────────────────────────────┐
│                         Views (Razor)                     │
│        Bootstrap 5 · jQuery · jQuery Validation            │
└───────────────────────────┬────────────────────────────────┘
                             │
┌───────────────────────────▼────────────────────────────────┐
│                       Controllers (MVC)                    │
│  Properties · Owners · Tenants · Bookings · Subscriptions   │
│  Promotions · Approvals · Ratings · ServicePartners · ...   │
└───────────────────────────┬────────────────────────────────┘
                             │
┌───────────────────────────▼────────────────────────────────┐
│                          Services                           │
│              AlertMatchingService (business logic)          │
└───────────────────────────┬────────────────────────────────┘
                             │
┌───────────────────────────▼────────────────────────────────┐
│                Data Access — EF Core (Code-First)           │
│                    ApplicationDbContext                     │
└───────────────────────────┬────────────────────────────────┘
                             │
┌───────────────────────────▼────────────────────────────────┐
│                    SQL Server Database                      │
└──────────────────────────────────────────────────────────┘

        Cross-cutting: ASP.NET Core Identity (Authentication +
                Role-Based Authorization: Admin / Owner / Tenant)
```

- **Areas/Identity** hosts the scaffolded Identity Razor Pages (Login, Register, Manage Account, etc.)
- **ViewComponents** provide reusable, self-contained UI pieces (e.g., the notification bell) that fetch their own data independent of the page's controller
- **ViewModels** (`DashboardViewModel`, `PageHeaderModel`) decouple the presentation layer from the domain entities

---

## 🗄 Domain Model / Database Design

The database is modeled around real-world real estate relationships:

| Entity | Responsibility |
|---|---|
| **User** | Core identity/profile shared by Owners, Tenants, and Admins (1-to-1 specialization) |
| **Owner** | A user who lists properties and holds subscriptions |
| **Tenant** | A user who searches, books properties, sets alerts, and requests services |
| **Admin** | A user who governs the platform (approvals, management) |
| **Property** | A listing (type, purpose, price, location, status) owned by an Owner |
| **Media** | Images/videos attached to a Property |
| **Booking** | A Tenant's reservation of a Property, with payment status |
| **PropertyApproval** | Admin's approval/rejection record for a Property |
| **Promotion** | A paid featured-listing boost for a Property |
| **SubscriptionPlan** | A pricing tier defining Owner entitlements |
| **Subscription** | An Owner's active subscription to a Plan |
| **Rating** | A trust review between two Users (giver → receiver) |
| **ServicePartner** | A third-party company offering a service category |
| **ServiceRequest** | A Tenant's request routed to a Service Partner |
| **Alert** | A Tenant's saved search criteria for automated matching |
| **Notification** | An in-app message delivered to a User |

**Relationship highlights:**
- `User` 1—1 `Owner` / `Tenant` / `Admin` (role specialization pattern)
- `Owner` 1—* `Property` · `Owner` 1—* `Subscription`
- `Property` 1—* `Media` · `Property` 1—* `Booking` · `Property` 1—* `Promotion` · `Property` 1—* `PropertyApproval`
- `Tenant` 1—* `Booking` · `Tenant` 1—* `Alert` · `Tenant` 1—* `ServiceRequest`
- `SubscriptionPlan` 1—* `Subscription`
- `ServicePartner` 1—* `ServiceRequest`
- `User` 1—* `Rating` (as Giver) and 1—* `Rating` (as Receiver)

> The schema evolves through **18 EF Core migrations**, tracked incrementally in `RealEstateProject/Migrations/`, most recently adding the **Notifications** system and **reshaping Alerts** for the automated property-matching feature.

---

## 🧱 Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core MVC (.NET 8) |
| **ORM** | Entity Framework Core 9 (Code-First + Migrations) |
| **Database** | Microsoft SQL Server (SQLite package also referenced for lightweight scenarios) |
| **Authentication** | ASP.NET Core Identity (Cookie-based, Role-based Authorization) |
| **Frontend** | Razor Views, Bootstrap 5, jQuery, jQuery Validation (Unobtrusive) |
| **Language** | C# |
| **IDE / Tooling** | Visual Studio, EF Core CLI Tools, Scaffolding (`Microsoft.VisualStudio.Web.CodeGeneration.Design`) |

---

## 📂 Project Structure

```
Real-Estate-Project/
├── AqarX.slnx                          # Solution file
├── Logo.png
└── RealEstateProject/
    ├── Program.cs                      # App bootstrap, Identity config, role/admin seeding
    ├── AqarX.csproj
    ├── Areas/
    │   └── Identity/Pages/              # Scaffolded Identity UI (Login, Register, Manage...)
    ├── Controllers/                     # 17 MVC controllers (one per domain module)
    ├── Data/
    │   └── ApplicationDbContext.cs      # EF Core DbContext & entity configuration
    ├── Migrations/                      # EF Core Code-First migration history
    ├── Models/                          # Domain entities
    │   └── ViewModels/                  # Dashboard & shared view models
    ├── Services/
    │   └── AlertMatchingService.cs      # Business logic: alert ↔ property matching
    ├── ViewComponents/
    │   └── NotificationBellViewComponent.cs
    ├── Views/                           # Razor views, organized per controller
    ├── wwwroot/                         # Static assets (css, js, lib, uploaded images)
    ├── appsettings.json                 # Connection strings & configuration
    └── Properties/launchSettings.json
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full instance)
- Visual Studio 2022 (recommended) or the `dotnet` CLI

### 1. Clone the repository

```bash
git clone https://github.com/Eng-MinaAshraf/Real-Estate-Project.git
cd Real-Estate-Project/RealEstateProject
```

### 2. Configure the database connection

Update the connection string in `appsettings.json` to match your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=AqarX;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Apply EF Core migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

The app seeds the **Admin/Owner/Tenant** roles and a default **Admin** account automatically on first launch, then serves the landing page at the configured URL (see `Properties/launchSettings.json`).

---

## 🔐 Default Admin Account

On first run, the app auto-creates a default administrator so you can log in immediately:

| Field | Value |
|---|---|
| Email | `admin@aqarx.com` |
| Password | `Admin@123` |

> ⚠️ **For development/demo use only.** Change or remove these credentials before any production deployment.

---

## 🖼 Screenshots

> Add screenshots of the Landing page, Property listing/search, Owner dashboard, and Admin approval workflow here to make the README and the presentation more visual.

---

## 🗺 Roadmap

- [ ] Payment gateway integration for bookings & subscriptions
- [ ] Map-based property search (geolocation)
- [ ] REST API layer for a future mobile app
- [ ] Real-time notifications (SignalR)
- [ ] Advanced analytics dashboard for Admins

---

## 👨‍💻 Author

**Mina Ashraf** — [Eng-MinaAshraf](https://github.com/Eng-MinaAshraf)

Developed as part of **Digital Egypt Pioneers Initiative — Round 4**, under the mentorship of **Eng. Islam Helmy**.

</div>
