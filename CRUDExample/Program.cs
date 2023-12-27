using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContracts;
using ServiceContracts;
using Services;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICountriesService, CountryService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepo>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepo>();
builder.Services.AddDbContext<ApplicationDbContext>( options => { options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]); } );



var app = builder.Build();
if(builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();



app.Run();
