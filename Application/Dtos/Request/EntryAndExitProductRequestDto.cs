using Application.Dtos.Enum;

namespace Application.Dtos.Request
{
    public class EntryAndExitProductRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public MovementTypeEnum Type { get; set; }
    }
}
