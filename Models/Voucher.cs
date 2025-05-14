namespace E_commerceData.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal DiscountAmount { get; set; }
        public bool IsPercentage { get; set; }

        public ICollection<OrderVoucher> OrderVouchers { get; set; } = new List<OrderVoucher>();
    }

}
