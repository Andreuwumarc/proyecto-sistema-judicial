using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddDbContext<ProyectoContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MiConexion")));

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",


    pattern: "{controller=Home}/{action=Home}/{id?}")
    .WithStaticAssets();
app.MapControllerRoute(
    name: "dashboard-clean",
    pattern: "Dashboard",
    defaults: new { controller = "Dashboard", action = "Index" });


app.Run();



