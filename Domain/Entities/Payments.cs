namespace Domain.Entities
{
    public class Payments
    {
        public long PaymentId { get; set; }

        public long IdOrder { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal AmountPaid { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string? Notes { get; set; }

        public virtual Orders Order { get; set; } = null!;
    }
}
