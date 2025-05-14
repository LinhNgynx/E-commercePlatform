namespace E_commerceData.Models
{
    public class OrderVoucher
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; } = null!;
    }

}
