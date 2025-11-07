//using DentalNova.Repository.DataContext;
//using Microsoft.EntityFrameworkCore;
using DentalNova.Business.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddBusinessLogicServices(builder.Configuration);
builder.Services.AddControllersWithViews();

// EF Core (SQL Server)
//Obtenemos la cadena de conexión desde appsettings.json
/* var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//gregamos el DbContext al contenedor de servicios.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
