using AutoMapper;
using JWTAuthentication.NET6._0.Auth;
using JWTAuthentication.NET6._0.Helpter;
using JWTAuthentication.NET6._0.Mappers;
using JWTAuthentication.NET6._0.Mappers.Contracts;
using JWTAuthentication.NET6._0.MiddleWare;
using JWTAuthentication.NET6._0.Repositories;
using JWTAuthentication.NET6._0.Repositories.Contracts;
using JWTAuthentication.NET6._0.Services;
using JWTAuthentication.NET6._0.Services.Contracts;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

// For Email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailService, SendMailForgotPass>();

// For Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnStr")));

// For Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//for services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

//for mappers
builder.Services.AddScoped<ICategoryMapper, CategoryMapper>();
builder.Services.AddScoped<IProductMapper, ProductMapper>();

//for repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPosterRepository, PosterRepository>();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
        };
    })
    // add login for goole
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    })
    // add login for facebook
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = configuration["Authentication:Facebook:ClientId"];
        facebookOptions.AppSecret = configuration["Authentication:Facebook:ClientSecret"];
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Ensure roles created
CreateRolesAsync(app.Services).GetAwaiter().GetResult();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// --------Configure Middleware----------
// Sử dụng middleware trước khi xử lý yêu cầu tới API
/*app.Use(async (context, next) =>
{
    // Thực hiện một số việc trước khi gửi yêu cầu tới API
    Debug.WriteLine("Before sending request to API inline");

    await next(); // Tiếp tục xử lý yêu cầu tới API

    // Thực hiện một số việc sau khi nhận phản hồi từ API
    Debug.WriteLine("After sending request to API inline");
});

// Sử dụng middleware OutlineMiddleware
app.UseMiddleware<OutlineMiddleware2>();
app.UseMiddleware<OutlineMiddleware>();*/

// Sử dụng Middleware cho định tuyến và xử lý yêu cầu API
// các yêu cầu có đường dẫn bắt đầu bằng /api/users sẽ được điều hướng đến UserController.
//The Endpoint middleware in the preceding diagram executes the filter pipeline for the corresponding app type—MVC or Razor Pages.
/*app.UseRouting();*/
/*app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
       name: "user",
       pattern: "api/users/{action}",
       defaults: new { controller = "User" }
   );
});*/

app.MapControllers();

app.Run();

async Task CreateRolesAsync(IServiceProvider serviceProvider)
{
    // Tạo scope mới
    using (var scope = serviceProvider.CreateScope())
    {
        var scopedServices = scope.ServiceProvider;
        var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { UserRoles.Admin, UserRoles.User };

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
