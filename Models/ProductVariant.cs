namespace E_commerceData.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public int StockQuantity { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
