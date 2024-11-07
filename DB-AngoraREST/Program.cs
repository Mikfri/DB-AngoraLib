using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.ApplicationServices;
using DB_AngoraLib.Services.EmailService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.RoleService;
using DB_AngoraLib.Services.SigninService;
using DB_AngoraLib.Services.TokenService;
using DB_AngoraLib.Services.TransferService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using DB_AngoraLib.Services.BreederBrandService;
using DB_AngoraLib.Services.BreederService;

var builder = WebApplication.CreateBuilder(args);

// Load secrets from secrets.json if in Production
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
//-----------------: DB-AngoraLib Services
builder.Services.AddScoped<IGRepository<Rabbit>, GRepository<Rabbit>>();
builder.Services.AddScoped<IRabbitService, RabbitServices>();
builder.Services.AddScoped<IGRepository<User>, GRepository<User>>();
builder.Services.AddScoped<IAccountService, AccountServices>();
builder.Services.AddScoped<IGRepository<ApplicationBreeder>, GRepository<ApplicationBreeder>>();
builder.Services.AddScoped<IApplicationService, ApplicationServices>();
builder.Services.AddScoped<IGRepository<TransferRequst>, GRepository<TransferRequst>>();
builder.Services.AddScoped<ITransferService, TransferServices>();

builder.Services.AddScoped<IGRepository<BreederBrand>, GRepository<BreederBrand>>();
builder.Services.AddScoped<IBreederBrandService, BreederBrandServices>();
builder.Services.AddScoped<IGRepository<Breeder>, GRepository<Breeder>>();
builder.Services.AddScoped<IBreederService, BreederServices>();

builder.Services.AddScoped<IGRepository<RefreshToken>, GRepository<RefreshToken>>();
builder.Services.AddScoped<ITokenService, TokenServices>();
builder.Services.AddScoped<IGRepository<Notification>, GRepository<Notification>>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddTransient<IEmailService, EmailServices>();

builder.Services.AddScoped<Rabbit_Validator>();

// Mine Lib IdentityUser services
builder.Services.AddScoped<ISigninService, SigninServices>();
builder.Services.AddScoped<IRoleService, RoleServices>();

// Bind EmailSettings fra appsettings.json
builder.Services.Configure<Settings_Email>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache

// Log configuration values to verify they are loaded correctly
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"];

Console.WriteLine($"JWT Issuer: {jwtIssuer}");
Console.WriteLine($"JWT Audience: {jwtAudience}");
Console.WriteLine($"JWT Key: {jwtKey}");

// DB CONNECTION-STRING & MIGRATION SETUP
builder.Services.AddDbContext<DB_AngoraContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (builder.Environment.IsProduction())
    {
        connectionString = builder.Configuration["AZURE_SQL_CONNECTION_STRING"];
        Console.WriteLine($"Debug: AZURE_SQL_CONNECTION_STRING: {(string.IsNullOrEmpty(connectionString) ? "NOT FOUND" : "FOUND")}");

        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = builder.Configuration["ConnectionStrings:SecretConnection"];
        }
    }

    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("DB-AngoraREST"));
});


// IDENTITY
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DB_AngoraContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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


//--------: Authorization
builder.Services.AddAuthorization(options =>
{

    //-----------------: USER POLICIES
    options.AddPolicy("ReadUser", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("User:Read", "Own") ||
        context.User.HasClaim("User:Read", "Any")));

    options.AddPolicy("UpdateUser", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("User:Update", "Own") ||
        context.User.HasClaim("User:Update", "Any")));

    //-----------------: RABBIT POLICIES
    options.AddPolicy("UpdateRabbit", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("Rabbit:Update", "Own") ||
        context.User.HasClaim("Rabbit:Update", "Any")));

    options.AddPolicy("DeleteRabbit", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("Rabbit:Delete", "Own") ||
        context.User.HasClaim("Rabbit:Delete", "Any")));
});

//--------: JSON ENUM CONVERTER
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // Soerger for at refence-loop kan haandteres, som er tilf�ldet for Rabbit_PedigreeDTO

});

builder.Services.AddEndpointsApiExplorer(); // Swagger API-dokumentation (///<summary> over dine API end-points vises i UI)

//--------------------: SWAGGER
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//--------: Google.OAuth2, Authentication UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DB-AngoraREST API", Version = "v1" });

    // Tilføj Google OAuth2 konfiguration
    //options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    //{
    //    Type = SecuritySchemeType.OAuth2,
    //    Flows = new OpenApiOAuthFlows
    //    {
    //        AuthorizationCode = new OpenApiOAuthFlow
    //        {
    //            AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
    //            TokenUrl = new Uri("https://oauth2.googleapis.com/token"),  // Dette er Google's token-endpoint som bruges til at validere tokenet
    //            Scopes = new Dictionary<string, string>
    //            {
    //                { "openid", "OpenID" },
    //                { "profile", "Profile" },
    //                { "email", "Email" }
    //            }
    //        }
    //    }
    //});
    // Tilføj JWT Bearer konfiguration
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    // Tilføj sikkerhedskrav for OAuth2 og Bearer-token
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        // Google OAuth2
        //{
        //    new OpenApiSecurityScheme
        //    {
        //        Reference = new OpenApiReference
        //        {
        //            Type = ReferenceType.SecurityScheme,
        //            Id = "oauth2"
        //        },
        //        Scheme = "oauth2",
        //        Name = "oauth2",
        //        In = ParameterLocation.Header,
        //    },
        //    new List<string>() // Scopes her, hvis nødvendigt
        //},
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


//--------: Konfigurer CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
    policy =>
    {
        policy.WithOrigins("https://localhost:7276") // Erstat med den korrekte oprindelse for Swagger UI
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // Tillad credentials såsom cookies, autorisation headers eller TLS klient certifikater
    });
});

var app = builder.Build();

app.UseCors("MyAllowSpecificOrigins");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
});
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseDefaultFiles(); // Middleware to serve default files like index.html

app.UseHttpsRedirection();

app.UseAuthentication();    // IdentityUser setup
app.UseAuthorization();

app.MapControllers();

app.Run();