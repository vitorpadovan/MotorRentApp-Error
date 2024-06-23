
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MotorRentApp.Database;

namespace MotorRentApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            #region Swagger Gen
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("user", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Pesquisa De Preço",
                    Description = "Api de usuários"
                });
            });
            #endregion

            #region MySql Config
            var mySqlConnection = builder.Configuration.GetConnectionString("Mysql");
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));
            #endregion

            #region Auth Config
            builder.Services.AddAuthentication().AddBearerToken();
            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#!";
                options.User.RequireUniqueEmail = true;
            });
            #endregion
            
            var app = builder.Build();
            app.MapIdentityApi<IdentityUser>().WithGroupName("user").WithTags("User");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/user/swagger.json", "User Api");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
