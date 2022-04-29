using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DemoApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DemoAppContextConnection");;
var connectionString1 = builder.Configuration.GetConnectionString("AppConfig"); ;

/*builder.Host.ConfigureAppConfiguration(builder =>
{
    //Connect to your App Config Store using the connection string
    builder.AddAzureAppConfiguration(connectionString1);
})
            .ConfigureServices(services =>
            {
                services.AddControllersWithViews();
            });*/

builder.Host.ConfigureAppConfiguration(builder =>
{
    builder.AddAzureAppConfiguration(config => config.Connect(connectionString1).UseFeatureFlags());
});

builder.Services.AddDbContext<DemoAppContext>(options =>
    options.UseSqlite(connectionString));;

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DemoAppContext>();;

var Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddFeatureManagement();

string[] scopes = new string[] { "https://storage.azure.com/user_impersonation" };

builder.Services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi(scopes)
                    .AddInMemoryTokenCaches();

builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI(); //microsoft identity UI



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
