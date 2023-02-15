using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XLab.Common.Interfaces;
using XLab.Web.Data.Context;
using XLab.Web.Data.Entities;

namespace XLab.Web.Data;

public class DbInitialization : ISupportApplicationInitialization
{
    private readonly ILogger<DbInitialization> _logger;
    private readonly IServiceProvider _serviceProvider;


    public DbInitialization(IServiceProvider serviceProvider, ILogger<DbInitialization> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }


    public async void OnAppInit()
    {
        using var scope = _serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.ChangeTracker.AutoDetectChangesEnabled = false;
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        try
        {
            _logger.LogDebug("Initialize..");
            await DatabaseMigrate(context);
            var canConnect = await context.Database.CanConnectAsync();
            if (canConnect)
            {
                await AddRoles(roleManager);
                await AddUsers(userManager);
            }

            _logger.LogDebug("Database Ready.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error in Seed Data.");
        }
    }
    
    private async Task DatabaseMigrate(DbContext context)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var migrations = pendingMigrations.ToList();
        if (migrations.Any())
        {
            _logger.LogDebug($"Run Pending Migrations {string.Join(", ", migrations)}");
            await context.Database.MigrateAsync();
        }
    }


    private async Task AddUsers(UserManager<User> userManager)
    {
        _logger.LogDebug("Add Users..");

        const string adminMail = "admin@mail.com";
        const string userPassword = "123456";
        var emails = new[] { adminMail, "user@mail.com" };

        foreach (var email in emails)
        {
            if (userManager.Users.Any(u => u.UserName == email)) continue;

            var user = new User
            {
                FirstName = "",
                LastName = "",
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                UserPassword = userPassword,
                IsAdministrator = email == adminMail
            };
            
            await userManager.CreateAsync(user, user.UserPassword);

            if (user.Email != adminMail) continue;
            await userManager.AddToRoleAsync(user, "Admin");
            await userManager.AddClaimAsync(user, new Claim(Constants.AppClaims.IsAdministrator, true.ToString()));
        }
    }
    private async Task AddRoles(RoleManager<Role> roleManager)
    {
        _logger.LogDebug("Add Roles..");

        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (roleManager.Roles.Any(u => u.Name == role)) continue;

            var newRole = new Role
            {
                Name = role,
                NormalizedName = role.ToUpperInvariant()
            };

            await roleManager.CreateAsync(newRole);
        }
    }
}