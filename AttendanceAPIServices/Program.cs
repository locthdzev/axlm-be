using System.Text;
using AttendanceAPIServices.Middlewares;
using AttendanceAPIServices.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.AttendanceRepositories;
using Repositories.AttendDetaillsRepositories;
using Repositories.ClassRepositories;
using Repositories.UserRepositories;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80); // Lắng nghe tất cả địa chỉ IP
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Set up Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Attendance API Service",
        Description = "API documentation for the AcademiX Learning Management System",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token below. Example: 'Bearer 12345abcde'",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{ }
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"]
    };
});

//Connect Database
builder.Services.AddDbContext<AXLMDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Subscribe Services and Repositories
builder.Services.AddScoped<IAttendanceServices, AttendanceServices>();

builder.Services.AddTransient<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddTransient<IAttendDetaillsRepository, AttendDetaillsRepository>();
builder.Services.AddTransient<IClassRepository, ClassRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API Service V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthorizeMiddleware>();
app.MapControllers();

app.Run();
