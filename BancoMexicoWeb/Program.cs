using BancoMexicoWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
builder.Services.AddTransient<TurnoService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.AccessDeniedPath = "/Home/AccesoDenegado";
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true; // Indica que la cookie solo debe ser accesible desde el servidor
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo enviar cookie sobre HTTPS
    });


var app = builder.Build();


app.MapControllerRoute(name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
         name: "default",
         pattern: "{controller=Home}/{action=Login}/{id?}"
        );


app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.Run();
