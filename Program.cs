﻿using DiplomService.Database;
using DiplomService.Models;
using DiplomService.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DiplomService.Controllers.ApiContollers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationContext>();

//builder.Services.AddHangfire(opt => opt.UseSqlServerStorage(connectionString));
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    // Получение UserManager<User> из области видимости запроса
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var chatServer = new ChatServer(new ApplicationContext(), userManager);
    new Thread(new ThreadStart(chatServer.Start)).Start();
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHangfireDashboard("/dashboard");

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
