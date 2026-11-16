# Restaurant Booking API

Backend for reserving tables in a restaurant. I built it to practice ASP.NET Core
and to deal with a real problem: two people trying to book the same table at the same time.

Work in progress.

## Stack
ASP.NET Core 8, EF Core, PostgreSQL, Docker, JWT, FluentValidation, Serilog, xUnit, Testcontainers

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

## API overview

| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/tables`, `/api/opening-hours` | public | Read tables and opening hours |
| POST/PUT/DELETE | `/api/tables`, `/api/opening-hours` | Admin | Manage them |
| GET | `/api/availability?date=&partySize=` | public | Free time slots for a day |
| POST | `/api/reservations` | user | Book a table (smallest free table that fits) |
| GET | `/api/reservations/my` | user | My reservations (paged, filterable) |
| GET | `/api/reservations/{id}` | owner / Admin | One reservation |
| POST | `/api/reservations/{id}/cancel` | owner / Admin | Cancel (users: at least 24h before) |
| GET | `/api/reservations` | Admin | All reservations (paged, filterable) |

List endpoints accept `page`, `pageSize` (max 50), `status`, `from` and `to`
(local restaurant dates, inclusive) and return `items`, `totalCount` and `totalPages`.

## Tests
```bash
dotnet test
```
Docker must be running: the integration tests start a real PostgreSQL container
with Testcontainers.

## Notes
- Projects are split into Domain / Application / Infrastructure / Api so the domain
  doesn't depend on EF Core.
- Times are stored in UTC. Opening hours are local restaurant time, so they are
  converted to UTC before comparing with reservations.
- Opening hours: one row per day of the week, and the restaurant is assumed to close
  before midnight (to keep slot calculation simple).
- Reservations have a fixed duration (90 min) and start on 30 min slots.
- Validation checks the shape of the input; business rules that need the database
  (e.g. duplicate table number) are checked in the services and return 409.
- Double booking is prevented in the database, not only in code: a PostgreSQL
  exclusion constraint (GiST, `tstzrange` with `&&`) rejects overlapping reservations
  on the same table, and cancelled reservations are ignored. A plain unique index
  would only catch identical start times. The service catches the violation and
  tries the next free table before returning 409.
- An integration test fires 12 parallel requests at the same slot and asserts that
  exactly 2 succeed (the two 6-seat tables) and 10 get 409.
- Cancelling sets the status to `Cancelled` instead of deleting, so history is kept
  and the slot is freed. Someone else's reservation returns 404, not 403, so its
  existence is not revealed.
- Pagination, filtering and counting happen in the database (`IQueryable`), with a
  stable sort (`StartTimeUtc`, then `Id`) so pages don't repeat or skip rows.
