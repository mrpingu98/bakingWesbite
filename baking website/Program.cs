//applies default settings, looks at appsettings.json and loads settings from that file
//also gives us access to any registered services 
using baking_website.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//registering services here 
//this makes sure our application knows about aspnetcore and MVC - bringing in frameworkd services that enable MVC in the app 
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();
//builder.Services.AddScoped<IPieRepository, MockPieRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();
builder.Services.AddDbContext<BethanysPieShopDbContext>(options => {
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:BethanysPieShopDbContextConnection"]);
});

var app = builder.Build();

//once app is built, can start bringing in middleware components 
//they should be placed behind the .Build() statement, but before .Run()

//////////////////////////////////////start of middleware
//middleware for returning static files - configured to look for incoming requests that are static files (e.g. jpg)
//then will look in the www folder for that file and return it 
app.UseStaticFiles();

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
app.MapDefaultControllerRoute();
////////////////////////////////////////// end of middleware

app.Run();


