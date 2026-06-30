using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using Scalar.AspNetCore;
using Microsoft.OpenApi;
using FluentValidation;
using System.Text;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();

var secretKey = builder.Configuration["JwtSettings:SecretKey"] 
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(xmlPath);
            var members = xmlDoc.SelectNodes("//member")?.Cast<System.Xml.XmlNode>();
            if (members != null)
            {
                document.Info.Description = string.Join("\n", members.Select(n => n.InnerText));
            }
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token"
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDocumentRepository, EfDocumentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EfEmployeeRepository>();
builder.Services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<SendDocumentHandler>();
builder.Services.AddScoped<SignDocumentHandler>();
builder.Services.AddScoped<DocumentSignedEventHandler>();
builder.Services.AddScoped<TransferEmployeeHandler>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AddEmployeeRequestValidator>();

var app = builder.Build();

// === ПРОВЕРКА ПОДКЛЮЧЕНИЯ ===
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");

    var masked = connectionString?.Replace("Password=", "Password=***");
    Console.WriteLine($"Строка подключения (пароль скрыт): {masked}");

    try
    {
        bool canConnect = dbContext.Database.CanConnect();
        Console.WriteLine($"Подключение к БД: {(canConnect ? "УСПЕШНО" : "НЕ УДАЛОСЬ")}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка подключения: {ex.ToString()}");
    }
}
// ========================================

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

/* ====== ДОБАВИТЬ:
- Notification
- Аутентификацию/авторизацию (JWT)
- Rate limiting
- CancellationToken
- Docker
- README
- Фронтенд
*/
