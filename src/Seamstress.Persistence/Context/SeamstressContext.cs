using Seamstress.Domain;
using Microsoft.EntityFrameworkCore;

namespace Seamstress.Persistence.Context
{
  public class SeamstressContext : DbContext //DBContext vem de using entityframeowrkcore
  {
    public SeamstressContext(DbContextOptions<SeamstressContext> options) : base(options) { }  // passa as options que recebemos na chamada construtor para a base() que é o DbContext

    public DbSet<Order> Orders { get; set; } = null!;//tabela do contexto do banco de dados
    public DbSet<User> Users { get; set; } = null!;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<OrderUser>().HasKey(OU => new { OU.OrderId, OU.UserId });
      modelBuilder.Entity<ItemColor>().HasKey(IC => new { IC.ItemId, IC.ColorId });
      modelBuilder.Entity<ItemFabric>().HasKey(IF => new { IF.ItemId, IF.FabricId });
      modelBuilder.Entity<ItemSize>().HasKey(IS => new { IS.ItemId, IS.SizeId });

      modelBuilder.Entity<Order>().HasMany(O => O.ItemOrders)
                                  .WithOne(IO => IO.Order)
                                  .HasForeignKey(IO => IO.OrderId)
                                  .OnDelete(DeleteBehavior.Cascade);
    }
  }
}