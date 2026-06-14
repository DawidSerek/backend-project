using ApplicationCore.Mapping;
using Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

builder.Services.RegisterEfModule(builder.Configuration, builder.Environment.ContentRootPath);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();