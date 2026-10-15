using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MonAn.Models;

[Table("BIT247672_DishImages")]
public class DishImage_BIT247672
{
    [Key, Column("BIT247672_Id")] public int Id { get; set; }
    [Required, MaxLength(500), Column("BIT247672_ImageUrl")] public string ImageUrl { get; set; }
    [Column("BIT247672_IsThumbnail")] public bool IsThumbnail { get; set; }
    [Column("BIT247672_DishId")] public int DishId { get; set; }
    public Dish_BIT247672 Dish { get; set; }
}
