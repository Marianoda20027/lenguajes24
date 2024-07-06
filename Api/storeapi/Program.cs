//PROGRAM
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using storeapi.Database;
using storeapi.Bussisnes;
using storeapi.Hubs;
using storeapi.Models;
using Microsoft.AspNetCore.SignalR;
using core;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using storeapi.Database;
using storeapi.Business;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<Categories>(); // or AddScoped/AddTransient based on your requirement

builder.Services.AddSignalR();

// Add SwaggerGen for Swagger UI
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Asegúrate de que este es el origen correcto
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Esto es importante para SignalR
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:7043",
            ValidAudience = "http://localhost:7043",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TheSecretKeyNeedsToBePrettyLongSoWeNeedToAddSomeCharsHere"))
        };
    });

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Obtener la cadena de conexión de las variables de entorno o de appsettings.json
string connection = Environment.GetEnvironmentVariable("DB") ?? builder.Configuration.GetConnectionString("MyDbConnection");

if (string.IsNullOrEmpty(connection))
{
    throw new ArgumentNullException(nameof(connection), "connectionString is required.");
}

// Inicializar la conexión de datos
DataConnection.Init(connection);

// Register business logic layer as a scoped service
builder.Services.AddScoped<StoreLogic>();

var app = builder.Build();
CartSave cartSave = new CartSave();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    CampannaDB.CreateMysqlAsync();
    PaymentDB.CreateMysql();
    StoreDB.CreateMysql();
    cartSave.EnsureComprasTableExistsAsync();
    cartSave.EnsureItemsTableExistsAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
   
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.MapHub<CampaignHub>("/CampaignHub");

app.MapControllers();
app.Run();

