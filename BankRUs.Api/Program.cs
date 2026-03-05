using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using BankRUs.Application.Authentication;
using BankRUs.Application.Authentication.AuthenticateUser;
using BankRUs.Application.Identity;
using BankRUs.Application.Repositories;
using BankRUs.Application.UseCases.Deposit;
using BankRUs.Application.UseCases.OpenAccount;
using BankRUs.Application.UseCases.OpenBankAccount;
using BankRUs.Application.UseCases.Withdrawal;
using BankRUs.Infrastructure.Configuration;
using BankRUs.Intrastructure.Autentication;
using BankRUs.Intrastructure.Identity;
using BankRUs.Intrastructure.Persistance;
using BankRUs.Intrastructure.Repositories;
using BankRUs.Intrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IEmailSender = BankRUs.Application.Services.IEmailSender;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");

var localPassword = Environment.GetEnvironmentVariable("SQL_SA_PASSWORD");
var keyVaultUri = Environment.GetEnvironmentVariable("KEYVAULT_URI");

if (!string.IsNullOrWhiteSpace(localPassword))
{
    connectionString += $";Password={localPassword};";
}
else if (!string.IsNullOrWhiteSpace(keyVaultUri) && builder.Environment.IsProduction())
{
    var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
    KeyVaultSecret secret = await secretClient.GetSecretAsync("BankRUsSqlPassword");
    connectionString = connectionString?.Replace("{BANKRUS_SQL_PASSWORD}", secret.Value);
}
else
{
    throw new InvalidOperationException(
        "No SQL password found! " +
        "Set SQL_SA_PASSWORD for local dev, or KEYVAULT_URI for Azure."
    );
}

// Registrera ApplicationDbContext i DI-containern
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString));

builder.Services
  .AddIdentity<ApplicationUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

// Command/Query handlers
builder.Services.AddScoped<OpenAccountHandler>();
builder.Services.AddScoped<OpenBankAccountHandler>();
builder.Services.AddScoped<AuthenticateUserHandler>();

builder.Services.AddScoped<DepositHandler>();
builder.Services.AddScoped<WithdrawalHandler>();

// Services
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenService, TokenService>();

if (builder.Environment.IsDevelopment())
{
    // Utveckling
    builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
}
else
{
    // Produktion
    builder.Services.AddScoped<IEmailSender, EmailSender>();
}

// Repositories
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IBankAccountTransactionRepository, BankAccountTransactionRepository>();

// 3 typer av livslängder på objekt
// - singleton = ett och samma objekt delas mellan alla andra under hela applikations livslängd
// - scoped = varje HTTP-reqeust f�r sin egen isntans som sen delas av alla objekt inom denna request
// - transitent = varje objekt f�r alltid sin egna instans av typen

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services
  .AddAuthentication(options =>
  {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options =>
  {
      var jwt = builder.Configuration
        .GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>()!;

      options.RequireHttpsMetadata = false; // false endast i dev
      options.SaveToken = true;

      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidIssuer = jwt.Issuer,

          ValidateAudience = true,
          ValidAudience = jwt.Audience,

          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(jwt.Secret)
        ),

          ValidateLifetime = true,
          ClockSkew = TimeSpan.FromSeconds(30),

          NameClaimType = JwtRegisteredClaimNames.Name,
          RoleClaimType = ClaimTypes.Role
      };
  });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "DevMac")
{
    // OpenAPI / Scalar
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();

    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();

// GET /api/me
// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.KMUFsIDTnFmyG3nMiGM6H9FNFUROf3wh7SmqJp-QV30

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
