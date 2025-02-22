using FileSignatures;
using FileSignatures.Formats;
using ImageBox.BusinessLogic.Interfaces;
using ImageBox.BusinessLogic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ImageBox.BusinessLogic;

public static class Extensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection, string configIssuer, string configAudience, string configKey)
    {
        serviceCollection.AddScoped<IAuthService, AuthService>();
        serviceCollection.AddSingleton<IImageService, ImageService>();

        var bytesKey = Encoding.UTF8.GetBytes(configKey!);

        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                         .AddJwtBearer(x=> x.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateIssuer = true,
                             ValidIssuer = configIssuer,
                             ValidateAudience = true,
                             ValidAudience = configAudience,
                             ValidateLifetime = true,
                             IssuerSigningKey = new SymmetricSecurityKey(bytesKey)
                         });

        var recognised = FileFormatLocator.GetFormats().OfType<Image>();
        var inspector = new FileFormatInspector(recognised);
        serviceCollection.AddSingleton<IFileFormatInspector>(inspector);

        return serviceCollection;
    }
}
