namespace E_commerceData.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }

        public Order Order { get; set; } = default!;
        public ProductVariant ProductVariant { get; set; } = default!;
    }


}
