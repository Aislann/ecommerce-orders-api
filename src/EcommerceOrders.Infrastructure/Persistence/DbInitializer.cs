using EcommerceOrders.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrders.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            AppDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync())
            {
                context.Users.AddRange(
                    new User(
                        "João Silva",
                        "joao@email.com"),

                    new User(
                        "Maria Souza",
                        "maria@email.com"));
            }

            if (!await context.Products.AnyAsync())
            {
                context.Products.AddRange(
                    new Product(
                        "Notebook",
                        3500.00m),

                    new Product(
                        "Mouse",
                        120.00m),

                    new Product(
                        "Teclado Mecânico",
                        250.00m),

                    new Product(
                        "Monitor",
                        900.00m));
            }

            await context.SaveChangesAsync();
        }
    }
}
