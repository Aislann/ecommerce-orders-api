using EcommerceOrders.Domain.Enums;
using EcommerceOrders.Domain.Exceptions;
using System.Net.NetworkInformation;

namespace EcommerceOrders.Domain.Entites
{
    public class Order
    {
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public Order(int userId, IEnumerable<OrderItem> orderItems)
        {
            if (userId <= 0)
                throw new DomainException("Buyer is required.");

            if (orderItems is null || !orderItems.Any())
                throw new DomainException("Order must contain at least one product.");

            UserId = userId;
            Status = OrderStatus.Started;
            CreatedAt = DateTime.UtcNow;

            _orderItems.AddRange(orderItems);
        }

        public void Update(int  userId, IEnumerable<OrderItem> orderItems)
        {
            if (Status != OrderStatus.Started)
                throw new DomainException("Only initiated orders can be changed.");

            if (userId <= 0)
                throw new DomainException("Buyer is required.");

            if (orderItems is null || !orderItems.Any())
                throw new DomainException("Order must contain at least one product.");

            UserId = userId;

            _orderItems.Clear();
            _orderItems.AddRange(orderItems);

            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Started &&
                Status != OrderStatus.Processed)
            {
                throw new DomainException("Only initiated or processed orders can be canceled.");
            }

            Status = OrderStatus.Canceled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Process()
        {
            if (Status != OrderStatus.Started)
                throw new DomainException("Only initiated orders can be processed.");

            Status = OrderStatus.Processed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Ship()
        {
            if (Status != OrderStatus.Processed)
                throw new DomainException("Only processed orders can be sent.");

            Status = OrderStatus.Shipped;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
