using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MonAn.Models;

[Table("BIT247672_Dishes")]
public class Dish_BIT247672
{
    [Key, Column("BIT247672_Id")] public int Id { get; set; }
    [Required, MaxLength(150), Column("BIT247672_Name")] public string Name { get; set; }
    [Column("BIT247672_Price", TypeName = "decimal(18,2)")] public decimal Price { get; set; }
    [Column("BIT247672_PreparationTime")] public int PreparationTime { get; set; }
    [Column("BIT247672_IsAvailable")] public bool IsAvailable { get; set; } = true;
    [MaxLength(1000), Column("BIT247672_Description")] public string Description { get; set; }
    [Column("BIT247672_DishCategoryId")] public int DishCategoryId { get; set; }
    public DishCategory_BIT247672 DishCategory { get; set; }
    public List<DishImage_BIT247672> Images { get; set; } = new();
}
