using EcommerceOrders.Domain.Exceptions;

namespace EcommerceOrders.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        private User() { }

        public User(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required.");

            Name = name;
            Email = email;
        }

        internal User(int id, string name, string email) : this(name, email)
        {
            if (id <= 0)
                throw new DomainException("UserId must be greater than zero.");

            Id = id;
        }
    }
}
