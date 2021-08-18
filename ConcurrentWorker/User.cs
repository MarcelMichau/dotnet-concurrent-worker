namespace ConcurrentWorker
{
    public class User
    {
        public User()
        {
            Orders = new List<Order>();
        }

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? SomethingUnique { get; set; }
        public Guid SomeGuid { get; set; }

        public string? Avatar { get; set; }
        public Guid CartId { get; set; }
        public string? SSN { get; set; }
        public Gender Gender { get; set; }

        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public string? Item { get; set; }
        public int Quantity { get; set; }
        public int? LotNumber { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
