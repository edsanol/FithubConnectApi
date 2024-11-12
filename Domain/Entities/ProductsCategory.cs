namespace Domain.Entities
{
    public partial class ProductsCategory
    {
        public long CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public int? IdGym { get; set; }

        public virtual Gym IdGymNavigation { get; set; } = null!;

        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
