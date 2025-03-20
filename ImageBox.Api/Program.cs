using ImageBox.Api;
using ImageBox.BusinessLogic;
using ImageBox.DataAccess;
using Scalar.AspNetCore;

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
    //services.AddCors();

    services.AddOpenApi(options => // Add bearer auth option in scalar
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
            x.Servers =
                    [
                        new ScalarServer("http://localhost:8080"),
                        new ScalarServer("https://localhost:8081")
                    ];
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    //app.UseCors(x => x
    //                .AllowAnyMethod()
    //                .AllowAnyHeader()
    //                .SetIsOriginAllowed(origin => true) // allow any origin
    //                                                    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
    //                .AllowCredentials());
}