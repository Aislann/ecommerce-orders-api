using EcommerceOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrders.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(
            EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(product => product.Price)
                .IsRequired()
                .HasPrecision(18, 2);
        }
    }
}
