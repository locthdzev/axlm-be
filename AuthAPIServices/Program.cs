using AuthAPIServices.Services;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Repositories.UserRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Lấy cài đặt JWT từ cấu hình
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Cấu hình Kestrel để lắng nghe tất cả địa chỉ trên cổng 80
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80); // Lắng nghe tất cả địa chỉ IP
});

// Thêm các dịch vụ cần thiết cho ứng dụng
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Thiết lập Swagger để tài liệu API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Auth API Service",
        Description = "API documentation for the AcademiX Learning Management System",
    });
    
    // Cấu hình bảo mật cho Swagger
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
            new string[] { }
        }
    });
});

// Kiểm tra giá trị JWT
if (string.IsNullOrEmpty(jwtSettings["Key"]) || 
    string.IsNullOrEmpty(jwtSettings["Issuer"]) || 
    string.IsNullOrEmpty(jwtSettings["Audience"]))
{
    throw new ArgumentNullException("JWT settings are not configured correctly.");
}

// Cấu hình xác thực JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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

// Kết nối tới cơ sở dữ liệu
builder.Services.AddDbContext<AXLMDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký các dịch vụ và repository
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

// Thêm CORS để cho phép mọi nguồn
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Cấu hình pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API Service V1");
    c.RoutePrefix = string.Empty; // Đặt Swagger UI ở trang gốc
});


app.UseCors("AllowAll"); // Áp dụng chính sách CORS
//app.UseHttpsRedirection(); // Chuyển hướng HTTP sang HTTPS
app.UseAuthentication(); // Xác thực người dùng
app.UseAuthorization(); // Ủy quyền người dùng
app.MapControllers(); // Đăng ký các controller

app.Run(); // Chạy ứng dụng
