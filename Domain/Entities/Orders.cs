namespace Domain.Entities
{
    public class Orders
    {
        public long OrderId { get; set; }

        public int IdAthlete { get; set; }

        public int IdGym { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalPaid { get; set; }

        public string Status { get; set; } = null!;

        public string? ShippingAddress { get; set; }

        public string? Notes { get; set; }

        public virtual Athlete IdAthleteNavigation { get; set; } = null!;

        public virtual Gym IdGymNavigation { get; set; } = null!;

        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();

        public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();
    }
}
