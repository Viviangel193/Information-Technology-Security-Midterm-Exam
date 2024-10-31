using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SampleSecureWeb.Data;
using SampleSecureWeb.Middleware;
using SampleSecureWeb.Services;
using Microsoft.AspNetCore.Identity;
using SampleSecureWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Menambahkan layanan dan pengaturan
builder.Services.AddControllersWithViews();

// Konfigurasi Entity Framework dengan SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrasi layanan UserData
builder.Services.AddScoped<IUser, UserData>();

// Menambahkan Memory Cache
builder.Services.AddMemoryCache(); // Tidak perlu deklarasi tambahan di .NET 8

// Menambahkan Session
builder.Services.AddSession();

// Registrasi UserService
builder.Services.AddScoped<IUserService, UserService>();

// Menambahkan PasswordSuggestionService
builder.Services.AddScoped<PasswordSuggestionService>();

// Konfigurasi layanan Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Konfigurasi autentikasi dengan cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Konfigurasi untuk error handling dan HSTS
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Mengaktifkan Session
app.UseSession(); 

// Autentikasi dan otorisasi
app.UseAuthentication();
app.UseAuthorization();

// Middleware untuk memeriksa autentikasi
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated && 
        !context.Request.Path.StartsWithSegments("/Account"))
    {
        context.Response.Redirect("/Account/Login");
        return;
    }
    await next.Invoke();
});

// Middleware Rate Limiting
app.UseMiddleware<RateLimitingMiddleware>();

// Konfigurasi routing
app.MapControllerRoute(
    name: "profile",
    pattern: "Profile/{action=Index}/{id?}",
    defaults: new { controller = "Profile", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Menjalankan aplikasi
app.Run();