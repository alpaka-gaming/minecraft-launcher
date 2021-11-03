using Infrastructure.Interfaces;
using Infrastructure.Services;
using Prism.Ioc;

namespace Infrastructure
{
    public static class Extensions
    {
        public static IContainerRegistry AddInfrastructure(this IContainerRegistry services)
        {
            services.Register<IDownloader, Downloader>();
            return services;
        }
    }
}