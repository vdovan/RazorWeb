using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorWeb.Services;
using RazorWeb.Models;
using RazorWeb.Constants;
using Microsoft.AspNetCore.Identity;
using CommonHelper;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace RazorWeb
{
    public class Startup
    {
        private Dictionary<string, string> strConnList = new Dictionary<string, string>();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

             services.AddOptions();
            var mailConfig = Configuration.GetSection("MailSettings");
            services.Configure<MailConfig>(mailConfig);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddDbContext<AppDbContext>(options =>
            {
                string conEncrypt = Configuration.GetConnectionString("AppDbContext");
                string conStr = string.Empty;

                if (!strConnList.ContainsKey(conEncrypt))
                {
                    conStr = conEncrypt.MyDecrypt(SystemConstants.LegalKey);
                    strConnList.Add(conEncrypt, conStr);
                }
                else
                {
                    conStr = strConnList[conEncrypt];
                }

                options.UseSqlServer(conStr);
            });

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            // services.AddDefaultIdentity<AppUser>()
            // .AddEntityFrameworkStores<AppDbContext>();
            //.AddDefaultTokenProviders();

            // IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Password Rule
                options.Password.RequireDigit = false; 
                options.Password.RequireLowercase = false; 
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequireUppercase = false; 
                options.Password.RequiredLength = 3; 
                options.Password.RequiredUniqueChars = 1; 

                // User Lockout config
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lock 5 minutes
                options.Lockout.MaxFailedAccessAttempts = 5; // Lock after wrong password 5 times
                options.Lockout.AllowedForNewUsers = true;

                // User config
                options.User.AllowedUserNameCharacters = 
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true; 

                // Log in config.
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
