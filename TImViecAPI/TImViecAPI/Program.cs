using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TImViecAPI.Data;
using System.Security.Claims;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//DbContext
builder.Services.AddSwaggerGen();
// Thêm DbContext
var connectionString = builder.Configuration.GetConnectionString("MySqlConn");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Authentication với Cookie
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/api/register/login"; // Đường dẫn khớp với API login
//        options.LogoutPath = "/api/register/logout";
//        options.ExpireTimeSpan = TimeSpan.FromHours(24);
//        options.SlidingExpiration = true;
//        options.Cookie.HttpOnly = true;
//        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
//            ? CookieSecurePolicy.None // Dev local
//            : CookieSecurePolicy.SameAsRequest; // Prod
//        options.Events.OnValidatePrincipal = context =>
//        {
//            return Task.CompletedTask;
//        };
//    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("NhaTuyenDungOnly", policy => policy.RequireRole("NhaTuyenDung"));
//});

// Configure JWT Authentication
//Cau hinh JWT
// Cấu hình JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
// Đăng ký EmailService
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Model cấu hình SMTP
public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FromEmail { get; set; }
}