using Microsoft.EntityFrameworkCore;
using RwaMovies.Models;
using RwaMovies.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<RwaMoviesContext>(options =>
{
    options.UseSqlServer("name=ConnectionStrings:RwaMoviesConnStr");
});
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IVideosService, VideosService>();
builder.Services.AddScoped<IGenresService, GenresService>();
builder.Services.AddScoped<ITagsService, TagsService>();
builder.Services.AddScoped<IImagesService, ImagesService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Videos}/{action=Index}/{id?}");

app.Run();
