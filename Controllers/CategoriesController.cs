using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonAn.Data;
using MonAn.Models;
using MonAn.Services;
namespace MonAn.Controllers;

public class CategoriesController : AppController
{
    private readonly AppDb _db;
    public CategoriesController(AppDb db, Lang t) : base(t) { _db = db; }

    public async Task<IActionResult> Index() =>
        View(await _db.DishCategories.AsNoTracking().Include(c => c.Dishes).OrderBy(c => c.Name).ToListAsync());

    public IActionResult Create() => View("Form", new DishCategory_BIT247672());

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,Description")] DishCategory_BIT247672 cat)
    {
        await ValidateAsync(cat);
        if (!ModelState.IsValid) return View("Form", cat);
        _db.DishCategories.Add(cat);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_saved";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return Missing("err_cat_nf");
        var cat = await _db.DishCategories.FindAsync(id.Value);
        return cat == null ? Missing("err_cat_nf") : View("Form", cat);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int? id, [Bind("Name,Description")] DishCategory_BIT247672 input)
    {
        if (id == null) return Missing("err_cat_nf");
        var cat = await _db.DishCategories.FindAsync(id.Value);
        if (cat == null) return Missing("err_cat_nf");
        input.Id = cat.Id;
        await ValidateAsync(input);
        if (!ModelState.IsValid) return View("Form", input);
        cat.Name = input.Name;
        cat.Description = input.Description;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_saved";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return Missing("err_cat_nf");
        var cat = await _db.DishCategories.FindAsync(id.Value);
        if (cat == null) return Missing("err_cat_nf");
        if (await _db.Dishes.AnyAsync(d => d.DishCategoryId == cat.Id))
        {
            TempData["Err"] = "err_cat_in_use";
            return RedirectToAction(nameof(Index));
        }
        _db.DishCategories.Remove(cat);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "ok_deleted";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateAsync(DishCategory_BIT247672 c)
    {
        ModelState.Remove("Name");
        c.Name = c.Name?.Trim();
        if (string.IsNullOrWhiteSpace(c.Name)) ModelState.AddModelError("Name", T["err_name_required"]);
        else if (await _db.DishCategories.AnyAsync(x => x.Id != c.Id && x.Name == c.Name))
            ModelState.AddModelError("Name", T["err_cat_dup"]);
    }
}
