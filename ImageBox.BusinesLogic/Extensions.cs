using Amazon;
using Amazon.S3;
using FileSignatures;
using FileSignatures.Formats;
using ImageBox.BusinessLogic.Interfaces;
using ImageBox.BusinessLogic.Services;
using ImageBox.Shared.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ImageBox.BusinessLogic;

public static class Extensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection, IConfigurationManager config)
    {
        AddLifeTimeServices(serviceCollection);

        AddAuthenticationService(serviceCollection, config);
        AddS3Service(serviceCollection, config);
        AddFileSignatureService(serviceCollection);

        return serviceCollection;
    }

    private static void AddLifeTimeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthService, AuthService>();
        serviceCollection.AddScoped<IImageFileService, ImageFileService>();
        serviceCollection.AddScoped<IImageS3Service, ImageS3Service>();
        serviceCollection.AddScoped<ITagService, TagService>();
    }


    private static void AddS3Service(IServiceCollection serviceCollection, IConfigurationManager config)
    {
        serviceCollection.Configure<S3Settings>(config.GetSection("S3Settings"));

        serviceCollection.AddSingleton<IAmazonS3>(x =>
        {
            var s3Settings = x.GetRequiredService<IOptions<S3Settings>>().Value;
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region),
                ServiceURL = s3Settings.Endpoint
            };

            return new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, config);
        });
    }

    private static void AddAuthenticationService(IServiceCollection serviceCollection, IConfigurationManager config)
    {
        var configIssuer = config["Jwt:Issuer"];
        var configAudience = config["Jwt:Audience"];
        var configKey = config["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(configIssuer) ||
            string.IsNullOrWhiteSpace(configAudience) ||
            string.IsNullOrWhiteSpace(configKey))
            throw new FormatException("One of the Jwt strings is missing or incorrectly specified.");

        var bytesKey = Encoding.UTF8.GetBytes(configKey!);

        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                         .AddJwtBearer(x =>
                         x.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateIssuer = true,
                             ValidIssuer = configIssuer,
                             ValidateAudience = true,
                             ValidAudience = configAudience,
                             ValidateLifetime = true,
                             IssuerSigningKey = new SymmetricSecurityKey(bytesKey)
                         });
    }

    private static void AddFileSignatureService(IServiceCollection serviceCollection)
    {
        var recognised = FileFormatLocator.GetFormats().OfType<Image>();
        IFileFormatInspector inspector = new FileFormatInspector(recognised);
        serviceCollection.AddSingleton(inspector);
    }
}
