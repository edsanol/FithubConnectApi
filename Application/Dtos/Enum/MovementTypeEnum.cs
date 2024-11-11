using System.Text.Json.Serialization;

namespace Application.Dtos.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MovementTypeEnum
    {
        Entry,
        Exit,
        Sale,
        Return
    }
}
