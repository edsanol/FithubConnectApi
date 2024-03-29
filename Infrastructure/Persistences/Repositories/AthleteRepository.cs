using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class AthleteRepository : GenericRepository<Athlete>, IAthleteRepository
    {
        private readonly DbFithubContext _context;

        public AthleteRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Athlete> AthleteById(int athleteID)
        {
            var athlete = await _context.Athlete
                .Where(x => x.AthleteId.Equals(athleteID))
                .Select(x => new Athlete
                {
                    AthleteId = x.AthleteId,
                    AthleteName = x.AthleteName,
                    AthleteLastName = x.AthleteLastName,
                    BirthDate = x.BirthDate,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Genre = x.Genre,
                    Password = x.Password,
                    IdGym = x.IdGym,
                    AuditCreateUser = x.AuditCreateUser,
                    AuditCreateDate = x.AuditCreateDate,
                    AuditUpdateUser = x.AuditUpdateUser,
                    AuditUpdateDate = x.AuditUpdateDate,
                    AuditDeleteUser = x.AuditDeleteUser,
                    AuditDeleteDate = x.AuditDeleteDate,
                    CardAccesses = x.CardAccesses
                        .Select(ca => new CardAccess
                        {
                            CardId = ca.CardId,
                            CardNumber = ca.CardNumber,
                            IdAthlete = ca.IdAthlete,
                            Status = ca.Status
                        }).ToList(),
                    Status = x.Status,
                    AthleteMemberships = x.AthleteMemberships
                        .Select(am => new AthleteMembership
                        {
                            StartDate = am.StartDate,
                            EndDate = am.EndDate,
                            IdMembershipNavigation = new Membership
                            {
                                MembershipName = am.IdMembershipNavigation.MembershipName,
                                Cost = am.IdMembershipNavigation.Cost,
                                MembershipId = am.IdMembershipNavigation.MembershipId,
                                Discounts = am.IdMembershipNavigation.Discounts
                                    .Select(d => new Discount
                                    {
                                        DiscountId = d.DiscountId,
                                        DiscountPercentage = d.DiscountPercentage,
                                        IdMembership = d.IdMembership,
                                        StartDate = d.StartDate,
                                        EndDate = d.EndDate,
                                        Status = d.Status
                                    }).ToList()
                            }
                        })
                        .Where(am => am.EndDate >= DateOnly.FromDateTime(DateTime.Now))
                        .ToList()
                }).AsNoTracking().SingleOrDefaultAsync();

            return athlete!;
        }

        public async Task<DashboardAthleteResponseDto> DashboardAthletes(int gymID)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // get total athletes
            var totalAthletes = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID))
                .CountAsync();

            // get active athletes
            var activeAthletes = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.AthleteMemberships
                .Any(am => am.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
                .CountAsync();

            // get inactive athletes
            var inactiveAthletes = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.AthleteMemberships
                               .All(am => am.EndDate < DateOnly.FromDateTime(DateTime.Now)))
                .CountAsync();

            // get daily assistance
            var dailyAssistance = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.AccessLogs.Any(al => al.AccessDateTime.Date == DateTime.Now.Date))
                .CountAsync();

            // get new athletes by month
            var newAthletesByMonth = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.AuditCreateDate.HasValue && x.AuditCreateDate.Value.Month == currentMonth && x.AuditCreateDate.Value.Year == currentYear)
                .CountAsync();

            // get income by month
            var incomeByMonth = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.AthleteMemberships.Any(am => am.StartDate.Month == currentMonth && am.StartDate.Year == currentYear))
                .SumAsync(x => x.AthleteMemberships
                .Where(am => am.StartDate.Month == currentMonth && am.StartDate.Year == currentYear)
                .Sum(am => am.IdMembershipNavigation.Cost));

            // get active athletes percentage
            float activeAthletesPercentage = (float)activeAthletes / totalAthletes * 100;
            if (float.IsNaN(activeAthletesPercentage))
            {
                activeAthletesPercentage = 0.0f;
            }

            // get inactive athletes percentage
            float inactiveAthletesPercentage = (float)inactiveAthletes / totalAthletes * 100;
            if (float.IsNaN(inactiveAthletesPercentage))
            {
                inactiveAthletesPercentage = 0.0f;
            }

            return new DashboardAthleteResponseDto
            {
                TotalAthletes = totalAthletes,
                ActiveAthletes = activeAthletes,
                ActiveAthletesPercentage = activeAthletesPercentage,
                InactiveAthletes = inactiveAthletes,
                InactiveAthletesPercentage = inactiveAthletesPercentage,
                DailyAssistance = dailyAssistance,
                NewAthletesByMonth = newAthletesByMonth,
                IncomeByMonth = incomeByMonth
            };
        }

        public async Task<bool> DeleteAthlete(int athleteID)
        {
            var athlete = await _context.Athlete.AsNoTracking().SingleOrDefaultAsync(x => x.AthleteId.Equals(athleteID));
            athlete.Status = false;
            athlete.AuditDeleteDate = Convert.ToDateTime(athlete.AuditDeleteDate);
            _context.Update(athlete);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<bool> EditAthlete(Athlete athlete)
        {
            _context.Update(athlete);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<IEnumerable<DashboardGraphicsResponse>> GetDailyAssistance(int gymID, DateOnly startDate, DateOnly endDate)
        {
            var dailyAssistance = await _context.AccessLog
                .Where(x => x.IdAthleteNavigation.IdGym.Equals(gymID) && DateOnly.FromDateTime(x.AccessDateTime) >= startDate && DateOnly.FromDateTime(x.AccessDateTime) <= endDate)
                .GroupBy(x => x.AccessDateTime.Date)
                .Select(x => new DashboardGraphicsResponse
                {
                    Time = DateOnly.FromDateTime(x.Key),
                    Value = x.Count()
                })
                .ToListAsync();

            return dailyAssistance;
        }

        public async Task<BaseEntityResponse<Athlete>> ListAthlete(BaseFiltersRequest filters, int gymID)
        {
            var response = new BaseEntityResponse<Athlete>();

            var athletes = _context.Athlete
                .Include(x => x.IdGymNavigation)
                .Where(x => x.IdGym.Equals(gymID) && x.Status.Equals(true))
                .Select(x => new Athlete
                {
                    AthleteId = x.AthleteId,
                    AthleteName = x.AthleteName,
                    AthleteLastName = x.AthleteLastName,
                    BirthDate = x.BirthDate,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Genre = x.Genre,
                    Password = x.Password,
                    IdGym = x.IdGym,
                    AuditCreateUser = x.AuditCreateUser,
                    AuditCreateDate = x.AuditCreateDate,
                    AuditUpdateUser = x.AuditUpdateUser,
                    AuditUpdateDate = x.AuditUpdateDate,
                    AuditDeleteUser = x.AuditDeleteUser,
                    AuditDeleteDate = x.AuditDeleteDate,
                    CardAccesses = x.CardAccesses
                        .Select(ca => new CardAccess
                        {
                            CardId = ca.CardId,
                            CardNumber = ca.CardNumber,
                            IdAthlete = ca.IdAthlete,
                            Status = ca.Status
                        }).ToList(),
                    Status = x.Status,
                    AthleteMemberships = x.AthleteMemberships
                        .Select(am => new AthleteMembership
                        {
                            StartDate = am.StartDate,
                            EndDate = am.EndDate,
                            IdMembershipNavigation = new Membership
                            {
                                MembershipName = am.IdMembershipNavigation.MembershipName,
                                Cost = am.IdMembershipNavigation.Cost,
                                MembershipId = am.IdMembershipNavigation.MembershipId,
                            }
                        }).ToList()
                }).AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                var filterTextLower = filters.TextFilter.ToLower();

                switch (filters.NumFilter)
                {
                    case 1:
                        athletes = athletes.Where(x => x.IdGym.Equals(gymID) && x.AthleteName.ToLower().Contains(filterTextLower));
                        break;
                    case 2:
                        athletes = athletes.Where(x => x.IdGym.Equals(gymID) && x.AthleteLastName.ToLower().Contains(filterTextLower));
                        break;
                    case 3:
                        athletes = athletes.Where(x => x.IdGym.Equals(Int32.Parse(filters.TextFilter)));
                        break;
                    case 4:
                        athletes = athletes.Where(x => x.IdGym.Equals(Int32.Parse(filters.TextFilter)) && x.Status.Equals(true));
                        break;
                }
            }

            if (filters.StateFilter is not null)
            {
                athletes = athletes.Where(x => x.Status.Equals(filters.StateFilter));
            }

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                athletes = athletes.Where(x => Convert.ToDateTime(x.AuditCreateUser) >= Convert.ToDateTime(filters.StartDate));
            }

            filters.Sort ??= "AthleteId";

            response.TotalRecords = await athletes.CountAsync();
            response.Items = await Ordering(filters, athletes, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<Athlete> LoginAthlete(string email)
        {
            var athlete = await _context.Athlete.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Equals(email));

            return athlete!;
        }

        public async Task<bool> RegisterAthlete(Athlete athlete)
        {
            await _context.AddAsync(athlete);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;

        }

        public async Task<bool> RegisterPassword(int athleteID, string password)
        {
            var athlete = await _context.Athlete.AsNoTracking().SingleOrDefaultAsync(x => x.AthleteId.Equals(athleteID));
            athlete.Password = password;
            _context.Update(athlete);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
