using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using onlineShop.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using onlineShop.Services;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Serialization;
using onlineShop.Contracts;
using onlineShop.Repositories;
using Microsoft.AspNetCore.Http;
using onlineShop.Filters;
using Microsoft.Extensions.Logging;

namespace onlineShop
{
    public class Startup
    {
        public readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultUI();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddLogging();

            // custom services
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IFileUploader, FileUploader>();
            services.AddTransient<IBreadcrumbNavBuilder, Services.BreadcrumbNavBuilder>();
            services.AddTransient<IViewMarkupExtractor, ViewMarkupExtractor>();
            services.AddTransient<IAuditTrailService, AuditTrailService>();
            services.AddTransient<ICartManager, CartManager>();

            // repositories
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ISubcategoryRepository, SubcategoryRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IBlogRepository, BlogRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddHttpContextAccessor();
            services.AddAntiforgery(options => {
                options.HeaderName = "X-XSRF-TOKEN";
            });

            // for published demo only
            services.AddTransient<DemoRestrictAdmin>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseStatusCodePagesWithRedirects("/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(config => {
                config.MapRoute("Default",
                    "{controller}/{action}/{id?}",
                    new { controller = "Home", action = "Index" }); 
            });

            // seed roles if don't exists yet
            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Admin", "Manager" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var superUserName = _configuration.GetSection("SuperUser")["Username"];
            var superUser = await userManager.FindByNameAsync(superUserName);

            if (superUser != null)
            {
                await userManager.AddToRoleAsync(superUser, "Admin");
            }
        }
    }
}
