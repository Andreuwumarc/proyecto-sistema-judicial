using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ProyectoContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectoContext"));
});

// 👈 Necesario para que funcione Session
builder.Services.AddDistributedMemoryCache(); // Guarda la sesión en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo antes que expire la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); // 👈 ¡Esto ya lo tenías bien!

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}")
    .WithStaticAssets();

app.Run();
