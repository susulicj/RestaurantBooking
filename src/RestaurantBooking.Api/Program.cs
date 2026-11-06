using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Infrastructure.Persistence;
using RestaurantBooking.Api.Middleware;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Infrastructure.Services;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurant Booking API",
        Version = "v1",
        Description = "API za rezervaciju stolova u restoranu."
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IDiningTableService, DiningTableService>();
builder.Services.AddScoped<IOpeningHoursService, OpeningHoursService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
