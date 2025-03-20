using ImageBox.DataAccess.Interfaces;
using ImageBox.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ImageBox.DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection, IConfigurationManager config)
    {
        AddLifeTimeServices(serviceCollection);

        AddDataBaseService(serviceCollection, config);

        ApplyDatabaseMigrations(serviceCollection);

        return serviceCollection;
    }

    private static void AddLifeTimeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IImageRepository, ImageRepository>();
        serviceCollection.AddScoped<ITagRepository, TagRepository>();
    }

    private static void AddDataBaseService(IServiceCollection serviceCollection, IConfigurationManager config)
    {
        var configConnectionString = config.GetConnectionString("PostgreSQL");

        if (string.IsNullOrWhiteSpace(configConnectionString))
            throw new FormatException("The connection string is missing or incorrectly specified.");

        serviceCollection.AddDbContext<ImageBoxDbContext>(options =>
        {
            options.UseNpgsql(configConnectionString);
        });
    }

    public static void ApplyDatabaseMigrations(IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();


        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ImageBoxDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }
}
