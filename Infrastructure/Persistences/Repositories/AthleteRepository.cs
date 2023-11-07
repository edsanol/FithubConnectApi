﻿using Domain.Entities;
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
            var athlete = await _context.Athlete.AsNoTracking().FirstOrDefaultAsync(x => x.AthleteId.Equals(athleteID));

            return athlete!;
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

        public async Task<BaseEntityResponse<Athlete>> ListAthlete(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Athlete>();

            var athletes = (from c in _context.Athlete
                            select c).AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumFilter)
                {
                    case 1:
                        athletes = athletes.Where(x => x.AthleteName.Contains(filters.TextFilter));
                        break;
                    case 2:
                        athletes = athletes.Where(x => x.AthleteLastName.Contains(filters.TextFilter));
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
    }
}
