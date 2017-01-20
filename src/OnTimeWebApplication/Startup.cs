using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using OnTimeWebApplication.Data;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Services;
using OnTimeWebApplication.TokenAuthentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Principal;

namespace OnTimeWebApplication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoleManager<AppRoleManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constant.AdministratorOnly, policy => policy.RequireRole(Constant.AdminRoleName, Constant.SuperAdminRoleName));
                options.AddPolicy(Constant.StudentAndLecturer, policy => policy.RequireRole(Constant.StudentRoleName, Constant.LecturerRoleName));
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            services.Configure<EFCoreOptions>(options => options.ConnectionString = Configuration.GetConnectionString("DefaultConnection"));

            // add auto reoccur attendance checking
            services.AddHangfire(config =>
                config.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<AttendanceCheckingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            #region token authenticatin setup
            // secretKey contains a secret passphrase only your server knows
            var secretKey = "mysupersecret_secretkey!123";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,
                RequireExpirationTime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            var _signInManager = app.ApplicationServices.GetService<SignInManager<ApplicationUser>>();
            var _userManager = app.ApplicationServices.GetService<UserManager<ApplicationUser>>();

            Func <string, string, Task<ClaimsIdentity>> GetIdentity = async (string username, string password) =>
            {
                var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    var claims = await _userManager.GetClaimsAsync(user);

                    await _signInManager.SignOutAsync();

                    return new ClaimsIdentity(new GenericIdentity(username, "Token"), claims);
                }

                // Credentials are invalid, or account doesn't exist
                return null;
            };

            Func<string, string, Task<ClaimsIdentity>> GetIdentityDemo = (string username, string password) =>
            {
                // Don't do this in production, obviously!
                if (username == "SuperAdmin" && password == "123456")
                {
                    return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { }));
                }

                // Credentials are invalid, or account doesn't exist
                return Task.FromResult<ClaimsIdentity>(null);
            };
            #endregion

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseWebSockets();

            app.UseStaticFiles();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
            app.UseIdentity();
            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                Expiration = TimeSpan.FromMinutes(3),
                SigningCredentials = signingCredentials,
                IdentityResolver = GetIdentity
            });

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            //InitializeApp(app.ApplicationServices);
        }

        private void InitializeApp(IServiceProvider services)
        {
            var roleManager = services.GetService<AppRoleManager>();
            var userManager = services.GetService<UserManager<ApplicationUser>>();

            if (roleManager == null || userManager == null)
            {
                throw new NullReferenceException();
            }

            var adminRole = roleManager.FindByNameAsync(Constant.AdminRoleName).Result;

            if (adminRole == null)
            {
                adminRole = new IdentityRole(Constant.AdminRoleName);
                roleManager.CreateAsync(adminRole).Wait();
            }

            var studentRole = roleManager.FindByNameAsync(Constant.StudentRoleName).Result;

            if (studentRole == null)
            {
                studentRole = new IdentityRole(Constant.StudentRoleName);
                roleManager.CreateAsync(studentRole).Wait();
            }

            var lecturerRole = roleManager.FindByNameAsync(Constant.LecturerRoleName).Result;

            if (lecturerRole == null)
            {
                lecturerRole = new IdentityRole(Constant.LecturerRoleName);
                roleManager.CreateAsync(lecturerRole).Wait();
            }

            var superAdminRole = roleManager.FindByNameAsync(Constant.SuperAdminRoleName).Result;

            if (superAdminRole == null)
            {
                superAdminRole = new IdentityRole(Constant.SuperAdminRoleName);
                roleManager.CreateAsync(superAdminRole).Wait();
            }

            // create super admin
            var SuperAdmin = userManager.FindByNameAsync("SuperAdmin").Result;

            if (SuperAdmin == null)
            {
                SuperAdmin = new ApplicationUser { UserName = "SuperAdmin", Email = "superadmin@tni.ac.th" };
                var result = userManager.CreateAsync(SuperAdmin, "123456").Result;

                if (!result.Succeeded)
                {
                    throw new Exception("Can't create admin user");
                }

                // add super admin role
                result = userManager.AddToRoleAsync(SuperAdmin, Constant.SuperAdminRoleName).Result;

                if (!result.Succeeded)
                {
                    throw new Exception("Can't add superadmin role to SuperAdmin");
                }

                // add admin role
                result = userManager.AddToRoleAsync(SuperAdmin, Constant.AdminRoleName).Result;

                if (!result.Succeeded)
                {
                    throw new Exception("Can't add admin role to SuperAdmin");
                }
            }
        }
    }
}
