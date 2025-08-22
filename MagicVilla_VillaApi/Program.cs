using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Mapper;
using MagicVilla_VillaApi.Services.Implementations;
using MagicVilla_VillaApi.Services.Interfaces;
using MagicVilla_VillaApi.SharedRepo;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(S =>
             S.UseLazyLoadingProxies().UseSqlServer(builder.Configuration["ConnectionStrings:dbconntext"]));
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(VillaProfile));
Log.Logger=new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("logs/villaLogs.txt")
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddScoped<IVillaNumberService, VillaNumberService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
