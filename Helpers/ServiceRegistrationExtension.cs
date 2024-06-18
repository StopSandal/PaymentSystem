using PaymentSystem.Services.Helpers;
using PaymentSystem.Services.Interfaces;
using PaymentSystem.Services.Services;

namespace PaymentSystem.Helpers
{
    /// <summary>
    /// Provides extension methods for registering services with the dependency injection container.
    /// </summary>
    public static class ServiceRegistrationExtension
    {
        /// <summary>
        /// Registers the application's services with the dependency injection container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
