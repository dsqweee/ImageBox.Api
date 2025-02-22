using ImageBox.Api;
using ImageBox.BusinessLogic;
using ImageBox.DataAccess;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

Configure(app);

app.Run();

void RegisterServices(IServiceCollection services)
{
    var configConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    var configIssuer = builder.Configuration["Jwt:Issuer"];
    var configAudience = builder.Configuration["Jwt:Audience"];
    var configKey = builder.Configuration["Jwt:Key"];

    if (string.IsNullOrWhiteSpace(configConnectionString))
        throw new FormatException("The connection string is missing or incorrectly specified.");

    if (string.IsNullOrWhiteSpace(configIssuer) ||
        string.IsNullOrWhiteSpace(configAudience) ||
        string.IsNullOrWhiteSpace(configKey))
        throw new FormatException("One of the Jwt strings is missing or incorrectly specified.");

    builder.Services.AddDataAccess(configConnectionString);
    builder.Services.AddBusinessLogic(configIssuer, configAudience, configKey);

    services.AddControllers();

    services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });
}

void Configure(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(x=>
        {
            x.WithTheme(ScalarTheme.DeepSpace);
            x.WithModels(false);
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
