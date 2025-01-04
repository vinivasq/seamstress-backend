using Seamstress.Domain;
using Microsoft.EntityFrameworkCore;
using Seamstress.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Seamstress.Persistence.Context
{
  public class SeamstressContext : IdentityDbContext<User, Role, int,
                                                    IdentityUserClaim<int>, UserRole,
                                                    IdentityUserLogin<int>, IdentityRoleClaim<int>,
                                                    IdentityUserToken<int>>
  {
    public SeamstressContext(DbContextOptions<SeamstressContext> options) : base(options) { }  // passa as options que recebemos na chamada construtor para a base() que é o DbContext

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderUser> OrdersUser { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Sizings> Sizings { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Color> Colors { get; set; } = null!;
    public DbSet<ItemColor> ItemsColors { get; set; } = null!;
    public DbSet<Fabric> Fabrics { get; set; } = null!;
    public DbSet<ItemFabric> ItemsFabrics { get; set; } = null!;
    public DbSet<Size> Sizes { get; set; } = null!;
    public DbSet<ItemSize> ItemsSizes { get; set; } = null!;
    public DbSet<ItemOrder> ItemOrder { get; set; } = null!;
    public DbSet<Set> Sets { get; set; } = null!;
    public DbSet<ItemSizeMeasurement> ItemSizeMeasurements { get; set; } = null!;
    public DbSet<SalePlatform> SalePlatforms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.HasPostgresExtension("unaccent");

      modelBuilder.Entity<UserRole>(userRole =>
        {
          userRole.HasKey(UR => new { UR.UserId, UR.RoleId });

          userRole.HasOne(UR => UR.Role)
                  .WithMany(R => R.UserRoles)
                  .HasForeignKey(ur => ur.RoleId)
                  .IsRequired();

          userRole.HasOne(UR => UR.User)
                  .WithMany(U => U.UserRoles)
                  .HasForeignKey(ur => ur.UserId)
                  .IsRequired();
        }
      );

      modelBuilder.Entity<OrderUser>().HasKey(OU => new { OU.OrderId, OU.UserId });
      modelBuilder.Entity<ItemColor>().HasKey(IC => new { IC.ItemId, IC.ColorId });
      modelBuilder.Entity<ItemFabric>().HasKey(IF => new { IF.ItemId, IF.FabricId });
      modelBuilder.Entity<ItemSize>().HasMany(IS => IS.Measurements)
                                     .WithOne(ISM => ISM.ItemSize)
                                     .HasForeignKey(ISM => ISM.ItemSizeId)
                                     .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Order>().HasMany(O => O.ItemOrders)
                                  .WithOne(IO => IO.Order)
                                  .HasForeignKey(IO => IO.OrderId)
                                  .OnDelete(DeleteBehavior.Cascade);


      modelBuilder.Entity<Customer>()
            .Property(c => c.IsActive)
            .HasDefaultValue(true);

      modelBuilder.Entity<Item>()
            .Property(i => i.IsActive)
            .HasDefaultValue(true);

      modelBuilder.Entity<Color>()
            .Property(c => c.IsActive)
            .HasDefaultValue(true);

      modelBuilder.Entity<Fabric>()
            .Property(f => f.IsActive)
            .HasDefaultValue(true);

      modelBuilder.Entity<Set>()
            .Property(s => s.IsActive)
            .HasDefaultValue(true);

      modelBuilder.Entity<Size>()
            .Property(s => s.IsActive)
            .HasDefaultValue(true);

      modelBuilder.Entity<User>()
            .Property(u => u.IsActive)
            .HasDefaultValue(true);
    }
  }
}