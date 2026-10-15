using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MonAn.Models;

[Table("BIT247672_DishCategories")]
public class DishCategory_BIT247672
{
    [Key, Column("BIT247672_Id")] public int Id { get; set; }
    [Required, MaxLength(100), Column("BIT247672_Name")] public string Name { get; set; }
    [MaxLength(500), Column("BIT247672_Description")] public string Description { get; set; }
    public List<Dish_BIT247672> Dishes { get; set; } = new();
}
