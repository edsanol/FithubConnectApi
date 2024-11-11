namespace Domain.Entities
{
    public class StockMovements
    {
        public long MovementId { get; set; }

        public long IdVariant { get; set; }

        public string MovementType { get; set; } = null!;

        public int Quantity { get; set; }

        public DateTime MovementDate { get; set; }

        public long? IdRelatedOrder { get; set; }

        public string Notes { get; set; } = string.Empty;

        public virtual ProductsVariant IdVariantNavigation { get; set; } = null!;

        public virtual Orders? IdRelatedOrderNavigation { get; set; }
    }
}
