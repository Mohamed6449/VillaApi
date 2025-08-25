using MagicVilla_Web.Mapper;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.Implementation;
using MagicVilla_Web.Services.interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IVillaService, VillaService>();
builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddHttpClient<IVillaNumberService, VillaNumberService>();
builder.Services.AddScoped<IVillaNumberService, VillaNumberService>();
builder.Services.AddHttpClient<IAccountServices, AccountServices>();
builder.Services.AddScoped<IAccountServices, AccountServices>();

#region Session 
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IOTimeout = TimeSpan.FromMinutes(20);// ÇáãÏÉ Çááí ÇÞÕí ãÏÉ íÇÎÏåÇ ÇáÓíÔä æåæ ÈíÚáã ÚãáíÉ 
    option.IdleTimeout = TimeSpan.FromMinutes(10);//ÇáãÏÉ ÈÊÇÚÉ ÚÏã ÇáÊÝÇÚá ãÚ ÇáÓíÑÝÑ 
    option.Cookie.IsEssential = true;// íÚäí íÔÊÛá ÈÏæäåÇ ÚÇÏí æáÇ áÇÒã ßæßí 
    option.Cookie.Path = "/";
    option.Cookie.HttpOnly = true;//ÇãÇä ÇßËÑ 
    option.Cookie.Name = ".MCV_Empity";
});
#endregion

#region Authintication 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.Cookie.HttpOnly = true;
    option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    option.AccessDeniedPath = "/Account/AccessDenied";
    option.SlidingExpiration = true;

});


#endregion

builder.Services.AddAutoMapper(typeof(VillaProfile));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
