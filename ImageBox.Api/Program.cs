using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using ImageBox.Api;
using ImageBox.BusinessLogic;
using ImageBox.DataAccess;
using ImageBox.Shared.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

Configure(app);

app.Run();

void RegisterServices(IServiceCollection services)
{
    builder.Services.AddDataAccess(builder.Configuration);
    builder.Services.AddBusinessLogic(builder.Configuration);

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
