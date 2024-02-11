using Application.Commons.Bases;
using Application.Dtos.Response;
using Application.Interfaces;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class DashboardApplication : IDashboardApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtHandler _jwtHandler;

        public DashboardApplication(IUnitOfWork unitOfWork, IJwtHandler jwtHandler)
        {
            _unitOfWork = unitOfWork;
            _jwtHandler = jwtHandler;
        }

        public async Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetDailyAssistanceGraphic(DateOnly startDate, DateOnly endDate)
        {
            var gymID = _jwtHandler.ExtractGymIdFromToken();
            var response = new BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>();
            var graphic = await _unitOfWork.AthleteRepository.GetDailyAssistance(gymID, startDate, endDate);

            var graphicResponse = graphic.Select(item => new DashboardGraphicsResponseDto
            {
                Time = item.Time,
                Value = item.Value
            }).ToList();

            return new BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>
            {
                IsSuccess = graphicResponse.Any(),
                Message = graphicResponse.Any() ? ReplyMessage.MESSAGE_QUERY : ReplyMessage.MESSAGE_QUERY_EMPTY,
                Data = graphicResponse
            };
        }

        public async Task<BaseResponse<DashboardAthleteResponseDto>> GetDashboard()
        {
            var gymID = _jwtHandler.ExtractGymIdFromToken();
            var response = new BaseResponse<DashboardAthleteResponseDto>();
            var dashboard = await _unitOfWork.AthleteRepository.DashboardAthletes(gymID);

            if (dashboard == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            response.Data = new DashboardAthleteResponseDto
            {
                TotalAthletes = dashboard.TotalAthletes,
                ActiveAthletes = dashboard.ActiveAthletes,
                ActiveAthletesPercentage = dashboard.ActiveAthletesPercentage,
                InactiveAthletes = dashboard.InactiveAthletes,
                InactiveAthletesPercentage = dashboard.InactiveAthletesPercentage,
                DailyAssistance = dashboard.DailyAssistance,
                NewAthletesByMonth = dashboard.NewAthletesByMonth,
                IncomeByMonth = dashboard.IncomeByMonth
            };

            response.IsSuccess = true;
            response.Message = ReplyMessage.MESSAGE_QUERY;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetIncomeGraphic(DateOnly startDate, DateOnly endDate)
        {
            var gymID = _jwtHandler.ExtractGymIdFromToken();
            var response = new BaseResponse<DashboardAthleteResponseDto>();
            var graphic = await _unitOfWork.AthleteMembershipRepository.GetIncome(gymID, startDate, endDate);

            var graphicResponse = graphic.Select(item => new DashboardGraphicsResponseDto
            {
                Time = item.Time,
                Value = item.Value
            }).ToList();

            return new BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>
            {
                IsSuccess = graphicResponse.Any(),
                Message = graphicResponse.Any() ? ReplyMessage.MESSAGE_QUERY : ReplyMessage.MESSAGE_QUERY_EMPTY,
                Data = graphicResponse
            };
        }

        public async Task<BaseResponse<IEnumerable<DashboardPieResponseDto>>> GetMembershipPercentage()
        {
            var gymID = _jwtHandler.ExtractGymIdFromToken();
            var response = new BaseResponse<IEnumerable<DashboardPieResponseDto>>();
            var pie = await _unitOfWork.MembershipRepository.MembershipPercentage(gymID);

            var pieResponse = pie.Select(item => new DashboardPieResponseDto
            {
                Label = item.Label,
                Value = item.Value
            }).ToList();

            return new BaseResponse<IEnumerable<DashboardPieResponseDto>>
            {
                IsSuccess = pieResponse.Any(),
                Message = pieResponse.Any() ? ReplyMessage.MESSAGE_QUERY : ReplyMessage.MESSAGE_QUERY_EMPTY,
                Data = pieResponse
            };
        }
    }
}
