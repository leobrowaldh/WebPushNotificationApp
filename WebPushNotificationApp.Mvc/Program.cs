using Database;
using Database.EntityModels;
using Microsoft.EntityFrameworkCore;
using WebPushNotificationsApp.PushService;
using WebPushNotificationApp.Mvc.Hubs;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Homepage}/{id?}");
app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");  // Map SignalR hub

app.Run();
