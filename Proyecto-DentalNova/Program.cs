//using DentalNova.Repository.DataContext;
//using Microsoft.EntityFrameworkCore;
using DentalNova.Business.Helpers;
using DentalNova.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Proyecto_DentalNova.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddBusinessLogicServices(builder.Configuration);
builder.Services.AddControllersWithViews();


// HttpContext (Cookies) más adelante
builder.Services.AddHttpContextAccessor();

// Cliente HTTP
builder.Services.AddHttpClient<IUsuarioService, UsuarioServiceApi>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    //var baseUrl = cfg["Api:BaseUrl"] ?? "https://sisemp-webapi-gbh9hyezbeapfja6.mexicocentral-01.azurewebsites.net";
    var baseUrl = cfg["Api:BaseUrl"] ?? "http://localhost:5260/";
    http.BaseAddress = new Uri(baseUrl);
    http.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IPacienteService, PacienteServiceApi>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Api:BaseUrl"] ?? "http://localhost:5260/";
    http.BaseAddress = new Uri(baseUrl);
    http.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IOdontologoService, OdontologoServiceApi>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Api:BaseUrl"] ?? "http://localhost:5260/";
    http.BaseAddress = new Uri(baseUrl);
    http.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ITratamientoService, TratamientoServiceApi>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Api:BaseUrl"] ?? "http://localhost:5260/";
    http.BaseAddress = new Uri(baseUrl);
    http.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configuración para IAuthService
builder.Services.AddHttpClient<IAuthService, AuthServiceApi>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    //var baseUrl = cfg["Api:BaseUrl"] ?? "https://sisemp-webapi-gbh9hyezbeapfja6.mexicocentral-01.azurewebsites.net";
    var baseUrl = cfg["Api:BaseUrl"] ?? "http://localhost:5260/";
    http.BaseAddress = new Uri(baseUrl);
    http.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configuración de Autenticación por Cookies para el MVC
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Si no está logueado, ir aquí
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Duración de la cookie
        options.SlidingExpiration = true;
    });

// EF Core (SQL Server)
//Obtenemos la cadena de conexión desde appsettings.json
/* var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//gregamos el DbContext al contenedor de servicios.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
*/

var cultureInfo = new System.Globalization.CultureInfo("es-MX");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // agregado
}
app.UseHttpsRedirection(); // agregado
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
