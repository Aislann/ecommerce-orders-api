using EcommerceOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrders.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(
            EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(order => order.Id);

            builder.Property(order => order.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(order => order.CreatedAt)
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(order => order.Items)
                .WithOne()
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(order => order.Items)
                .HasField("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
