using Application.Commons.Bases;
using Application.Dtos.Response;

namespace Application.Interfaces
{
    public interface IDashboardApplication
    {
        Task<BaseResponse<DashboardAthleteResponseDto>> GetDashboard();
        Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetDailyAssistanceGraphic(DateOnly startDate, DateOnly endDate);
        Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetIncomeGraphic(DateOnly startDate, DateOnly endDate);
        Task<BaseResponse<IEnumerable<DashboardPieResponseDto>>> GetMembershipPercentage();
    }
}
