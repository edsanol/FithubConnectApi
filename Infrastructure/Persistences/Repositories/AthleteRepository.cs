using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            var timeZoneBogota = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            var currentTimeInBogota = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneBogota);

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
                    FingerPrint = x.FingerPrint,
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
                        .Where(am => am.EndDate >= DateOnly.FromDateTime(currentTimeInBogota))
                        .ToList()
                }).AsNoTracking().SingleOrDefaultAsync();

            return athlete!;
        }

        public async Task<DashboardAthleteResponseDto> DashboardAthletes(int gymID)
        {
            var timeZoneBogota = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            var currentTimeInBogota = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneBogota);
            var currentMonth = currentTimeInBogota.Month;
            var currentYear = currentTimeInBogota.Year;

            // get total athletes
            var totalAthletes = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.Status == true)
                .CountAsync();

            // get active athletes
            var activeAthletes = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.Status == true && x.AthleteMemberships
                .Any(am => am.EndDate >= DateOnly.FromDateTime(currentTimeInBogota)))
                .CountAsync();

            // get inactive athletes
            var inactiveAthletes = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.Status == true && x.AthleteMemberships
                               .All(am => am.EndDate < DateOnly.FromDateTime(currentTimeInBogota)))
                .CountAsync();

            // get daily assistance
            var dailyAssistance = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.Status == true && x.AccessLogs.Any(al => al.AccessDateTime.Date == currentTimeInBogota.Date))
                .CountAsync();

            // get new athletes by month
            var newAthletesByMonth = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.AuditCreateDate.HasValue && x.AuditCreateDate.Value.Month == currentMonth && x.AuditCreateDate.Value.Year == currentYear)
                .CountAsync();

            // get income by month
            var incomeByMonth = await _context.Athlete
                .Where(x => x.IdGym.Equals(gymID) && x.Status == true && x.AthleteMemberships.Any(am => am.StartDate.Month == currentMonth && am.StartDate.Year == currentYear))
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
            if (athlete != null)
            {
                athlete.Status = false;
                athlete.AuditDeleteDate = Convert.ToDateTime(athlete.AuditDeleteDate);
                _context.Update(athlete);
                var recordsAffected = await _context.SaveChangesAsync();

                return recordsAffected > 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DestroyAthleteFromDB(string email)
        {
            var timeZoneBogota = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            var currentTimeInBogota = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneBogota);

            var athlete = await _context.Athlete.AsNoTracking().SingleOrDefaultAsync(x => x.Email.Equals(email));
            if (athlete != null)
            {
                athlete.AthleteName = "Anonymous";
                athlete.AthleteLastName = "Anonymous";
                athlete.BirthDate = DateTime.MinValue;
                athlete.Email = $"anonymous{athlete.AthleteId}@mail.com";
                athlete.PhoneNumber = "0000000000";
                athlete.Genre = "Anonymous";
                athlete.Password = "Anonymous";
                athlete.FingerPrint = null;
                athlete.Status = false;
                athlete.AuditDeleteDate = currentTimeInBogota;
                athlete.AuditDeleteUser = "System";
                _context.Update(athlete);
                var recordsAffected = await _context.SaveChangesAsync();

                return recordsAffected > 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> EditAthlete(Athlete athlete)
        {
            _context.Update(athlete);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<IEnumerable<DashboardGraphicsResponse>> GetDailyAssistance(int gymID, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            var dailyAssistance = await _context.AccessLog
                .Where(x => x.IdAthleteNavigation.IdGym == gymID
                    && x.AccessDateTime >= startDateTime
                    && x.AccessDateTime <= endDateTime)
                .GroupBy(x => x.AccessDateTime.Date)
                .Select(g => new DashboardGraphicsResponse
                {
                    Time = DateOnly.FromDateTime(g.Key),
                    Value = g.Select(x => x.IdAthlete).Distinct().Count()
                })
                .ToListAsync();

            return dailyAssistance;
        }

        public async Task<BaseEntityResponse<Athlete>> ListAthlete(BaseFiltersRequest filters, int gymID)
        {
            var response = new BaseEntityResponse<Athlete>();
            var timeZoneBogota = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            var currentTimeInBogota = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneBogota);

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
                    FingerPrint = x.FingerPrint,
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
                    case 5:
                        if (filters.TextFilter == "string")
                        {
                            athletes = athletes.Where(x => x.IdGym.Equals(gymID) &&
                                x.AthleteMemberships.Any(am => !string.IsNullOrEmpty(am.IdMembershipNavigation.MembershipName) &&
                                    am.EndDate >= DateOnly.FromDateTime(currentTimeInBogota))
                            );
                        }
                        else
                        {
                            DateTime endDate = string.IsNullOrEmpty(filters.EndDate) ? DateTime.MinValue : DateTime.Parse(filters.EndDate);
                            List<int> excludedAthleteIds = filters.TextFilter.Split(',').Select(int.Parse).ToList();

                            athletes = athletes.Where(x => x.IdGym.Equals(gymID) &&
                                x.AthleteMemberships.Any(am => !string.IsNullOrEmpty(am.IdMembershipNavigation.MembershipName) &&
                                    am.EndDate >= DateOnly.FromDateTime(currentTimeInBogota)) &&
                                !excludedAthleteIds.Contains(x.AthleteId) ||
                                (excludedAthleteIds.Contains(x.AthleteId) && x.AuditUpdateDate > endDate));
                        }

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
            var athlete = _context.Athlete
                .Where(x => x.Email.Equals(email))
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
                    FingerPrint = x.FingerPrint,
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
                }).AsNoTracking().SingleOrDefault();

            return athlete!;
        }

        public async Task<bool> RegisterAthlete(Athlete athlete)
        {
            await _context.AddAsync(athlete);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;

        }

        public async Task<bool> RegisterAthleteFingerPrint(int athleteID, string fingerPrint)
        {
            var timeZoneBogota = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            var currentTimeInBogota = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneBogota);
            var athlete = await _context.Athlete.AsNoTracking().SingleOrDefaultAsync(x => x.AthleteId.Equals(athleteID));
            if (athlete != null)
            {
                athlete.FingerPrint = fingerPrint;
                athlete.AuditUpdateDate = currentTimeInBogota;
                athlete.AuditUpdateUser = "System";
                _context.Update(athlete);
                var recordsAffected = await _context.SaveChangesAsync();

                return recordsAffected > 0;
            }
            return false;
        }

        public async Task<bool> RegisterPassword(int athleteID, string password)
        {
            var athlete = await _context.Athlete.AsNoTracking().SingleOrDefaultAsync(x => x.AthleteId.Equals(athleteID));
            if (athlete != null)
            {
                athlete.Password = password;
                _context.Update(athlete);
                var recordsAffected = await _context.SaveChangesAsync();

                return recordsAffected > 0;
            }
            return false;
        }
    }
}
