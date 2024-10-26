using Application.Commons.Bases;
using Application.Dtos.Response;
using Application.Interfaces;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
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

        private Dtos.Response.AthleteAssistenceDto MapToApplicationDto(Infrastructure.Commons.Bases.Response.AthleteAssistenceDto dto)
        {
            return new Dtos.Response.AthleteAssistenceDto
            {
                AthleteId = dto.AthleteId,
                AthleteName = dto.AthleteName,
                AthleteLastName = dto.AthleteLastName,
                Email = dto.Email,
                DateAssistence = dto.DateAssistence,
                TimeAssistence = dto.TimeAssistence.ToString("HH:mm:ss")
            };
        }

        public async Task<BaseResponse<BaseEntityResponse<Dtos.Response.AthleteAssistenceDto>>> GetAthleteAssitance(BaseFiltersRequest filters)
        {
            var gymID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<BaseEntityResponse<Dtos.Response.AthleteAssistenceDto>>();

            var assistance = await _unitOfWork.AccessLogRepository.GetAthleteAssitance(filters, gymID);

            if (assistance.Items is not null)
            {
                var mappedItems = assistance.Items.Select(MapToApplicationDto).ToList();
                var mappedResponse = new BaseEntityResponse<Dtos.Response.AthleteAssistenceDto>
                {
                    TotalRecords = assistance.TotalRecords,
                    Items = mappedItems
                };

                response.IsSuccess = true;
                response.Data = mappedResponse;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<DashboardGraphicsResponseDto>>> GetDailyAssistanceGraphic(DateOnly startDate, DateOnly endDate)
        {
            var gymID = _jwtHandler.ExtractIdFromToken();
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

        public async Task<BaseResponse<Dtos.Response.DashboardAthleteResponseDto>> GetDashboard()
        {
            var gymID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<Dtos.Response.DashboardAthleteResponseDto>();
            var dashboard = await _unitOfWork.AthleteRepository.DashboardAthletes(gymID);

            if (dashboard == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            response.Data = new Dtos.Response.DashboardAthleteResponseDto
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
            var gymID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<Dtos.Response.DashboardAthleteResponseDto>();
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

        public async Task<BaseResponse<IEnumerable<Dtos.Response.DashboardPieResponseDto>>> GetMembershipPercentage()
        {
            var gymID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<IEnumerable<Dtos.Response.DashboardPieResponseDto>>();
            var pie = await _unitOfWork.MembershipRepository.MembershipPercentage(gymID);

            var pieResponse = pie.Select(item => new Dtos.Response.DashboardPieResponseDto
            {
                Label = item.Label,
                Value = item.Value
            }).ToList();

            return new BaseResponse<IEnumerable<Dtos.Response.DashboardPieResponseDto>>
            {
                IsSuccess = pieResponse.Any(),
                Message = pieResponse.Any() ? ReplyMessage.MESSAGE_QUERY : ReplyMessage.MESSAGE_QUERY_EMPTY,
                Data = pieResponse
            };
        }

        public async Task<BaseResponse<IEnumerable<Dtos.Response.AthleteBirthDateDto>>> GetAthleteBirthDate()
        {
            var gymID = _jwtHandler.ExtractIdFromToken();
            var response = new BaseResponse<IEnumerable<Dtos.Response.AthleteBirthDateDto>>();
            var birthDate = await _unitOfWork.AthleteRepository.GetAthleteBirthDate(gymID);

            var birthDateResponse = birthDate.Select(item => new Dtos.Response.AthleteBirthDateDto
            {
                AthleteId = item.AthleteId,
                AthleteName = item.AthleteName,
                AthleteLastName = item.AthleteLastName,
                BirthDate = item.BirthDate,
                Age = item.Age
            }).ToList();

            return new BaseResponse<IEnumerable<Dtos.Response.AthleteBirthDateDto>>
            {
                IsSuccess = birthDateResponse.Any(),
                Message = birthDateResponse.Any() ? ReplyMessage.MESSAGE_QUERY : ReplyMessage.MESSAGE_QUERY_EMPTY,
                Data = birthDateResponse
            };
        }
    }
}
