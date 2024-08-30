using static DocPort.Services.ServiceInterfaces;

namespace DocPort.Services
{
    public static class ServicesExtensions
    { 
        /// <summary>
        /// Add DocPort Services as scoped services to the dependency injection container.
        /// </summary>
        /// <param name="services">Service container</param>
        /// <returns>The service container that has our services added.</returns>
        public static IServiceCollection AddDocPortServices(this IServiceCollection services)
        {
            services.AddScoped<IDocsService, DocsService>();
            services.AddScoped<IContentHoldersService, ContentHoldersService>();

            return services;
        }
    }
}
