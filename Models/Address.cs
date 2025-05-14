namespace E_commerceData.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Commune { get; set; } = null!;
        public string Detail { get; set; } = null!;
        public string HousingType { get; set; } = null!;

        public User User { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
