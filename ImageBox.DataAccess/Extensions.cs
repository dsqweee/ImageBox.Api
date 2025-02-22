using ImageBox.DataAccess.Interfaces;
using ImageBox.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ImageBox.DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IImageRepository, ImageRepository>();
        serviceCollection.AddScoped<ITagRepository, TagRepository>();

        serviceCollection.AddDbContext<ImageBoxDbContext>(options => {
            options.UseNpgsql(connectionString);
        });

        return serviceCollection;
    }
}
