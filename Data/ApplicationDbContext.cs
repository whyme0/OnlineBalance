using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBalance.Models;

namespace OnlineBalance.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<TemporaryOperation> TemporaryOperations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fields configuration
            #region User Model Configuration
            builder.Entity<User>()
                .Property(u => u.FirstName).IsRequired();
            builder.Entity<User>()
                .Property(u => u.LastName).IsRequired();
            builder.Entity<User>()
                .Property(u => u.BirthDate).IsRequired();
            builder.Entity<User>()
                .Ignore(u => u.FullName);
            #endregion

            #region Account Model Configuration
            builder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany(u => u.Accounts);
            builder.Entity<Account>()
                .Property(a => a.Balance).HasDefaultValue(0);
            builder.Entity<Account>()
                .Property(a => a.Currency).IsRequired().HasConversion<int>();
            builder.Entity<Account>()
                .Property(a => a.Number).IsRequired();
            builder.Entity<Account>()
                .HasIndex(a => a.Number).IsUnique();
            #endregion
            
            #region Operation Model Configuration
            builder.Entity<Operation>()
                .Property(o => o.Amount).IsRequired();
            builder.Entity<Operation>()
                .Property(o => o.Description)
                .HasMaxLength(250);
            builder.Entity<Operation>()
                .Property(o => o.Date).IsRequired();
            builder.Entity<Operation>()
                .Property(o => o.SenderNumber).IsRequired();
            builder.Entity<Operation>()
                .Property(o => o.RecipientNumber).IsRequired();
            #endregion

            #region TemporaryOperation Model Configuration
            // Nothing
            #endregion

            // Ignore models
            builder.Ignore<CreateUserDTO>();

            // Override default table names
            builder.Entity<User>(b => b.ToTable("Users"));
            builder.Entity<IdentityRole>(b => b.ToTable("Roles"));
            builder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("RoleClaims"));
            builder.Entity<IdentityUserClaim<string>>(b => b.ToTable("UserClaims"));
            builder.Entity<IdentityUserToken<string>>(b => b.ToTable("Tokens"));
            builder.Entity<IdentityUserLogin<string>>(b => b.ToTable("UserLogins"));
            builder.Entity<IdentityUserRole<string>>(b => b.ToTable("UserRoles"));
        }
    }
}
