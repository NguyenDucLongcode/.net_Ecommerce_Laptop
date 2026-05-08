<<<<<<< HEAD
using ComChienMaDui.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

=======
﻿using ComChienMaDui.Data;
using ComChienMaDui.Models;
using ComChienMaDui.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình EmailSettings từ appsettings.json
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

builder.Services.AddScoped<IEmailService, EmailService>(); // Đăng ký dịch vụ EmailService với DI container

// Cấu hình JwtSettings từ appsettings.json
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

// Lấy JwtSettings để sử dụng trong ứng dụng
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình Authentication với JWT Bearer
builder.Services
    .AddAuthentication(options =>
    {
        // Thiết lập scheme mặc định cho xác thực và thách thức (challenge)
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        // Thiết lập scheme mặc định cho thách thức (challenge) khi xác thực thất bại
        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Cấu hình các tham số xác thực token JWT
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key)
                )
            };
            
        // Thêm cấu hình đọc Token từ Cookie "AuthToken"
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken"))
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IJwtService, JwtService>();

>>>>>>> 94c5e59 (Thêm chưc năng gửi email để xác nhận tài khoản khi đăng kí và thêm chức năng login  bằng jwt để xác thực người dùngCommit 2)
var myConnectionString = builder.Configuration.GetConnectionString("apicon");
builder.Services.AddDbContext<EcommerceLaptopContext>(option => option.UseSqlServer(myConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

<<<<<<< HEAD
=======
app.UseAuthentication();
>>>>>>> 94c5e59 (Thêm chưc năng gửi email để xác nhận tài khoản khi đăng kí và thêm chức năng login  bằng jwt để xác thực người dùngCommit 2)
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


<<<<<<< HEAD
=======

>>>>>>> 94c5e59 (Thêm chưc năng gửi email để xác nhận tài khoản khi đăng kí và thêm chức năng login  bằng jwt để xác thực người dùngCommit 2)
app.Run();
