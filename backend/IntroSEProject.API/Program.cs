
using IntroSEProject.API.Configs;
using IntroSEProject.API.Middlewares;
using IntroSEProject.API.RedisCache;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;

namespace IntroSEProject.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            // Add services to the container.
            builder.Services.AddLogging();
            builder.Services.AddControllers();
            builder.Services.AddSingleton<ICacheService, RedisCacheService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);
            builder.Services.AddScoped<ITokenManager, TokenManager>();
            var secretKey = builder.Configuration.GetValue<string>("AuthToken:SecretKey") ?? "";
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("AuthToken:Issuer"),
                    ValidAudience = builder.Configuration.GetValue<string>("AuthToken:Audience"),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        //var tokenManager = context.HttpContext.RequestServices.GetRequiredService<ITokenManager>();
                        //return tokenManager.ValidateAccessToken(context);
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
            builder.Services.Configure<SendGridEmailSenderOptions>(options =>
            {
                options.ApiKey = builder.Configuration.GetValue<string>("SendGrid:ApiKey");
                options.SenderEmail = builder.Configuration.GetValue<string>("SendGrid:SenderEmail");
                options.SenderName = builder.Configuration.GetValue<string>("SendGrid:SenderName");
            });

            var specificOrigin = "_specificOrigin";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: specificOrigin,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:5173")
                                      .AllowCredentials()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader();
                                  });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(specificOrigin);

            app.UseMiddleware<ReadAccessTokenFromCookieMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}