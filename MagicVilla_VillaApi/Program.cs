using CQRS_test.MiddleWareException;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Mapper;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Services.Implementations;
using MagicVilla_VillaApi.Services.Interfaces;
using MagicVilla_VillaApi.Services.InterFaces;
using MagicVilla_VillaApi.SharedRepo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(S =>
             S.UseLazyLoadingProxies().UseSqlServer(builder.Configuration["ConnectionStrings:dbconntext"]));

#region Identity DependancyInjection
builder.Services.AddIdentity<User, IdentityRole>(Opt =>
{
    Opt.Password.RequireDigit = true;
    Opt.Password.RequireLowercase = true;
    Opt.Password.RequireUppercase = true;
    Opt.Password.RequireNonAlphanumeric = true;
    Opt.Password.RequiredLength = 6;

    Opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    Opt.Lockout.MaxFailedAccessAttempts = 3;
    Opt.Lockout.AllowedForNewUsers = true;

    //user sitting
    Opt.User.RequireUniqueEmail = true;

    Opt.SignIn.RequireConfirmedPhoneNumber = false;
    Opt.SignIn.RequireConfirmedEmail = true;

    Opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1); // „œ… «·≈€·«ﬁ
    Opt.Lockout.MaxFailedAccessAttempts = 3; // ⁄œœ «·„Õ«Ê·«  «·„”„ÊÕ…
    Opt.Lockout.AllowedForNewUsers = true;  //  ›⁄Ì· «·ﬁ›· ··„” Œœ„Ì‰ «·Ãœœ

})
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserWithClaim", policy => policy.
    RequireAssertion(context => context.User.IsInRole("Admin") && context.User.HasClaim("Create Product", "True")));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});
#endregion

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(VillaProfile));
builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("logs/villaLogs.txt")
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddScoped<IVillaNumberService, VillaNumberService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

#region Authentication
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
        };
    });

builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT Key"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    var provider = builder.Services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        o.SwaggerDoc(description.GroupName,
            new OpenApiInfo
            {
                Title = $"Magic Villa {description.ApiVersion}",
                Version = description.ApiVersion.ToString()
            });
    }
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerUI(options =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                $"Magic Villa {description.ApiVersion}");
    }
    // œ« «··Ì »ÌŒ·Ìﬂ  › Õ ’›Õ… UI ⁄·Ï ÿÊ· „‘ JSON
    options.RoutePrefix = string.Empty;
});
app.UseExceptionHandler("/ErrorHandling/ProcessError");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<MiddleWareExceptionHandler>();
app.MapControllers();
app.Run();