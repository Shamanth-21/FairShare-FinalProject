# 🧾 FairShare – Group Expense Management Web App

**Live Demo:** [https://fairshare-webapp-frdbevfcpeeave8.switzerlandnorth-01.azurewebsites.net](https://fairshare-webapp-frdbevfcpeeave8.switzerlandnorth-01.azurewebsites.net)

FairShare is a full-stack **ASP.NET Core MVC web application** designed to simplify group expense tracking.  
It allows users to create groups, add members, record shared expenses, and view automated cost-splitting summaries.  
The project demonstrates complete **CRUD functionality**, **database persistence (SQLite)**, **API integration**, and **Azure cloud deployment**.

---

## 🧩 Project Overview

| Feature | Description |
|----------|-------------|
| **Framework** | ASP.NET Core MVC (.NET 8.0) |
| **Database** | SQLite (local persistence) |
| **Hosting** | Microsoft Azure App Service |
| **Architecture** | Model–View–Controller (MVC) |
| **API Integration** | [ExchangeRate.host](https://exchangerate.host) for live currency conversion |
| **Frontend** | Razor Views, Bootstrap, Chart.js visualizations |
| **Version Control** | GitHub (multi-member collaboration) |

---

## ⚙️ Functionalities

✅ **Create Groups** – Users can create new groups and add members.  
✅ **Add Expenses** – Record expenses, specify who paid, and auto-split costs.  
✅ **Edit / Delete** – Modify or remove existing expenses or groups.  
✅ **Dashboard Analytics** – Interactive charts summarizing spending.  
✅ **Currency Conversion** – Fetches live exchange rates via ExchangeRate API.  
✅ **Persistent Storage** – Uses SQLite DB to store all user, group, and expense data.  

---

## 🧠 MVC Architecture Overview

src/
└── FairShare.Web/
├── Controllers/ # Request handling and routing (MVC Controller layer)
├── Models/ # Core domain and entity models (Expense, Group, User)
├── ViewModels/ # Combined models for UI rendering
├── Views/ # Razor pages and HTML templates
├── Services/ # Business logic and helper services
├── Data/ # DbContext and EF Core configurations
└── wwwroot/ # CSS, JS, and static assets

---

## 🌐 API Integration

**API Used:** [ExchangeRate.host](https://exchangerate.host)  
**Purpose:** Fetch live currency exchange rates to convert group expenses into a unified currency.  
**Endpoint Example:**
```bash
GET https://api.exchangerate.host/latest?base=USD
```
---

🚀 Deployment & Hosting

Platform: Azure App Service
Build Pipeline:

Project built using .vscode/tasks.json

Published to /bin/Release/net8.0/publish

Deployed using Azure App Service Extension in VS Code

Connection String (SQLite):

"ConnectionStrings": {
  "DefaultConnection": "Data Source=D:\\home\\data\\FairShare.db"
}

---

👩‍💻 Team Members & Contributions
| Member                       | Role                       | Contributions                                                                                           |
| ---------------------------- | -------------------------- | ------------------------------------------------------------------------------------------------------- |
| **Manideep Kadarla**         | Backend & Database         | Backend APIs using EF Core; Database schema/migrations; Expense share calculation logic.                |
| **Shamanth Kodipura Mahesh** | Project Lead & Integration | Project setup & integration; Testing and QA; Documentation; Azure deployment configuration.             |
| **Ujwala Tripurana**         | Frontend & Visualization   | UI/UX with Bootstrap and Razor views; Chart.js analytics and data visualizations; CSS & layout styling. |

---

🧱 Technical Highlights

Implemented complete CRUD operations using EF Core.

Integrated dependency injection and DbContext lifecycle management.

Applied logging providers (Console, Debug) for runtime diagnostics.

Configured Azure Web.config for stdout logging during deployment.

Added ExchangeRate API integration for dynamic currency conversion.

Built responsive Bootstrap UI with Chart.js dashboards.

---

🧩 How to Run Locally
# Clone repository
git clone https://github.com/Shamanth-21/FairShare-FinalProject.git
cd FairShare-FinalProject

# Restore dependencies
dotnet restore src/FairShare.Web/FairShare.Web.csproj

# Build project
dotnet build -c Release src/FairShare.Web/FairShare.Web.csproj

# Run locally
dotnet run --project src/FairShare.Web/FairShare.Web.csproj

Then visit http://localhost:7167

---

💬 Reflections & Learnings

Learned practical MVC implementation and controller-based routing.

Experienced team-based Git workflows and Azure CI/CD deployment.

Understood Entity Framework migrations, seeding, and local database management.

Integrated an external REST API for real-time data fetching.

Designed a user-friendly and responsive interface with Bootstrap and Chart.js.

---

📂 Repository Structure
.vscode/
  ├── tasks.json         # Publish configuration
  └── settings.json      # Azure deployment path
src/
  └── FairShare.Web/
       ├── Controllers/
       ├── Data/
       ├── Models/
       ├── Services/
       ├── Views/
       ├── wwwroot/
       ├── appsettings.json
       └── Program.cs
publish/
  └── ... (Release build output)

---

📈 Future Improvements

Implement user authentication with ASP.NET Identity.

Migrate from SQLite to Azure SQL Database.

Add email notifications for settlements.

Integrate OAuth for login (Google/Microsoft).

Enhance mobile responsiveness and dark mode UI.

---

🏁 Deployment Link

Azure Deployed App:
🔗 https://fairshare-webapp-frdbevfcpeeave8.switzerlandnorth-01.azurewebsites.net


🧾 License

This project was developed as part of ISM 6225 – Application Development for Analytics at the University of South Florida.
© 2025 FairShare Team – All rights reserved.
