using MonAn.Models;
namespace MonAn.Data;

public static class Seed
{
    public static void Run(AppDb db)
    {
        if (db.DishCategories.Any()) return;
        var starter = new DishCategory_BIT247672 { Name = "Món khai vị", Description = "Món nhẹ dùng trước bữa chính" };
        var main = new DishCategory_BIT247672 { Name = "Món chính", Description = "Món ăn no cho bữa trưa, bữa tối" };
        var dessert = new DishCategory_BIT247672 { Name = "Tráng miệng & Đồ uống", Description = "Chè, cà phê và đồ ngọt" };
        db.DishCategories.AddRange(starter, main, dessert);

        DishImage_BIT247672 Img(string seed, bool thumb) => new() { ImageUrl = $"https://picsum.photos/seed/{seed}/800/600", IsThumbnail = thumb };
        db.Dishes.AddRange(
            new Dish_BIT247672 { Name = "Gỏi cuốn tôm thịt", Price = 45000, PreparationTime = 10, Description = "Bánh tráng cuốn tôm, thịt, bún và rau thơm, chấm tương đậu phộng.", DishCategory = starter, Images = { Img("goicuon1", true) } },
            new Dish_BIT247672 { Name = "Nem rán", Price = 55000, PreparationTime = 20, Description = "Nem chiên giòn nhân thịt, mộc nhĩ và miến.", DishCategory = starter },
            new Dish_BIT247672 { Name = "Phở bò tái", Price = 65000, PreparationTime = 15, Description = "Nước dùng hầm xương 8 tiếng, bò tái mềm, bánh phở tươi.", DishCategory = main, Images = { Img("pho1", true), Img("pho2", false) } },
            new Dish_BIT247672 { Name = "Bún chả Hà Nội", Price = 70000, PreparationTime = 25, Description = "Chả nướng than hoa, bún tươi, nước chấm chua ngọt.", DishCategory = main },
            new Dish_BIT247672 { Name = "Cơm tấm sườn bì chả", Price = 60000, PreparationTime = 20, Description = "Sườn nướng mật ong, bì, chả trứng và đồ chua.", DishCategory = main },
            new Dish_BIT247672 { Name = "Bánh mì thịt", Price = 25000, PreparationTime = 5, Description = "Bánh mì giòn nhân thịt nguội, pate và rau.", DishCategory = main },
            new Dish_BIT247672 { Name = "Chè ba màu", Price = 30000, PreparationTime = 10, Description = "Đậu xanh, đậu đỏ, thạch lá dứa và nước cốt dừa.", DishCategory = dessert },
            new Dish_BIT247672 { Name = "Cà phê sữa đá", Price = 29000, PreparationTime = 5, Description = "Cà phê phin đậm, sữa đặc, nhiều đá.", DishCategory = dessert },
            new Dish_BIT247672 { Name = "Chè khúc bạch", Price = 35000, PreparationTime = 12, IsAvailable = false, Description = "Khúc bạch mềm mịn, hạnh nhân, vải.", DishCategory = dessert }
        );
        db.SaveChanges();
    }
}
