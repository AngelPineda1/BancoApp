var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
var app = builder.Build();

app.MapControllerRoute(name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.Run();
