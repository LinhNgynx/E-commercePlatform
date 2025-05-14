namespace E_commerceData.DTO
{
    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public List<int>? VoucherIds { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
    public class OrderItemDto
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
