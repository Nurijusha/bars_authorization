using System;
using System.Text;
using Authentication.Data;
using Authentication.Dto;
using Authentication.Handlers;
using Authentication.Interfaces;
using Authentication.Services;
using Force.Cqrs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Authentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!WebHostEnvironment.IsDevelopment())
            {
                var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                var databaseUri = new Uri(connectionUrl!);

                var db = databaseUri.LocalPath.TrimStart('/');
                var userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

                var connection =
                    $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
                services.AddDbContext<AuthDbContext>(builder => builder.UseNpgsql(connection));
            }
            else
            {
                services.AddDbContext<AuthDbContext>(builder => builder
                    .UseNpgsql(Configuration.GetConnectionString("Postgres")));
            }
            services.AddIdentityCore<IdentityUser<Guid>>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddSignInManager();
            services.AddControllers();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Authentication", Version = "v1"});
            });
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();
            
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAsyncHandler<UserInput, RegistrationResponse>, RegistrationHandler>();
            services.AddScoped<IAsyncHandler<UserInput, LoginResponse>, LoginHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseCors(builder => builder.AllowAnyOrigin());
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}