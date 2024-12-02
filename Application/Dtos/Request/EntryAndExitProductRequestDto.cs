using Application.Dtos.Enum;

namespace Application.Dtos.Request
{
    public class EntryAndExitProductRequestDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
        public MovementTypeEnum Type { get; set; }
    }
}
