using System.Text;
using System.Text.Json.Serialization;
using Backend.Data;
using Backend.Services.Authentication;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration secret = configurationBuilder.AddUserSecrets<Program>().Build();
IConfiguration config = configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();


AddServices();
ConfigureSwagger();
AddDbContext();
AddAuthentication();
AddIdentity();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
scope.ServiceProvider.GetService<YtDbContext>().Database.Migrate();

authenticationSeeder.AddRoles();
authenticationSeeder.AddAdmin();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void AddServices()
{
    builder.Services.AddControllers();
    builder.Services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler =  ReferenceHandler.IgnoreCycles);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<AuthenticationSeeder>();
    builder.Services.AddScoped<IFavoriteVideoRepository, FavoriteVideoRepository>();
    builder.Services.AddScoped<IFeaturedVideoRepository, FeaturedVideoRepository>();
}

void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
}

void AddDbContext()
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<YtDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOption =>
        {
            sqlOption.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        }));
}

void AddAuthentication()
{
    var validIssuer = Environment.GetEnvironmentVariable("JWSETTINGS__VALIDISSUER") ?? config["JwtSettings:ValidIssuer"];
    var validAudience = Environment.GetEnvironmentVariable("JWSETTINGS__VALIDAUDIENCE") ?? config["JwtSettings:ValidAudience"];
    var issuerSigningKey = Environment.GetEnvironmentVariable("SIGNINGKEY__ISSUERSIGNINGKEY") ?? secret["SigningKey:IssuerSigningKey"];
    
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(issuerSigningKey)
                ),
            };
        });
}

void AddIdentity()
{
    builder.Services
        .AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<YtDbContext>();
}