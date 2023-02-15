using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PluralizeService.Core;
using XLab.Common.Interfaces;
using XLab.Web.Data.Entities;

namespace XLab.Web.Data.Context;

public sealed class ApplicationDbContext
    : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{

    private readonly IDateTimeService _dateTimeService;
    private readonly ICurrentUserService<string> _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTimeService, ICurrentUserService<string> currentUserService)
        : base(options)
    {
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var types = new[]
        {
            typeof(User),
            typeof(Role),
            typeof(UserClaim),
            typeof(UserRole),
            typeof(UserLogin),
            typeof(RoleClaim),
            typeof(UserToken)
        };

        foreach (var type in types)
            builder.Entity(type)
                .ToTable(PluralizationProvider.Pluralize(type.Name), "auth");

        // To Set MaxLength for all string Properties
        foreach (var property in builder.Model
                     .GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(string)))
            // skip property that have MaxLength
            if (property.GetMaxLength() == null)
                property.SetMaxLength(256);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        ChangeTracker.DetectChanges();
        var entityEntries = ChangeTracker.Entries<IAuditable>().ToList();
        foreach (var entry in entityEntries)
            switch (entry.State)
            {
                case EntityState.Added:
                    OnAdded(entry);
                    break;
                case EntityState.Modified:
                    OnModified(entry);
                    break;
                case EntityState.Deleted:
                    OnDeleted(entry);
                    break;
            }

        return base.SaveChangesAsync(cancellationToken);
    }


    private void OnDeleted(EntityEntry<IAuditable> entry)
    {
        entry.State = EntityState.Modified;
        entry.Entity.IsActive = false;
        entry.Entity.IsDeleted = true;
        OnModified(entry);
    }

    private void OnModified(EntityEntry<IAuditable> entry)
    {
        entry.Entity.ModifiedBy = _currentUserService.UserId;
        entry.Entity.ModifiedAt = _dateTimeService.Now;
    }

    private void OnAdded(EntityEntry<IAuditable> entry)
    {
        entry.Entity.IsActive = true;
        entry.Entity.IsDeleted = false;
        entry.Entity.CreatedBy = _currentUserService.UserId;
        entry.Entity.CreatedAt = _dateTimeService.Now;
        OnModified(entry);
    }
}