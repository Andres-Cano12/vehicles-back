

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AccessData.Context;
using Microsoft.EntityFrameworkCore;
using BusinnesLogic.Services;
using AccessData.Repository;
using AutoMapper;
using App.Config.Dependencies;
using App.Common.Classes.Base.Repositories;
using AccessData.Entities;
using System.Runtime.Caching.Hosting;
using App.Common.Classes.Cache;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using App.Common.Classes.Validator;
using Microsoft.Extensions.DependencyModel;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetGigs.Models;
using Microsoft.AspNetCore.Identity;
using DotNetGigs.Helpers;
using DotNetGigs.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace WebApi
{
    public class Startup
    {
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));


        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DBContext>(options =>
                                       options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IJwtFactory, JwtFactory>();

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            services.AddIdentity<User, IdentityRole>
                (o =>
                {
                    // configure identity options
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 6;
                    
                })
                 .AddEntityFrameworkStores<DBContext>()
                .AddDefaultTokenProviders();

            services.AddMemoryCache();

            var configMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = configMapper.CreateMapper();

            services.AddSingleton(mapper);
            // RollBar
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Repository
            services.AddScoped<IBaseCRUDRepository<JobSeeker>, JobRepository>();
            services.AddScoped<IBaseCRUDRepository<User>, UserRepository>();
            services.AddScoped<IBaseCRUDRepository<Vehicle>, VehicleRepository>();
            #endregion

            #region Interface
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            #endregion

            #region Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVehicleService, VehicleService>();
            #endregion

            services.AddScoped<IServiceValidator<User>, UserResourceValidator>();
            services.AddScoped<IServiceValidator<Vehicle>, VehicleResourceValidator>();
            #region Others
            services.AddSingleton<App.Common.Classes.Cache.IMemoryCacheManager, MemoryCacheManager>();
            #endregion



            services.Configure<RequestLocalizationOptions>(opts =>
            {
                string english = "en-US";
                string spanish = "es";
                string spanishColombia = "es-CO";

                var supportedCultures = new List<CultureInfo> {
                    new CultureInfo(english),
                    new CultureInfo(spanish),
                    new CultureInfo(spanishColombia)
                };
                opts.DefaultRequestCulture = new RequestCulture(culture: english, uiCulture: english);
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;

            });

            services.AddCors(options =>
            {
                 options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200")
                    );
            });

            services.AddMvc()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
               .AddDataAnnotationsLocalization().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };


            app.UseCors("CorsPolicy");
            app.UseStaticFiles();// For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "Files/imgs")),
                RequestPath = "/Files/imgs"
            });
            //Enable directory browsing
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "Files/imgs")),
                RequestPath = "/Files/imgs"
            });
            //app.UseHttpsRedirection();
            //app.UseAuthentication();
            app.UseMvc();
        }
    }
}
 