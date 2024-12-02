namespace Domain.Entities
{
    public class OrderDetails
    {
        public long OrderDetailId { get; set; }

        public long IdOrder { get; set; }

        public long IdVariant { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public int ReturnQuantity { get; set; }

        public virtual Orders Order { get; set; } = null!;

        public virtual ProductsVariant Variant { get; set; } = null!;
    }
}
