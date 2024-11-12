namespace Domain.Entities
{
    public class Products
    {
        public long ProductId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public long IdCategory { get; set; }

        public decimal BasePrice { get; set; }

        public bool IsActive { get; set; }

        public int IdGym { get; set; }

        public virtual Gym IdGymNavigation { get; set; } = null!;

        public virtual ProductsCategory IdProductsCategoryNavigation { get; set; } = null!;

        public virtual ICollection<ProductsVariant> ProductsVariants { get; set; } = new List<ProductsVariant>();
    }
}
