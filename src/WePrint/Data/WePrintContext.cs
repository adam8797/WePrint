using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WePrint.Models;

namespace WePrint.Data
{
    public class WePrintContext : IdentityDbContext<user, Role, Guid>
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

            builder.Entity<user>(e =>
            {
                e.HasMany(x => x.printers).WithOne(x => x.owner).OnDelete(DeleteBehavior.NoAction);
                e.ToTable("Users");
            });

            builder.Entity<organization>(e =>
            {
                e.HasMany(x => x.users).WithOne(x => x.organization).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.projects).WithOne(x => x.organization).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<pledge>(e =>
            {
                e.HasOne(x => x.project).WithMany(x => x.pledges).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.maker).WithMany(x => x.pledges).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<project>(e =>
            {
                e.OwnsOne(x => x.address);
                e.HasMany(x => x.updates).WithOne(x => x.project).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<project_update>(e =>
            {
                e.HasOne(x => x.posted_by).WithMany().OnDelete(DeleteBehavior.NoAction);
            });


            builder.Entity<Role>(e => {
                e.ToTable("Roles");
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

        public DbSet<printer> Printers { get; set; }

        public DbSet<organization> Organizations { get; set; }

        public DbSet<project> Projects { get; set; }

        public DbSet<project_update> ProjectUpdates { get; set; }

        public DbSet<pledge> Pledges { get; set; }
    }
}
