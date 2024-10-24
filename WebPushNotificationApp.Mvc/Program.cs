using Database;
using Database.EntityModels;
using Database.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebPushNotificationsApp.PushService;
using WebPushNotificationApp.Mvc.Hubs;
using Microsoft.AspNet.SignalR;
using dotenv;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();
// Add services to the container.
builder.Services.AddDbContext<WebPushAppContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("WebPushAppConecction")));

builder.Configuration["VapidDetails:Subject"] = Environment.GetEnvironmentVariable("VAPID_SUBJECT") ?? builder.Configuration["VapidDetails:Subject"];
builder.Configuration["VapidDetails:PublicKey"] = Environment.GetEnvironmentVariable("VAPID_PUBLIC_KEY") ?? builder.Configuration["VapidDetails:PublicKey"];
builder.Configuration["VapidDetails:PrivateKey"] = Environment.GetEnvironmentVariable("VAPID_PRIVATE_KEY") ?? builder.Configuration["VapidDetails:PrivateKey"];


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<WebPushAppContext>();

builder.Services.AddSingleton<IPushService, PushService>();

builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddControllersWithViews();


builder.Services.AddSignalR();
//Before using db we will store subscription in session, for testing purposes.
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/Home/Index");
});

app.Run();
