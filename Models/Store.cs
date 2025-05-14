namespace E_commerceData.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;

        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }

}
