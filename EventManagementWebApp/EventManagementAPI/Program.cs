using EventManagementAPI.Data;
using EventManagementAPI.Repositories;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.Validations;
using EventManagementAPI.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string corsSpecificOrigins = "_corsSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsSpecificOrigins,
        policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
            else
            {
                policy.WithOrigins("https://gtreasury.bamboohr.com/").AllowAnyHeader().AllowAnyMethod();
            }
        });
});

// Validations
builder.Services.AddScoped<IValidator<VenueDto>, VenueDtoValidator>();
builder.Services.AddScoped<IValidator<PricingTierDto>, PricingTierDtoValidator>();
builder.Services.AddScoped<IValidator<EventDto>, EventDtoValidator>();
builder.Services.AddScoped<IValidator<TicketTypeDto>, TicketTypeDtoValidator>();
builder.Services.AddScoped<IValidator<PurchaseTicketDto>, PurchaseTicketDtoValidator>();

// Business Services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<ITicketService, TicketService>();

// SQLite - EF Core
var connectionString = builder.Configuration.GetSection("DatabaseConfiguration").GetValue(typeof(string), "ConnectionString")
    ?? "Data Source=Databases/eventmanagement.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString.ToString()));
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IPricingRepository, PricingRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

// Database Initialize
builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseConfiguration"));
builder.Services.AddHostedService<DatabaseInitializer>();

// ExceptionHandler
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Accessible at /swagger
}

app.UseHttpsRedirection();
app.UseCors(corsSpecificOrigins);
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();
