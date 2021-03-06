using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuxrobGM.Sdk.ServerAnalytics;
using SuxrobGM.Sdk.ServerAnalytics.Sqlite;
using Syncfusion.Licensing;

using SGM.Domain.Entities.UserEntities;
using SGM.Domain.Interfaces.Repositories;
using SGM.Domain.Interfaces.Services;
using SGM.Infrastructure.Data;
using SGM.Infrastructure.Repositories;
using SGM.Infrastructure.Services;
using SGM.Web.Blog.Utils;

namespace SGM.Web.Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            SyncfusionLicenseProvider.RegisterLicense(Configuration.GetSection("SynLicenseKey").Value);


            // Infrastructure layer
            ConfigureDatabases(services);
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Web layer
            ConfigureIdentity(services);
            services.AddScoped<ImageHelper>();
            services.AddScoped(_ => new SqliteDbContext(Configuration.GetConnectionString("AnalyticsSqliteDbConnection")));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(5);
                options.SlidingExpiration = true;
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/Blog/List", "/");
                options.Conventions.AddPageRoute("/Blog/Index", "/{slug}");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseServerAnalytics(new SqliteAnalyticsRepository())
                .ExcludePath("/js", "/lib", "/css", "/fonts", "/wp-includes", "/wp-admin", "/wp-includes/")
                .ExcludeExtension(".jpg", ".png", ".ico", ".txt", ".php", "sitemap.xml", "sitemap.xsl")
                .ExcludeLoopBack()
                .Exclude(ctx => ctx.Request.Headers["User-Agent"].ToString().ToLower().Contains("bot"));

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private void ConfigureDatabases(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                        Configuration.GetConnectionString("RemoteDbConnection"))
                    .UseLazyLoadingProxies());
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789_.-";
                options.User.RequireUniqueEmail = true;
                //options.SignIn.RequireConfirmedAccount = true;
            });

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    var facebookAuthSection = Configuration.GetSection("Authentication:Facebook");
                    options.AppId = facebookAuthSection["AppId"];
                    options.AppSecret = facebookAuthSection["AppSecret"];
                });
        }
    }
}
