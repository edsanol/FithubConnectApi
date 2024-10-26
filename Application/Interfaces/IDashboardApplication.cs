using Application.Commons.Bases;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IDashboardApplication
    {
        Task<BaseResponse<Dtos.Response.DashboardAthleteResponseDto>> GetDashboard();
        Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetDailyAssistanceGraphic(DateOnly startDate, DateOnly endDate);
        Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetIncomeGraphic(DateOnly startDate, DateOnly endDate);
        Task<BaseResponse<IEnumerable<Dtos.Response.DashboardPieResponseDto>>> GetMembershipPercentage();
        Task<BaseResponse<BaseEntityResponse<Dtos.Response.AthleteAssistenceDto>>> GetAthleteAssitance(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<Dtos.Response.AthleteBirthDateDto>>> GetAthleteBirthDate();
    }
}
