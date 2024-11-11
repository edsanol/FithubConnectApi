namespace Domain.Entities
{
    public class ProductsVariant
    {
        public long VariantId { get; set; }

        public long IdProduct { get; set; }

        public string SKU { get; set; } = null!;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; }

        public virtual Products Product { get; set; } = null!;

        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();
    }
}
