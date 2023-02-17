using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Serilog;
using XLab.Common.Interfaces;
using XLab.Data.Context;
using XLab.Web.Data;
using XLab.Web.Data.Context;
using XLab.Web.Data.Entities;
using XLab.Web.ExtensionMethod;

namespace XLab.Web
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
            //-------Security---------
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.ConfigureWarnings(builder =>
                {
                    builder.Ignore(
                        CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning,
                        RelationalEventId.BoolWithDefaultWarning);
                });

                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<User, Role>()
                .AddDefaultUI()
                .AddClaimsPrincipalFactory<AdditionalUserClaimsPrincipalFactory>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/LogIn";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(6);
            });

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });

            //-------Security---------

            services.AddDistributedMemoryCache();

            services.Configure<IdentityOptions>(Configuration.GetSection("IdentityOptions"));
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddScoped(typeof(ICurrentUserService<>), typeof(CurrentUserService<>));
            services.RegisterAllServices(typeof(ISupportApplicationInitialization));
            services.AddHttpContextAccessor();

            services.AddDbContext<ProjectContext>((_, options) =>
            {
                string? connectionString = Configuration.GetConnectionString("DefaultConnection");

                options.UseSqlServer(connectionString,
                    builder => { builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName); });

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddMvc();
            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                // options.MessageTemplate = "{RemoteIpAddress} {RequestScheme} {RequestHost} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                // Emit debug-level events instead of the defaults
                // options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                };
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}