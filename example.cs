using Microsoft.EntityFrameworkCore;
using RestaurantAdmin.Data;

// Adding for authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
builder.Configuration.AddEnvironmentVariables();

// Get database connection string from environment variables
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new ArgumentNullException("Database connection string is missing.");

// Retrieve JWT settings from environment variables
string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new ArgumentNullException("JWT Key is missing.");
string jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new ArgumentNullException("JWT Issuer is missing.");
string jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new ArgumentNullException("JWT Audience is missing.");

// Add Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173") // Change this to your frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Add Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowReactApp");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
