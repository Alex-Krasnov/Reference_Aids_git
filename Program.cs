using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Reference_Aids.Data;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<Reference_AIDSContext>(options => options.UseNpgsql(connection));

builder.Services.AddControllersWithViews();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = 2048;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
