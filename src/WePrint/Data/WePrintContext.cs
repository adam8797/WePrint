using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WePrint.Data
{
    public class WePrintContext : IdentityDbContext<User, Role, Guid>
    {
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
            builder.Entity<Job>().OwnsOne(x => x.Address);
            builder.Entity<Job>().HasOne(x => x.Customer).WithMany(x => x.Jobs).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Job>().HasOne(x => x.AcceptedBid);

            builder.Entity<User>().HasMany(x => x.Printers).WithOne(x => x.Owner).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<User>().HasMany(x => x.Reviews).WithOne(x => x.ReviewedUser).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Organization>().HasMany(x => x.Users).WithOne(x => x.Organization).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Organization>().HasMany(x => x.Projects).WithOne(x => x.Organization).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Bid>().HasOne(x => x.Bidder).WithMany(x => x.Bids).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Bid>().HasOne(x => x.Job).WithMany(x => x.Bids).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Comment>().HasOne(x => x.User);
            builder.Entity<Comment>().HasOne(x => x.Parent);

            builder.Entity<Review>().HasOne(x => x.Job);
            builder.Entity<Review>().HasOne(x => x.Reviewer);
            builder.Entity<Review>().HasOne(x => x.ReviewedUser);

            builder.Entity<Pledge>().HasOne(x => x.Project).WithMany(x => x.Pledges).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Pledge>().HasOne(x => x.Maker).WithMany(x => x.Pledges).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Project>().OwnsOne(x => x.Address);
            builder.Entity<Project>().HasMany(x => x.Updates).WithOne(x => x.Project).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProjectUpdate>().HasOne(x => x.PostedBy);

            base.OnModelCreating(builder);
        }


        public DbSet<Job> Jobs { get; set; }

        public DbSet<Printer> Printers { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Project> Projects { get; set; }

        [Obsolete("Dont you dare use this! Use IFileService instead!")]
        public DbSet<File> Files { get; set; }
    }
}
