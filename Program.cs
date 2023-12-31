using System.Text;
using ArzonOL.Data;
using ArzonOL.Entities;
using ArzonOL.Repositories;
using ArzonOL.Repositories.Interfaces;
using ArzonOL.Services.AuthService;
using ArzonOL.Services.AuthService.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ArzonOL.Services.CategoryService;
using ArzonOL.Services.CategoryService.Interfaces;
using ArzonOL.Services.ProductServeice.Interfaces;
using ArzonOL.Services.ProductServeice;
using ArzonOL.Services.CartService.Interfaces;
using ArzonOL.Services.CartService;
using Microsoft.Extensions.Caching.Memory;
using ArzonOL.Services.VoterService.Interfaces;
using ArzonOL.Services.VoterService;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
  options.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryApproachService, CategoryApproachService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMailSender, MailSender>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IVoterService, VoterService>();
builder.Services.AddAutoMapper(typeof(Program)); 
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
   
});

builder.Services.AddIdentity<UserEntity, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddSignInManager<SignInManager<UserEntity>>()
        .AddDefaultTokenProviders();
        
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    Console.WriteLine("JwtBearerDefaults.AuthenticationScheme: " + JwtBearerDefaults.AuthenticationScheme);
})
.AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:client_id"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:client_secret"]!;
    })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


var app = builder.Build();

if(app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await InitializeDataService.CreateDefaultAdmin(app);
await InitializeDataService.CreateDefaultRoles(app);
await InitializeDataService.CreateDefaultUser(app);
await InitializeDataService.CreateDefaultMerchand(app);
app.Run();
