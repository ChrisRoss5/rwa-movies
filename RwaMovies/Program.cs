using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RwaMovies.Models;
using RwaMovies.Services;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<RwaMoviesContext>(options =>
    options.UseSqlServer("name=ConnectionStrings:RwaMoviesConnStr"));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RwaMovies API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        var jwtKey = builder.Configuration["JWT:Key"];
        var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
            ValidateLifetime = true,
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        // https://github.com/dotnet/aspnetcore/issues/9039
        options.Events.OnRedirectToLogin = (ctx) => OnRedirectHandler(in ctx, (int)HttpStatusCode.Unauthorized);
        options.Events.OnRedirectToAccessDenied = (ctx) => OnRedirectHandler(in ctx, (int)HttpStatusCode.Forbidden);
        static Task<int> OnRedirectHandler(in RedirectContext<CookieAuthenticationOptions> ctx, int statusCode)
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
                ctx.Response.StatusCode = statusCode;
            else
                ctx.Response.Redirect(ctx.RedirectUri);
            return Task.FromResult(0);
        }
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
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
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/StaticFiles",
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")),
    OnPrepareResponse = (ctx) =>
    {
        if (!ctx.Context.User.IsInRole("Admin"))
            ctx.Context.Response.Redirect("/Auth/AccessDenied?ReturnUrl=" + ctx.Context.Request.Path);
    }
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Videos}/{action=Index}/{id?}"
);

app.Run();
