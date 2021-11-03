using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IDownloader, Downloader>();
            return services;
        }
    }
}