
using LIBCORE.Helper;
using LIBCORE.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HTML_FC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<EmailService>();

            // Bind JwtSettings từ appsettings.json
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            // Lấy giá trị từ cấu hình
            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

            // Kiểm tra nếu `jwtSettings` null hoặc thiếu giá trị quan trọng
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
            {
                throw new Exception("Lỗi: Không tìm thấy cấu hình JWT hoặc SecretKey bị thiếu.");
            }

            // Đăng ký Authentication với JWT
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero // Không có độ trễ token
                    };
                });

            // Thêm service khác (Database, Business Layer, ...)
            string? connectionString = builder.Configuration.GetValue<string>("Settings:ConnectionString");
            Functions.AddModelServices(builder.Services);
            Functions.AddDataRepositoryAndBusinessLayerServices(builder.Services, connectionString!);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            // Bật Authentication trước Authorization
            app.UseAuthentication(); // ✅ PHẢI CÓ
            app.UseAuthorization(); // ✅ PHẢI CÓ    
            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }
    }
}
