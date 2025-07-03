using System.Text.Json;
using System.Text.Json.Serialization;
using ITventory.Application;
using ITventory.Infrastructure;
using ITventory.Infrastructure.EF.AppInit;
using ITventory.Infrastructure.Identity;
using ITventory.Infrastructure.Identity.RegistrationService;
using ITventory.Shared.Abstractions.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks for Azure Container Apps
builder.Services.AddHealthChecks();

builder.Services.AddShared();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);
builder.Services.AddTransient<ICommandHandler<RegisterUser>, RegisterUserHandler>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDevelopment", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddAuthentication() /*AddCookie(IdentityConstants.ApplicationScheme)*/
    .AddBearerToken(IdentityConstants.BearerScheme);


builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<UserIdentity>(options =>
    {
        options.Password.RequiredLength = 5;
        options.Password.RequireDigit = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Lockout = new LockoutOptions
        {
            MaxFailedAccessAttempts = 100
        };
    })
    .AddEntityFrameworkStores<UserManagerDbContext>()
    .AddApiEndpoints();


builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
    o.TokenLifespan = TimeSpan.FromHours(24));

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<UserManagerDbContext>(options =>
    options.UseNpgsql(configuration.GetSection("Identity:ConnectionString").Value!));

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ITventory", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.UseCors("LocalDevelopment");
app.UseAuthentication();
app.UseAuthorization();

// Configure for container deployment
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add health check endpoint
app.MapHealthChecks("/health");

app.MapControllers(); //.RequireAuthorization();
app.MapIdentityApi<UserIdentity>();


app.Run();

// Make Program class accessible for testing
public partial class Program { }