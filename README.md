# JoinTogether

*Learn, Explore & Connect*

JoinTogether is a web application for people who are new to Malmö, or who want to get to know the city better. Users learn about landmarks in Malmö, test their knowledge with a short quiz, and once they pass (75 % or more) they unlock the ability to create or join activities at that location together with other people.

## 🌍 Live demo on Azure ☁️

<div align="center">

### ✨ Try JoinTogether now! ✨

<a href="https://jointogether-frontend-etdgeygfgaedcegk.swedencentral-01.azurewebsites.net">
  <img src="https://img.shields.io/badge/🚀_Open_the_app-JoinTogether-2EA36B?style=for-the-badge" alt="Open the app" />
</a>
&nbsp;
<a href="https://jointogether-api-eyd5cke9fsd3dwe8.swedencentral-01.azurewebsites.net/api/Location">
  <img src="https://img.shields.io/badge/🔌_Try_the_API-Locations-0078D4?style=for-the-badge" alt="Try the API" />
</a>

<br/><br/>

![Hosted on Azure](https://img.shields.io/badge/Hosted_on-Microsoft_Azure-0078D4?style=flat-square&logo=microsoftazure&logoColor=white)
![Region](https://img.shields.io/badge/Region-Sweden_Central_🇸🇪-FECC02?style=flat-square)
![Deploy](https://img.shields.io/badge/Deploy-GitHub_Actions-2088FF?style=flat-square&logo=githubactions&logoColor=white)

</div>

| | Part | Link |
|:---:|---|---|
| 🗺️ | **Web app** – learn, take quizzes and join activities | [jointogether-frontend…azurewebsites.net](https://jointogether-frontend-etdgeygfgaedcegk.swedencentral-01.azurewebsites.net) |
| ⚙️ | **REST API** – list of Malmö locations (JSON) | [jointogether-api…azurewebsites.net/api/Location](https://jointogether-api-eyd5cke9fsd3dwe8.swedencentral-01.azurewebsites.net/api/Location) |
| 🗄️ | **Database** – Azure SQL Database | *(private)* |

> 💡 **Tip:** Create a new account in the web app, pick a place like 🏰 *Malmö City Library* or 🌀 *Turning Torso*, score **75 % or more** 🎯 on the quiz, and unlock activities 🎉
>
> 😴 The first load can take up to a minute while the Azure app and database wake up.

## Project goals

The project is part of the course *Ämnesövergripande projekt inom systemutveckling* (SYSM9, HT26, Newton) and is carried out together with the course *Agil systemutveckling*.

**Product goal:** make it easier for students, newcomers and people who want to meet others to learn about Malmö's culture and history, discover activities, and find people to do them with.

**Course goals the project demonstrates:**

1. Plan, carry out and follow up work using methods, tools and techniques from the field, including memory management, generic types, delegates, LINQ, asynchronous programming and scenario-based testing.
2. Use practical methods, tools and techniques to plan, carry out and close a system development project (Scrum, sprints, Trello, daily stand-ups).
3. Build and deploy applications to a cloud service (Azure App Service + Azure SQL).
4. Apply knowledge from earlier courses in the programme.
5. Independently plan and carry out a system development project.

**Where the technical requirements show up in the code:**

| Requirement | Where |
|---|---|
| Generic types | `IGenericRepository<T>` / `GenericRepository<T>` in the DAL |
| LINQ | Queries and mapping in `QuizService`, `ActivityService`, `LocationService` |
| Delegates | Lambda expressions passed as `Func<T, bool>` / `Func<T, TResult>` in LINQ queries |
| Asynchronous programming | `async`/`await` through every layer (controllers → services → repositories) |
| Memory management | Scoped `DbContext` lifetime via dependency injection, `await using` for contexts in tests |
| Scenario-based testing | xUnit + Moq tests in `JoinTogether.Tests`, plus a Postman collection |

## Features

- Register and log in (ASP.NET Core Identity + JWT)
- Browse Malmö locations on an interactive Mapbox map
- Read about a location's history and culture
- Take a quiz about the location and get per-question feedback
- Pass with 75 % or more to unlock activities at that location
- Create an activity at a location and see activities for each location
- Join an activity (limited by max participants)

## Tech stack

**Backend**
- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 (SQL Server provider)
- ASP.NET Core Identity + JWT Bearer authentication
- Layered architecture: API → BLL → DAL, with a Shared DTO project

**Frontend**
- React 19 + Vite
- React Router
- Mapbox GL JS

**Database**
- SQL Server LocalDB (local development)
- Azure SQL Database (production)

**Testing**
- xUnit, Moq, EF Core InMemory provider
- Postman collection for manual API testing

**DevOps & tools**
- GitHub Actions (build + test, deploy API, deploy frontend)
- Azure App Service (Linux) for both API and frontend
- Trello for Scrum planning

## Project structure

```
Group1_JoinTogether/
├── .github/workflows/
│   ├── dotnet-tests.yml          # Build + run tests on push/PR to master
│   ├── deploy-api.yml            # Deploy API to Azure App Service
│   └── deploy-frontend.yml       # Build React app and deploy to Azure App Service
│
├── JoinTogether.API/             # ASP.NET Core Web API (entry point)
│   ├── Controllers/
│   │   ├── ActivityController.cs
│   │   ├── AuthController.cs
│   │   ├── LocationController.cs
│   │   └── QuizController.cs
│   ├── Program.cs                # DI, CORS, JWT, migrations + seeding on startup
│   └── appsettings.json
│
├── JoinTogether.BLL/             # Business logic
│   ├── Interfaces/               # IActivityService, IAuthService, ILocationService, IQuizService
│   └── Services/                 # ActivityService, AuthService, LocationService, QuizService
│
├── JoinTogether.DAL/             # Data access
│   ├── Data/
│   │   ├── AppDbContext.cs       # IdentityDbContext<ApplicationUser>
│   │   └── LocationQuizSeeder.cs # Seeds locations and quiz questions
│   ├── Entities/                 # ApplicationUser, Location, QuizQuestion, QuizOption,
│   │                             # QuizAttempt, Activity, ActivityParticipant
│   ├── Migrations/
│   ├── Repositories/             # Generic repository + LocationRepository
│   └── DependencyInjection.cs    # AddDataAccess() extension
│
├── JoinTogether.Shared/          # DTOs shared between layers
│   └── DTOs/                     # ActivityDtos, AuthDtos, LocationDTOs, QuizDtos
│
├── JoinTogether.Tests/           # xUnit tests
│   ├── API/Docs/                 # Postman collection
│   ├── BLL/                      # ActivityService, LocationService, QuizService tests
│   ├── DAL/                      # LocationRepository tests
│   └── Helpers/                  # Test data + in-memory DbContext factory
│
├── jointogether.client/          # React frontend (Vite)
│   ├── src/
│   │   ├── api/                  # httpClient + activity/auth/location/quiz API calls
│   │   ├── components/           # Map, location cards, navbar, form fields
│   │   ├── context/              # AuthContext (login state + token)
│   │   └── pages/                # AuthPage, HomePage, QuizPage, CreateActivityPage
│   ├── .env.example
│   └── package.json
│
└── JoinTogether.API.slnx
```

## Getting started (local development)

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22](https://nodejs.org/)
- SQL Server LocalDB (included with Visual Studio on Windows), or SQL Server in Docker on macOS/Linux
- A free [Mapbox](https://www.mapbox.com/) access token

### 1. Clone the repository

```bash
git clone https://github.com/MammaGula/Group1_JoinTogether.git
cd Group1_JoinTogether
```

### 2. Configure the database connection

On Windows, the default connection string in `appsettings.json` uses LocalDB and works without changes.

On macOS/Linux (Docker), set your own connection string with user secrets so no passwords end up in the repo:

```bash
cd JoinTogether.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=JoinTogetherDb;User Id=sa;Password=<your-password>;TrustServerCertificate=True"
```

### 3. Run the API

```bash
cd JoinTogether.API
dotnet run --launch-profile https
```

The API starts on `https://localhost:7211`. On startup it applies EF Core migrations and seeds the locations and quiz questions automatically, so no manual `dotnet ef database update` is needed.

### 4. Run the frontend

```bash
cd jointogether.client
cp .env.example .env        # then add your Mapbox token to .env
npm install
npm run dev
```

Open http://localhost:5173. By default the frontend talks to `https://localhost:7211/api`. To point it somewhere else, set `VITE_API_BASE_URL` in `.env`.

### 5. Run the tests

```bash
dotnet test JoinTogether.API.slnx
```

## API endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Auth/register` | – | Create an account |
| POST | `/api/Auth/login` | – | Log in and receive a JWT |
| GET | `/api/Location` | – | List all locations |
| GET | `/api/Location/{id}` | – | Location details |
| GET | `/api/Quiz/location/{locationId}` | – | Quiz questions (correct answers hidden) |
| GET | `/api/Quiz/location/{locationId}/status` | ✔ | Has the user passed this quiz? |
| POST | `/api/Quiz/submit` | ✔ | Submit answers and get score + feedback |
| GET | `/api/Activity/location/{locationId}` | – | Activities at a location |
| GET | `/api/Activity/{id}` | – | Activity details with participants |
| POST | `/api/Activity` | ✔ | Create an activity (requires passed quiz) |
| POST | `/api/Activity/{activityId}/join` | ✔ | Join an activity (requires passed quiz) |

## Deployment

Every push to `master` triggers GitHub Actions that build and deploy both apps to Azure App Service (Sweden Central).

Production configuration is set in the Azure Portal, not in the repo:

- **API** (`jointogether-api` → Environment variables): connection string `DefaultConnection` (type SQLAzure), `Cors__AllowedOrigins__0` set to the frontend URL, and optionally `Jwt__Key`.
- **Frontend** (`jointogether-frontend` → Configuration → Startup command): `pm2 serve /home/site/wwwroot --no-daemon --spa`
- **GitHub secret** `VITE_MAPBOX_ACCESS_TOKEN` is injected at build time.

## Created by

Group 1, SYSM9 – Newton Kompetensutveckling, HT26

| Name | GitHub |
|---|---|
| Supaphit Ruengsri | [@MammaGula](https://github.com/MammaGula) |
| Amin Nazari | [@Aminnaz93](https://github.com/Aminnaz93) |
| Gustav Lenander | [@Frirz](https://github.com/Frirz) |
| Rachel | [@Rachel-Sardine007](https://github.com/Rachel-Sardine007) |

