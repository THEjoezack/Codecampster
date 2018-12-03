﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codecamp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<CodecampDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CodecampDbContextConnection")));

            services.AddIdentity<CodecampUser, IdentityRole>()
                .AddEntityFrameworkStores<CodecampDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddSingleton<IEmailSender, AuthMessageEmailSender>();
            services.AddTransient<ISpeakerBusinessLogic, SpeakerBusinessLogic>();
            services.AddTransient<IUserBusinessLogic, UserBusinessLogic>();
            services.AddTransient<IEventBusinessLogic, EventBusinessLogic>();
            services.AddTransient<ISessionBusinessLogic, SessionBusinessLogic>();
            // Register the event business logic service
            services.AddTransient<EventBusinessLogic>();
            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("AppSettings"));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireSpeakerRole", policy => policy.RequireRole("Speaker"));
                options.AddPolicy("RequireVolunteerRole", policy => policy.RequireRole("Volunteer"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseDatabaseErrorPage();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Enable and use authentication
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Create user roles and admin user
            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<CodecampUser>>();

            string[] roleNames = { "Admin", "Speaker", "Volunteer", "Attendee" };

            IdentityResult roleResult;

            // Create the application roles, if they do not exist
            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Create the site admin user
            var adminUsername = Configuration.GetSection("AppSettings")["AdminUser"];
            var adminEmail = Configuration.GetSection("AppSettings")["AdminUser"];
            var adminPassword = Configuration.GetSection("AppSettings")["AdminPass"];
            if (adminUsername.Length != 0 && adminPassword.Length != 0
                && adminEmail.Length != 0)
            {
                var sysAdmin = new CodecampUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                var _user = await userManager.FindByEmailAsync(adminEmail);
                if (_user == null)
                {
                    var createAdminUserResult
                        = await userManager.CreateAsync(sysAdmin, adminPassword);

                    if (createAdminUserResult.Succeeded)
                        // Assign the sys admin user to the admin role
                        await userManager.AddToRoleAsync(sysAdmin, "Admin");
                }
            }
        }
    }
}
