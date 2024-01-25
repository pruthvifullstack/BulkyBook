using BulkyBookWeb.AzureService;
using BulkyBookWeb.Data;
using BulkyBookWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

//for Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index"; // Set the login path
        options.AccessDeniedPath = "/Home/AccessDenied"; // Set the access denied path
    });

builder.Services.AddScoped<AzureStorageService>();

//for Cross-plateform accessiblity
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Azure Service Bus configuration
builder.Services.AddSingleton<IQueueClient>(x =>
{
    var configuration = x.GetRequiredService<IConfiguration>();
    var connectionString = "Endpoint=sb://bulky-web-app.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=5U+5ZoO+L/i+iZtZ1CZ2vKMRNOve1hD/d+ASbNPyut0=";
    var queueName = "register";

    return new QueueClient(connectionString, queueName);
});

builder.Services.AddSingleton<ServiceBus>();

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

//for cross-plateform access
app.UseCors("AllowAllOrigins");

//for Authentication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
