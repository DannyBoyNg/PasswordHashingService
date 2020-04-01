using Microsoft.Extensions.DependencyInjection;
using System;

namespace DannyBoyNg.Services
{
    /// <summary>
    /// Contains static methods to help with Dependancy Injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the password hashing service.
        /// </summary>
        /// <param name="serviceCollection">DI container.</param>
        /// <returns>DI container.</returns>
        public static IServiceCollection AddPasswordHashingService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IPasswordHashingService, PasswordHashingService>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the password hashing service.
        /// </summary>
        /// <param name="serviceCollection">DI container.</param>
        /// <param name="options">The options.</param>
        /// <returns>DI container.</returns>
        public static IServiceCollection AddPasswordHashingService(this IServiceCollection serviceCollection, Action<PasswordHashingSettings> options)
        {
            serviceCollection.AddScoped<IPasswordHashingService, PasswordHashingService>();
            serviceCollection.Configure(options);
            return serviceCollection;
        }
    }
}
