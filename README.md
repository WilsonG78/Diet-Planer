# Diet-Planer

A web application for managing patient diets in a hospital.

## Author

**Filip Latawiec**  
AGH University of Science and Technology, Kraków  
Advanced Programming 2 — Laboratory 11

---

## Project Description

Diet-Planer is a web application built with ASP.NET Core MVC, designed for hospital staff. It allows managing patients, assigning dietary plans, and organising hospital wards. Access is protected by a session-based login system — every user must authenticate before using the application. Passwords are stored exclusively as SHA-256 hashes.

The application also exposes a REST API that allows external systems to access data programmatically using token-based authentication.

---

## Features

### Data Management
- **Patients** — create, edit and delete patients (first name, last name, PESEL); assign a diet and a ward
- **Diets** — define diets with name, type (Low Fat / Vegetarian) and caloric value (kcal)
- **Wards** — manage hospital wards (name, floor); view all patients assigned to a ward
- **Users** — admin-only; create accounts with roles, change passwords

### Reports & Statistics
- **Ward Overview** — per-ward breakdown: total patients, % with assigned diet, average kcal, diet distribution
- **Diet Distribution** — table showing each diet's share of the total patient population, broken down by ward
- **Unassigned Patients** — list of patients missing a diet or ward assignment, with direct links to edit

### Security
- Session-based login with username and password
- Passwords hashed with SHA-256
- Admin account created automatically on first run
- User management section restricted to the Admin role only

### REST API
- Full CRUD for patients, diets and wards (`/api/patients`, `/api/diets`, `/api/wards`)
- Authentication via HTTP headers: `X-Username` and `X-Api-Token`
- Every user account has a unique API token

---

## Tech Stack

| Technology | Version |
|---|---|
| ASP.NET Core MVC | .NET 10 |
| Entity Framework Core | 8.0.0 |
| SQLite | via EF Core |
| Bootstrap | 5.x |
| C# | 13 |

---

## Database Schema

```
Diet        — Id, Name, Kcal, DietType
Patient     — Id, Name, Surname, Pesel, DietId (FK), WardId (FK)
Ward        — Id, Name, Floor
User        — Id, Name, Surname, LoginName, PasswordHash, ApiToken, UserRole
```

### Relationships
- `Patient` → `Diet` — many-to-one (a patient has one diet)
- `Patient` → `Ward` — many-to-one (a patient belongs to one ward)
- `Diet` → `Patient[]` — one-to-many (a diet can have many patients)
- `Ward` → `Patient[]` — one-to-many (a ward can have many patients)

---

## User Roles

| Role | Permissions |
|---|---|
| **Admin** | Full access to all resources; manages user accounts |
| **Nurse** | Access to patients, wards, diets and reports |
| **Dietician** | Access to patients, wards, diets and reports |

---

## Running the Application

### Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Steps

```bash
# 1. Navigate to the project folder
cd DietPlaner

# 2. Start the application
dotnet run
```

On first run the application automatically:
- Applies all pending EF Core migrations
- Creates the SQLite database file
- Populates the database with seed data

The application will be available at: `http://localhost:5000`

---

## Seed Data

The following records are inserted on first run (when tables are empty):

### Users

| Login | Password | Role | API Token |
|---|---|---|---|
| `admin` | `admin123` | Admin | `a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4` |
| `akowalska` | `nurse123` | Nurse | `b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5` |
| `pnowak` | `nurse123` | Nurse | `c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6` |
| `mwisniewska` | `diet123` | Dietician | `d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1` |
| `tzajac` | `diet123` | Dietician | `e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2` |

### Diets
| Name | Kcal | Type |
|---|---|---|
| Low Fat | 1500 | lowFat |
| Vegetarian | 1800 | vegetarian |
| Standard | 2000 | lowFat |

### Wards
| Name | Floor |
|---|---|
| Internal Medicine | 1 |
| Cardiology | 2 |
| Orthopedics | 3 |

### Patients (10 records)

| Name | Surname | PESEL | Diet | Ward |
|---|---|---|---|---|
| Jan | Kowalski | 65041512345 | Low Fat | Cardiology |
| Zofia | Wójcik | 48092367890 | Vegetarian | Internal Medicine |
| Marek | Kamiński | 72031598765 | Standard | Orthopedics |
| Barbara | Lewandowska | 55060234567 | Low Fat | Cardiology |
| Krzysztof | Zieliński | 80112056789 | Vegetarian | Internal Medicine |
| Halina | Szymańska | 42031878901 | Standard | Orthopedics |
| Robert | Wójcik | 91042312345 | Low Fat | Internal Medicine |
| Irena | Dąbrowska | 53072489012 | Vegetarian | Cardiology |
| Grzegorz | Kozłowski | 78052934567 | *(none)* | Orthopedics |
| Elżbieta | Mazur | 61083145678 | Standard | *(none)* |

> The last two patients are intentionally missing assignments to demonstrate the **Unassigned Patients** report.

---

## REST API

### Authentication

Every request must include the following headers:

```
X-Username: admin
X-Api-Token: a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4
```

Missing or invalid headers return `401 Unauthorized`.

### Endpoints

#### Patients `/api/patients`

| Method | URL | Description |
|---|---|---|
| GET | `/api/patients` | List all patients |
| GET | `/api/patients/{id}` | Get patient by ID |
| POST | `/api/patients` | Create a patient |
| PUT | `/api/patients/{id}` | Update a patient |
| DELETE | `/api/patients/{id}` | Delete a patient |

Example POST / PUT body:
```json
{
  "name": "Jan",
  "surname": "Kowalski",
  "pesel": "90010112345",
  "dietId": 1,
  "wardId": 2
}
```

#### Diets `/api/diets`

| Method | URL | Description |
|---|---|---|
| GET | `/api/diets` | List all diets |
| GET | `/api/diets/{id}` | Get diet by ID |
| POST | `/api/diets` | Create a diet |
| PUT | `/api/diets/{id}` | Update a diet |
| DELETE | `/api/diets/{id}` | Delete a diet |

Example POST / PUT body:
```json
{
  "name": "Diabetic",
  "kcal": 1600,
  "dietType": 0
}
```

> `dietType`: `0` = lowFat, `1` = vegetarian

#### Wards `/api/wards`

| Method | URL | Description |
|---|---|---|
| GET | `/api/wards` | List all wards |
| GET | `/api/wards/{id}` | Get ward by ID |
| POST | `/api/wards` | Create a ward |
| PUT | `/api/wards/{id}` | Update a ward |
| DELETE | `/api/wards/{id}` | Delete a ward |

---

## API Demo Program

A console demo program is located in `DietPlanerApiDemo/`.

```bash
# Terminal 1 — start the web application
cd DietPlaner && dotnet run

# Terminal 2 — run the demo
cd DietPlanerApiDemo && dotnet run
```

The demo performs the following steps in order:
1. GET all diets
2. GET all wards
3. GET all patients (before insert)
4. POST — create a new patient
5. GET — fetch the created patient by ID
6. PUT — update the patient's diet
7. GET — verify the update
8. DELETE — remove the patient
9. GET — verify deletion (expects 404)
10. GET with wrong token (expects 401)
