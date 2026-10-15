using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MonAn.Data;
using MonAn.Services;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Lang>();
builder.Services.AddDbContext<AppDb>(o =>
    o.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql => sql.CommandTimeout(10)));

var app = builder.Build();

Console.WriteLine("START A: Web host da tao.");

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dishes}/{action=Index}/{id?}");

Console.WriteLine("START B: Dang khoi tao database.");

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();

    Console.WriteLine("START C: Dang kiem tra SQL.");
    var connected = db.Database.CanConnect();
    Console.WriteLine($"START D: SQL connected = {connected}");

    if (!connected)
        throw new Exception("Khong ket noi duoc SQL Server LocalDB.");

    Console.WriteLine("START E: Dang tao database.");
    db.Database.EnsureCreated();

    Console.WriteLine("START F: Dang nap du lieu mau.");
    Seed.Run(db);

    Console.WriteLine("START G: Database va du lieu mau da xong.");
}
catch (Exception ex)
{
    Console.WriteLine("DATABASE ERROR:");
    Console.WriteLine(ex.ToString());
}

Console.WriteLine("START H: Website dang khoi dong.");
app.Run();
