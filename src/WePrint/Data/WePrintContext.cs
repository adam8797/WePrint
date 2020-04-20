using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WePrint.Models;

namespace WePrint.Data
{
    public class WePrintContext : IdentityDbContext<User, Role, Guid>
    {
        public WePrintContext()
        {

        }

        public WePrintContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(e =>
            {
                e.HasMany(x => x.Printers).WithOne(x => x.Owner).OnDelete(DeleteBehavior.NoAction);
                e.ToTable("Users");
            });

            builder.Entity<Organization>(e =>
            {
                e.HasMany(x => x.Users).WithOne(x => x.Organization).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.Projects).WithOne(x => x.Organization).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Pledge>(e =>
            {
                e.HasOne(x => x.Project).WithMany(x => x.Pledges).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.Maker).WithMany(x => x.Pledges).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Project>(e =>
            {
                e.OwnsOne(x => x.Address);
                e.HasMany(x => x.Updates).WithOne(x => x.Project).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<ProjectUpdate>(e =>
            {
                e.HasOne(x => x.PostedBy).WithOne().HasPrincipalKey<User>().OnDelete(DeleteBehavior.NoAction); 
                e.HasOne(x => x.EditedBy).WithOne().HasPrincipalKey<User>().OnDelete(DeleteBehavior.NoAction);
            });


            builder.Entity<Role>(e => {
                e.ToTable("Roles");
                e.HasData(new Role[]
                {
                    new Role()
                    {
                        Id = Guid.NewGuid(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Name = "Administrator",
                        NormalizedName = "ADMINISTRATOR",
                    }
                });
            });

            builder.Entity<IdentityUserRole<Guid>>(e => {
                e.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<Guid>>(e => {
                e.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(e => {
                e.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<Guid>>(e => {
                e.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<Guid>>(e => {
                e.ToTable("UserTokens");
            });
        }

        public DbSet<Printer> Printers { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectUpdate> ProjectUpdates { get; set; }

        public DbSet<Pledge> Pledges { get; set; }
    }
}
