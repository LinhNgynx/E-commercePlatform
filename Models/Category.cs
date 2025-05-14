namespace E_commerceData.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal DiscountPercentage { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
