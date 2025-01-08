namespace Domain.Entities
{
    public class MuscleGroups
    {
        public long MuscleGroupId { get; set; }

        public string MuscleGroupName { get; set; } = null!;

        public string MuscleGroupDescription { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Routines> Routines { get; set; } = new List<Routines>();
    }
}
