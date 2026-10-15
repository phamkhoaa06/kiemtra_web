using Microsoft.AspNetCore.Mvc;
namespace MonAn.Controllers;

public class LanguageController : Controller
{
    public IActionResult Set(string lang, string returnUrl)
    {
        Response.Cookies.Append("lang", lang == "vi" ? "vi" : "en",
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true });
        return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
    }
}
