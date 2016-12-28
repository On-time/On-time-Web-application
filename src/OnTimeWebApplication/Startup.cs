using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnTimeWebApplication.Data;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Services;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
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

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //InitializeApp(app.ApplicationServices);
        }

        private void InitializeApp(IServiceProvider services)
        {
            const string adminRoleName = "Admin";
            const string studentRoleName = "Student";
            const string lecturerRoleName = "Lecturer";
            const string adminUsername3 = "admin3@gmail.com";

            var roleManager = services.GetService<AppRoleManager>();
            var userManager = services.GetService<UserManager<ApplicationUser>>();

            if (roleManager == null || userManager == null)
            {
                throw new NullReferenceException();
            }

            var adminRole = roleManager.FindByNameAsync(adminRoleName).Result;

            if (adminRole == null)
            {
                adminRole = new IdentityRole(adminRoleName);
                roleManager.CreateAsync(adminRole).Wait();
            }

            var studentRole = roleManager.FindByNameAsync(studentRoleName).Result;

            if (studentRole == null)
            {
                studentRole = new IdentityRole(studentRoleName);
                roleManager.CreateAsync(studentRole).Wait();
            }

            var lecturerRole = roleManager.FindByNameAsync(lecturerRoleName).Result;

            if (lecturerRole == null)
            {
                lecturerRole = new IdentityRole(lecturerRoleName);
                roleManager.CreateAsync(lecturerRole).Wait();
            }

            // create admin user3
            var adminUser3 = userManager.FindByNameAsync(adminUsername3).Result;

            if (adminUser3 == null)
            {
                adminUser3 = new ApplicationUser { UserName = adminUsername3, Email = adminUsername3 };
                var result = userManager.CreateAsync(adminUser3, "Tni123456").Result;

                if (!result.Succeeded)
                {
                    throw new Exception("Can't create admin user");
                }

                result = userManager.AddToRoleAsync(adminUser3, adminRoleName).Result;

                if (!result.Succeeded)
                {
                    throw new Exception("Can't add admin role to admin3");
                }
            }
        }
    }
}
