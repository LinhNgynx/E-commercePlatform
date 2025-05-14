namespace E_commerceData.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; } = default!;

        public User User { get; set; } = default!;
        public Address Address { get; set; } = default!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<OrderVoucher> OrderVouchers { get; set; } = new List<OrderVoucher>();
    }


}
