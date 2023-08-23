using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQWeb.ExcelCreate.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var appDbContext = services.GetRequiredService<AppDbContext>();

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    await appDbContext.Database.MigrateAsync();

    if (!appDbContext.Users.Any())
    {
        await userManager.CreateAsync(new IdentityUser { UserName = "deneme", Email = "deneme@outlook.com" }, "Password12*");
        await userManager.CreateAsync(new IdentityUser { UserName = "deneme2", Email = "deneme2@outlook.com" }, "Password12*");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
