# 🏍 MotorcycleShopMVC

A full-stack ASP.NET Core MVC project for managing a Motorcycle Shop system with role-based authentication, admin dashboard, vendor approval workflow, and e-commerce features (cart, wishlist, orders, etc.).

---

# 👥 Team & AI Collaboration Guide

This project is developed by **4 developers**, each supported by their own AI assistant.

To ensure consistency across humans + AI:

## 🔥 AI RULES (IMPORTANT)

Each AI assistant must understand:

- Project uses **ASP.NET Core MVC**
- Authentication = **Cookie + Session hybrid**
- Role system = `Admin / Vendor / Customer`
- Admin features are protected by `[RoleAuthorize("Admin")]`
- Vendor must be **approved by Admin before login access**
- UI uses **Razor Views + Bootstrap**
- Database = **Entity Framework Core + MySQL**

---

## 👨‍💻 Roles of team

| Member | Responsibility |
|--------|----------------|
| Dev 1 | Authentication + Account system |
| Dev 2 | Admin dashboard + user management |
| Dev 3 | Product (Motorcycle / Part) system |
| Dev 4 | Cart / Order / Wishlist |

---

# 🧠 Project Overview

MotorcycleShopMVC is an e-commerce system where:

### 👤 Users
- Register as `Customer` or `Vendor`
- Login via Email + Password
- Vendor must wait for Admin approval

### 🛠 Admin
- Manage users (lock/unlock/delete)
- Approve / reject vendors
- Change roles
- View orders
- View revenue analytics dashboard

### 🏪 Vendor
- Can add motorcycles and parts (after approval)
- Manage products
- Manage orders

### 🛒 Customer
- Browse motorcycles & parts
- Add to wishlist
- Add to cart
- Place orders

---

# 🔐 Authentication System

This project uses **dual authentication system**:

## 1. Cookie Authentication (Primary)
- Uses `Microsoft.AspNetCore.Authentication.Cookies`
- Stores:
  - Name
  - Email
  - Role
  - UserId

## 2. Session (Fallback + UI Sync)
Stored values:
```csharp
Session["UserId"]
Session["UserEmail"]
Session["UserFullName"]
Session["UserRole"]