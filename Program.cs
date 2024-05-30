using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(connectionString).UseLazyLoadingProxies());

builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    // Получение UserManager<User> из области видимости запроса
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
    optionsBuilder.UseSqlServer(connectionString)
                  .UseLazyLoadingProxies();

    var options = optionsBuilder.Options;
    var chatServer = new ChatServer(new ApplicationContext(options));
    new Thread(new ThreadStart(chatServer.Start)).Start();
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
