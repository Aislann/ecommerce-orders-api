using EcommerceOrders.Application.Services.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceOrders.Application.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<CreateOrderService>();
            services.AddScoped<GetOrderService>();
            services.AddScoped<ListOrdersService>();
            services.AddScoped<UpdateOrderService>();
            services.AddScoped<CancelOrderService>();
            services.AddScoped<DeleteOrderService>();
            services.AddScoped<ProcessOrderService>();
            services.AddScoped<ShipOrderService>();

            return services;
        }
    }
}
