# MuniFlow — Municipal Purchase Order Approval System

A lightweight ERP-style purchase order (PO) approval workflow built with **.NET 8**, **ASP.NET Core**, **Entity Framework Core**, and **Microsoft SQL Server**. Inspired by the workflows found in municipal ERP systems like Tyler Munis, MuniFlow demonstrates end-to-end handling of a common government finance process: submit → route → approve/reject → audit-ready PDF.

Built as a demonstration project for the **Lafayette Consolidated Government — IS&T Software Services** Programmer Analyst role, which supports the Tyler Enterprise ERP replacement of the legacy Lawson (Infor S3) platform.

---

## Features

- **REST API** — 5 endpoints for creating POs, approving/rejecting, and downloading PDF reports
- **Server-rendered web UI** — Razor Pages dashboard, PO list, create form, and details view
- **Approval workflow** — Pending → Approved / Rejected with full audit history per PO
- **PDF report generation** — Audit-ready PO reports via QuestPDF (header, status, line items, approval history)
- **Department budget tracking** — Foreign-key relationship between POs and departments with seeded data
- **Swagger / OpenAPI docs** — Interactive API explorer at `/swagger`
- **Code-first EF migrations** — Reproducible schema via `dotnet ef` tooling

---

## Tech Stack

| Layer            | Technology                                      |
|------------------|-------------------------------------------------|
| Runtime          | .NET 8.0                                        |
| API              | ASP.NET Core Web API + Swagger / OpenAPI        |
| UI               | Razor Pages (server-rendered)                   |
| ORM              | Entity Framework Core 8.0.31 (code-first)       |
| Database         | Microsoft SQL Server 2022 (Developer Edition)   |
| Reporting        | QuestPDF                                        |
| IDE              | Visual Studio 2026 Community                    |

---

## Architecture


Browser ──► Razor Pages ──► Controllers ──► EF Core ──► SQL Server
│ │ │
└────────► PDF Service (QuestPDF) ◄─────────────┘


- **Models** — `Department`, `PurchaseOrder`, `Approval` (with FK relationships)
- **DTOs** — Request/response contracts decoupled from EF entities
- **Controllers** — Thin REST layer, DI-injected `DbContext`
- **Services** — `PurchaseOrderPdfReport` isolates PDF generation
- **Pages** — Dashboard, PO list, PO details, create form

---

## API Endpoints

| Method | Route                                        | Description                              |
|--------|----------------------------------------------|------------------------------------------|
| GET    | `/api/purchaseorders`                        | List all purchase orders                 |
| GET    | `/api/purchaseorders/{id}`                   | Get a specific PO with approval history  |
| POST   | `/api/purchaseorders`                        | Create a new pending PO                  |
| POST   | `/api/purchaseorders/{id}/decision`          | Approve or reject a pending PO           |
| GET    | `/api/purchaseorders/{id}/pdf`               | Download a PDF report for a PO           |

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server 2022 (LocalDB, Developer, or Express)
- Visual Studio 2022+ or `dotnet` CLI

### Setup

1. Clone the repo:
```bash
   git clone https://github.com/<your-username>/muniflow.git
   cd muniflow
```

2. Update the connection string in `appsettings.json` to point at your SQL Server instance.

3. Apply the EF migrations:
```bash
   dotnet ef database update
```

4. Run the app:
```bash
   dotnet run
```

5. Open:
   - Dashboard: `https://localhost:<port>/`
   - Swagger UI: `https://localhost:<port>/swagger`

---

## Data Model

- **Department** (1) ── (∞) **PurchaseOrder** (1) ── (∞) **Approval**
- Seed data: three departments (Finance, IT, Public Works) with annual budgets
- `Amount` and `AnnualBudget` are `decimal(18,2)` for financial precision

---

## Relevance to Municipal ERP Work

MuniFlow mirrors the core pattern behind Tyler Munis / Infor S3 approval workflows:

- Multi-department budget scoping
- Serial approval routing with named approvers and comments
- Audit trail persistence (approval history is append-only, never overwritten)
- Report generation for finance/audit review
- REST endpoints suitable for downstream integration (bolt-ons, dashboards, third-party approvers)

Built to demonstrate hands-on familiarity with the **.NET / C# / SQL Server / SSRS-style reporting** stack used across LCG's ERP modernization.

---

## Author

**Hruthwik Reddy Marikanti**
MS Computer Science, University of Louisiana at Lafayette (2026)
