using Microsoft.AspNetCore.Mvc;
using MonAn.Services;
namespace MonAn.Controllers;

public abstract class AppController : Controller
{
    protected readonly Lang T;
    protected AppController(Lang t) { T = t; }
    protected IActionResult Missing(string key)
    {
        Response.StatusCode = 404;
        return View("~/Views/Shared/NotFound.cshtml", key);
    }
}
