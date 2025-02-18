using ImageBox.Api.DataBase;
using ImageBox.Api.Interfaces;
using ImageBox.Api.Repositories;
using ImageBox.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

Configure(app);

app.Run();

void RegisterServices(IServiceCollection services)
{
    var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    services.AddDbContext<ImageBoxDbContext>(options => {
        options.UseNpgsql(connectionString);
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerTokenOptions);

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddSingleton<IImageService, ImageService>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IImageRepository, ImageRepository>();


    services.AddControllers();
    services.AddOpenApi();
}

void Configure(WebApplication app)
{

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}

void JwtBearerTokenOptions(JwtBearerOptions opt)
{
    var configIssuer = builder.Configuration["Jwt:Issuer"];
    var configAudience = builder.Configuration["Jwt:Audience"];
    var configKey = builder.Configuration["Jwt:Key"];

    var bytesKey = Encoding.UTF8.GetBytes(configKey!);

    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configIssuer,
        ValidateAudience = true,
        ValidAudience = configAudience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(bytesKey)
    };
}