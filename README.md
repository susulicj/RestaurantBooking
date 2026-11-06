# Restaurant Booking API

Backend for reserving tables in a restaurant. I built it to practice ASP.NET Core
and to deal with a real problem: two people trying to book the same table at the same time.

Work in progress.

## Stack
ASP.NET Core 8, EF Core, PostgreSQL, Docker

## Run it
docker compose up -d
dotnet ef database update -p src/RestaurantBooking.Infrastructure -s src/RestaurantBooking.Api
dotnet run --project src/RestaurantBooking.Api

Swagger is at /swagger (the port is printed in the console).

## What works so far
- CRUD for tables (`/api/tables`)
- CRUD for opening hours (`/api/opening-hours`)
- Global error handling middleware that returns ProblemDetails

## Notes
- Projects are split into Domain / Application / Infrastructure / Api so the domain
  doesn't depend on EF Core.
- Times are stored in UTC.
- Opening hours: one row per day of the week, and the restaurant is assumed to close
  before midnight (to keep slot calculation simple).