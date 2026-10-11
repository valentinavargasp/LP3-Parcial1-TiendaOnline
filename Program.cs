using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configuración regional para formatos de moneda y números consistentes
var culturaArgentina = new CultureInfo("es-AR");
CultureInfo.DefaultThreadCurrentCulture = culturaArgentina;
CultureInfo.DefaultThreadCurrentUICulture = culturaArgentina;

// Registro de servicios para Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
