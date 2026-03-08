# Invoice Management API

A RESTful API for managing invoices built with **ASP.NET Core 8**, **Dapper**, **SQL Server**, and **JWT authentication**.

---

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQL Server (local or Docker)

### 1. Database

Run [`sql/schema.sql`](sql/schema.sql) against your SQL Server instance to create the database and tables.

> **Using Docker?**
> ```bash
> docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
>   -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
> ```

### 2. Configuration

Edit `InvoiceManagement.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=InvoiceManagementDb;User Id=sa;Password=YourStrong@Passw0rd;Encrypt=False;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "CHANGE_THIS_TO_A_STRONG_SECRET_IN_PRODUCTION",
    "Issuer": "InvoiceManagement.Api",
    "Audience": "InvoiceManagement.Client",
    "ExpiryMinutes": 120
  }
}
```

---

## How to Run the Project

```bash
# 1. Clone and navigate
git clone https://github.com/RavvyS/teamcollaboro.git
cd teamcollaboro

# 2. Restore packages
dotnet restore

# 3. Run the API
cd InvoiceManagement.Api
dotnet run --launch-profile http
```

The API starts at **`http://localhost:5168`**.  
A built-in frontend UI is available at the same URL (`wwwroot/index.html`).

---

## API Endpoints

### Auth *(no token required)*

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/Auth/register` | Register a new user |
| `POST` | `/api/Auth/login` | Login and receive a JWT token |

**Register** — `POST /api/Auth/register`
```json
{ "name": "John", "email": "john@example.com", "password": "Pass@123", "role": "User" }
```
Returns `201 Created` · `409` if email already exists

**Login** — `POST /api/Auth/login`
```json
{ "email": "john@example.com", "password": "Pass@123" }
```
Returns `200 OK` with `{ token, name, email, role }` · `401` if invalid

---

### Invoices *(JWT required)*

Add to every request:
```
Authorization: Bearer <your_token>
```

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| `POST` | `/api/Invoices` | User/Admin | Create invoice |
| `GET` | `/api/Invoices` | User/Admin | List all invoices |
| `GET` | `/api/Invoices/{id}` | User/Admin | Get invoice by ID |
| `PUT` | `/api/Invoices/{id}` | User/Admin | Update invoice |
| `DELETE` | `/api/Invoices/{id}` | **Admin only** | Delete invoice |

**Create / Update body:**
```json
{
  "invoiceNumber": "INV-001",
  "customerName": "Acme Corp",
  "amount": 1500.00,
  "status": "Draft"
}
```
> `status` accepts: `Draft` · `Paid` · `Cancelled`

---

## Swagger Configuration *(Bonus)*

Swagger UI is enabled automatically in **Development** mode.

**URL:** `http://localhost:5168/swagger`

### Authorizing in Swagger

1. Call `POST /api/Auth/login` and copy the `token` from the response.
2. Click **Authorize 🔒** (top right of the Swagger page).
3. Paste the token *(without the `Bearer` prefix)* and click **Authorize**.
4. All secured endpoints are now unlocked for the session.

> Swagger is disabled in production via the `app.Environment.IsDevelopment()` guard in `Program.cs`.
