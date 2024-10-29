using System.Text;
using AssignmentAPIServices.Middlewares;
using AssignmentAPIServices.Services;
using Data.Entities;
using Data.Utilities.CloudStorage;
using Data.Utilities.Excel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.AssignmDetailsRepositories;
using Repositories.AssignmentRepositories;
using Repositories.SubmissionRepositories;
using Repositories.UserRepositories;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Set up Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Assignment API Service",
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
builder.Services.AddScoped<IAssignmentServices, AssignmentServices>();
builder.Services.AddScoped<IExcel, Excel>();
// Cấu hình DI cho CloudStorageSettings và CloudStorage
builder.Services.Configure<CloudStorageSettings>(builder.Configuration.GetSection("CloudStorageSettings"));
builder.Services.AddScoped<ICloudStorage>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<CloudStorageSettings>>().Value;
    return new CloudStorage(settings);
});

builder.Services.AddTransient<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddTransient<IAssignmDetailsRepository, AssignmDetailsRepository>();
builder.Services.AddTransient<ISubmissionRepository, SubmissionRepository>();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assignment API Service V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthorizeMiddleware>();
app.MapControllers();

app.Run();