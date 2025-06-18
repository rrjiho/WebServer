
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ServerAPI.DB;
using ServerAPI.Middlewares;
using ServerAPI.Services;
using StackExchange.Redis;
using System.Text;

namespace ServerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Game Server API", Version = "v1" });
            });

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRankingService, RankingService>();
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            builder.Services.AddScoped<ISessionService, SessionService>();

            // AWS Connect
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONN");
            var redisConn = Environment.GetEnvironmentVariable("REDIS_CONN");

            var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
            var googleRedirectUri = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
                options.CallbackPath = new PathString("/auth/google/callback");
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("https://servercloud-dev.com") 
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(80);
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Server API v1"));

            app.UseCors("AllowFrontend");

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<SessionAuthMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
