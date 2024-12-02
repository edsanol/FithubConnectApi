using Application.Commons.Bases;
using Application.Dtos.Request;

namespace Application.Interfaces
{
    public interface IOrdersPaymentsApplication
    {
        Task<BaseResponse<bool>> RegisterOrder(OrderRequestDto request);
    }
}
