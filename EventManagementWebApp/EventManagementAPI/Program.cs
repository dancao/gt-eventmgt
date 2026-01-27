using AutoMapper.Data;
using EventManagementAPI.AutoMapper;
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
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

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

//builder.Services.AddAutoMapper(cfg =>
//{
//    cfg.AddDataReaderMapping();
//    cfg.AddProfile<MappingProfile>();
//}, typeof(Program));

builder.Services.AddScoped<IValidator<VenueDto>, VenueDtoValidator>();

builder.Services.AddScoped<IEventService, EventService>();

//SQLite
builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseConfiguration"));
var connectionString = builder.Configuration.GetSection("DatabaseConfiguration").GetValue(typeof(string), "ConnectionString")
    ?? "Data Source=Databases/eventmanagement.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString.ToString()));
builder.Services.AddScoped<IVenueRepository, VenueRepository>();

builder.Services.AddHostedService<DatabaseInitializer>();

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(); // Accessible at /swagger
}

app.UseHttpsRedirection();

app.UseCors(corsSpecificOrigins);
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
