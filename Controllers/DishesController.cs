using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonAn.Data;
using MonAn.Models;
using MonAn.Services;
namespace MonAn.Controllers;

public class DishesController : AppController
{
    private readonly AppDb _db;
    public DishesController(AppDb db, Lang t) : base(t) { _db = db; }

    // ---------- LIST: search + filter + price range + sort (all in SQL) ----------
    public async Task<IActionResult> Index(string q, int? categoryId, string status, decimal? minPrice, decimal? maxPrice, string sort)
    {
        bool parseError = ModelState.Any(kv => kv.Value.Errors.Count > 0 && (kv.Key == "minPrice" || kv.Key == "maxPrice"));
        ModelState.Clear();

        var items = _db.Dishes.AsNoTracking().Include(d => d.DishCategory).Include(d => d.Images).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) { var kw = q.Trim(); items = items.Where(d => d.Name.Contains(kw)); }
        if (categoryId.HasValue) items = items.Where(d => d.DishCategoryId == categoryId.Value);
        if (status == "available") items = items.Where(d => d.IsAvailable);
        else if (status == "unavailable") items = items.Where(d => !d.IsAvailable);

        bool badRange = parseError || minPrice < 0 || maxPrice < 0 || (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice);
        if (badRange) ViewBag.RangeError = true;
        else
        {
            if (minPrice.HasValue) items = items.Where(d => d.Price >= minPrice.Value);
            if (maxPrice.HasValue) items = items.Where(d => d.Price <= maxPrice.Value);
        }

        items = sort switch
        {
            "price_asc" => items.OrderBy(d => d.Price),
            "price_desc" => items.OrderByDescending(d => d.Price),
            "time_asc" => items.OrderBy(d => d.PreparationTime),
            _ => items.OrderBy(d => d.Name)
        };
        ViewBag.Categories = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        return View(await items.ToListAsync());
    }

    // ---------- DETAILS ----------
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return Missing("err_dish_nf");
        var dish = await _db.Dishes.AsNoTracking().Include(d => d.DishCategory).Include(d => d.Images)
            .FirstOrDefaultAsync(d => d.Id == id.Value);
        return dish == null ? Missing("err_dish_nf") : View(dish);
    }

    // ---------- CREATE / EDIT ----------
    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View("Form", new Dish_BIT247672 { IsAvailable = true });
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT247672 dish)
    {
        await ValidateAsync(dish);
        if (!ModelState.IsValid) { await LoadCategories(); return View("Form", dish); }
        _db.Dishes.Add(dish);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_saved";
        return RedirectToAction(nameof(Details), new { id = dish.Id });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return Missing("err_dish_nf");
        var dish = await _db.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id.Value);
        if (dish == null) return Missing("err_dish_nf");
        await LoadCategories();
        return View("Form", dish);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int? id, [Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT247672 input)
    {
        if (id == null) return Missing("err_dish_nf");
        var dish = await _db.Dishes.FindAsync(id.Value);
        if (dish == null) return Missing("err_dish_nf");
        input.Id = dish.Id;
        await ValidateAsync(input);
        if (!ModelState.IsValid) { await LoadCategories(); return View("Form", input); }
        dish.Name = input.Name;
        dish.Price = input.Price;
        dish.PreparationTime = input.PreparationTime;
        dish.IsAvailable = input.IsAvailable;
        dish.Description = input.Description;
        dish.DishCategoryId = input.DishCategoryId;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_saved";
        return RedirectToAction(nameof(Details), new { id = dish.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return Missing("err_dish_nf");
        var dish = await _db.Dishes.FindAsync(id.Value);
        if (dish == null) return Missing("err_dish_nf");
        _db.Dishes.Remove(dish);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_deleted";
        return RedirectToAction(nameof(Index));
    }

    // ---------- IMAGES ----------
    [HttpPost]
    public async Task<IActionResult> AddImage(int? id, string imageUrl, bool isThumbnail)
    {
        if (id == null) return Missing("err_dish_nf");
        var dish = await _db.Dishes.Include(d => d.Images).FirstOrDefaultAsync(d => d.Id == id.Value);
        if (dish == null) return Missing("err_dish_nf");

        imageUrl = imageUrl?.Trim();
        if (string.IsNullOrEmpty(imageUrl) || !Uri.TryCreate(imageUrl, UriKind.Absolute, out var u)
            || (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps))
        {
            TempData["Err"] = "err_image_url";
            return RedirectToAction(nameof(Details), new { id = dish.Id });
        }
        bool makeThumb = isThumbnail || !dish.Images.Any();
        if (makeThumb) foreach (var i in dish.Images) i.IsThumbnail = false;
        dish.Images.Add(new DishImage_BIT247672 { ImageUrl = imageUrl, IsThumbnail = makeThumb });
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_image_added";
        return RedirectToAction(nameof(Details), new { id = dish.Id });
    }

    [HttpPost]
    public async Task<IActionResult> SetThumbnail(int? id)
    {
        if (id == null) return Missing("err_img_nf");
        var img = await _db.DishImages.FindAsync(id.Value);
        if (img == null) return Missing("err_img_nf");
        var all = await _db.DishImages.Where(i => i.DishId == img.DishId).ToListAsync();
        foreach (var i in all) i.IsThumbnail = i.Id == img.Id;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_thumb";
        return RedirectToAction(nameof(Details), new { id = img.DishId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteImage(int? id)
    {
        if (id == null) return Missing("err_img_nf");
        var img = await _db.DishImages.FindAsync(id.Value);
        if (img == null) return Missing("err_img_nf");
        int dishId = img.DishId;
        bool wasThumb = img.IsThumbnail;
        _db.DishImages.Remove(img);
        await _db.SaveChangesAsync();
        if (wasThumb)
        {
            var first = await _db.DishImages.Where(i => i.DishId == dishId).OrderBy(i => i.Id).FirstOrDefaultAsync();
            if (first != null) { first.IsThumbnail = true; await _db.SaveChangesAsync(); }
        }
        TempData["Ok"] = "ok_deleted";
        return RedirectToAction(nameof(Details), new { id = dishId });
    }

    // ---------- helpers ----------
    private async Task LoadCategories() =>
        ViewBag.Categories = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

    private async Task ValidateAsync(Dish_BIT247672 d)
    {
        foreach (var k in new[] { "Name", "Price", "PreparationTime", "DishCategoryId" }) ModelState.Remove(k);
        d.Name = d.Name?.Trim();
        if (string.IsNullOrWhiteSpace(d.Name)) ModelState.AddModelError("Name", T["err_name_required"]);
        if (d.Price <= 0) ModelState.AddModelError("Price", T["err_price"]);
        if (d.PreparationTime <= 0) ModelState.AddModelError("PreparationTime", T["err_time"]);
        if (!await _db.DishCategories.AnyAsync(c => c.Id == d.DishCategoryId))
            ModelState.AddModelError("DishCategoryId", T["err_category_missing"]);
        else if (!string.IsNullOrEmpty(d.Name) &&
                 await _db.Dishes.AnyAsync(x => x.Id != d.Id && x.DishCategoryId == d.DishCategoryId && x.Name == d.Name))
            ModelState.AddModelError("Name", T["err_name_dup"]);
    }
}
