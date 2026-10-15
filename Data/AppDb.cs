using Microsoft.EntityFrameworkCore;
using MonAn.Models;
namespace MonAn.Data;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> o) : base(o) { }
    public DbSet<DishCategory_BIT247672> DishCategories { get; set; }
    public DbSet<Dish_BIT247672> Dishes { get; set; }
    public DbSet<DishImage_BIT247672> DishImages { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Dish_BIT247672>()
          .HasOne(d => d.DishCategory).WithMany(c => c.Dishes)
          .HasForeignKey(d => d.DishCategoryId).OnDelete(DeleteBehavior.Restrict);
        mb.Entity<DishImage_BIT247672>()
          .HasOne(i => i.Dish).WithMany(d => d.Images)
          .HasForeignKey(i => i.DishId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<Dish_BIT247672>().HasIndex(d => new { d.Name, d.DishCategoryId }).IsUnique();
    }
}
