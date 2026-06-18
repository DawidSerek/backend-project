using ApplicationCore.Mapping;
using Infrastructure.Modules;
using Infrastructure.Seed;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

builder.Services.RegisterEfModule(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services.AddJwt(new (builder.Configuration));
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

await IdentitySeeder.SeedAsync(app.Services);
await PositionSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();

public partial class Program;