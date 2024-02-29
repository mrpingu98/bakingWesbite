//applies default settings, looks at appsettings.json and loads settings from that file
//also gives us access to any registered services 
using Azure.Core;
using baking_website.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

//registering services here 
//this makes sure our application knows about aspnetcore and MVC - bringing in frameworkd services that enable MVC in the app 
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();
//builder.Services.AddScoped<IPieRepository, MockPieRepository>();
builder.Services.AddScoped<IShoppingCart, ShoppingCart>(sp => ShoppingCart.GetCart(sp));
//invokes GetCart method - passing in the service provider - AddScoped means same shopping cart that is instanstiated in GetCart
//will be used


//The lambda expression sp => ShoppingCart.GetCart(sp) serves as a factory method.
//Whenever an instance of IShoppingCart is requested from the container, this factory method is invoked to create a new instance of
//ShoppingCart.The ShoppingCart.GetCart(sp) method is responsible for initializing and returning the ShoppingCart instance.
// So, in essence, this line of code ensures that whenever the application needs an IShoppingCart instance(typically within the
// scope of an HTTP request in ASP.NET Core), it will get a new instance of ShoppingCart initialized via the GetCart method.
//if no factory method is provided, then a 'default' service is created (just creates an instance of the required class by default,
// with no configuration for it) 

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();
builder.Services.AddDbContext<BethanysPieShopDbContext>(options => {
    options.UseSqlite(
    builder.Configuration["ConnectionStrings:BethanysPieShopDbContextConnection"]);});

var app = builder.Build();

//once app is built, can start bringing in middleware components 
//they should be placed behind the .Build() statement, but before .Run()

//////////////////////////////////////start of middleware
//middleware for returning static files - configured to look for incoming requests that are static files (e.g. jpg)
//then will look in the www folder for that file and return it 
app.UseStaticFiles();
app.UseSession();

//want to see errors in my app 
//this shows like detailed info on errors, but don't want users to see it - wrap if statement to only call it if app is 
//running in development mode
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//middleware so you can navigate to pages 
//sets some defaults for an MVC to route to the pages(views) that we will have
//this is endpoint middleware
app.MapDefaultControllerRoute();  // {controller}/{action}/{id?}
////////////////////////////////////////// end of middleware


DbInitialiser.Seed(app); 
app.Run();


