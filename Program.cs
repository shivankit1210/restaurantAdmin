using Microsoft.EntityFrameworkCore;
using RestaurantAdmin.Data;

// Adding for authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //added file for authentication and Failed to determine the https port for redirect.

//<added here -->>

builder.Configuration.AddEnvironmentVariables();
// Get database connection string from environment variables
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new ArgumentNullException("Database connection string is missing.");

// added ends here -->>    

// ✅ Ensure configuration is properly loaded
var configuration = builder.Configuration;

// ✅ Retrieve JWT settings safely
string jwtKey = configuration.GetValue<string>("Jwt:Key")
                ?? throw new ArgumentNullException("Jwt:Key is missing in appsettings.json.");

string jwtIssuer = configuration.GetValue<string>("Jwt:Issuer")
                   ?? throw new ArgumentNullException("Jwt:Issuer is missing in appsettings.json.");

string jwtAudience = configuration.GetValue<string>("Jwt:Audience")
                     ?? throw new ArgumentNullException("Jwt:Audience is missing in appsettings.json.");

//  Add Database Context
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// < added here -->

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// added ends here -->>    

//  Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("https://your-netlify-app.netlify.app", "http://localhost:5173") // Adjust if needed
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

//  Add Authentication and Authorization
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

// ✅ Add API Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Apply CORS before Authentication/Authorization
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

// ✅ Middleware Order is Important
app.UseHttpsRedirection();
app.UseAuthentication();  // ✅ Added missing Authentication middleware
app.UseAuthorization();
app.MapControllers();

app.Run();
