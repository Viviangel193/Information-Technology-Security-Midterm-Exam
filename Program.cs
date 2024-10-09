using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SampleSecureWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Menambahkan layanan ke dalam container
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrasi IUser dengan implementasi UserData (bukan UserRepository)
builder.Services.AddScoped<IUser, UserData>(); // Mendaftarkan IUser dengan implementasinya UserData

// Konfigurasi autentikasi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Jalur untuk login
        options.AccessDeniedPath = "/Account/AccessDenied"; // Jalur untuk akses ditolak
    });

var app = builder.Build();

// Konfigurasi middleware
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

app.UseAuthentication(); // Mengaktifkan autentikasi
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
