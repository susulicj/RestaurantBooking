# Restaurant Booking API

Backend for reserving tables in a restaurant. I built it to practice ASP.NET Core
and to deal with a real problem: two people trying to book the same table at the same time.

Work in progress.

## Stack
ASP.NET Core 8, EF Core, PostgreSQL, Docker, JWT, FluentValidation, Serilog, xUnit

## Run it

Prerequisites: .NET 8 SDK, Docker Desktop, and the EF tool
(`dotnet tool install --global dotnet-ef --version 8.*`).

```bash
docker compose up -d
dotnet ef database update -p src/RestaurantBooking.Infrastructure -s src/RestaurantBooking.Api
dotnet run --project src/RestaurantBooking.Api
```

Swagger is at `/swagger` (the port is printed in the console).

## Configuration (user-secrets)

Secrets are not stored in the repo. Set them once after cloning:

```bash
dotnet user-secrets init --project src/RestaurantBooking.Api
dotnet user-secrets set "Jwt:Key" "<random string, 32+ chars>" --project src/RestaurantBooking.Api
dotnet user-secrets set "Seed:AdminEmail" "admin@restaurant.com" --project src/RestaurantBooking.Api
dotnet user-secrets set "Seed:AdminPassword" "<your password>" --project src/RestaurantBooking.Api
```

The admin account is created at startup from these values.

## Authentication

- `POST /api/auth/register` creates a user with the `Customer` role.
- `POST /api/auth/login` returns a JWT. In Swagger, click **Authorize** and paste the token.
- Write endpoints for tables and opening hours require the `Admin` role.
  No token gives 401, a Customer token gives 403.

## What works so far
- CRUD for tables (`/api/tables`) and opening hours (`/api/opening-hours`)
- Registration, login, JWT and role-based authorization
- Input validation with FluentValidation (400 with errors per field)
- Global error handling middleware returning ProblemDetails with a `traceId`
- Structured logging with Serilog (console and daily log files)

## Tests
```bash
dotnet test
```

## Notes
- Projects are split into Domain / Application / Infrastructure / Api so the domain
  doesn't depend on EF Core.
- Times are stored in UTC.
- Opening hours: one row per day of the week, and the restaurant is assumed to close
  before midnight (to keep slot calculation simple).
- Validation checks the shape of the input; business rules that need the database
  (e.g. duplicate table number) are checked in the services and return 409.
- (add one thing that was tricky this week, in your own words)