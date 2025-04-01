using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Models.Identity;
using EduCodePlatform.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity � ������
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>() // <-- ������ �������� �����
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Razor Pages + MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// 4. ����� ��� �������� ���� (AngleSharp, ExCSS, Jint)
builder.Services.AddScoped<CodeCheckService>();

// ҳ���� ����� ������ app
var app = builder.Build();

// ��������� ������, �� ��������� �������� app
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await RoleInitializer.SeedRolesAndAdminAsync(roleManager, userManager);
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
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
