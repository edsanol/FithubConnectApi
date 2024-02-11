using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IAthleteRepository
    {
        Task<BaseEntityResponse<Athlete>> ListAthlete(BaseFiltersRequest filters, int gymID);
        Task<Athlete> AthleteById(int athleteID);
        Task<bool> RegisterAthlete(Athlete athlete);
        Task<bool> EditAthlete(Athlete athlete);
        Task<bool> DeleteAthlete(int athleteID);
        Task<Athlete> LoginAthlete(string email);
        Task<DashboardAthleteResponseDto> DashboardAthletes(int gymID);
        Task<IEnumerable<DashboardGraphicsResponseDto>> GetDailyAssistance(int gymID, DateOnly startDate, DateOnly endDate);
    }
}
