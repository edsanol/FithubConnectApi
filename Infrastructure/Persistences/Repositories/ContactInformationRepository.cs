using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ContactInformationRepository : GenericRepository<ContactInformation>, IContactInformationRepository
    {
        private readonly DbFithubContext _context;
        public ContactInformationRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ContactInformation> GetContactInformation()
        {
            var contact = await _context.ContactInformation.FirstOrDefaultAsync();

            if (contact == null)
            {
                return new ContactInformation();
            }
            else
            {
                return contact;
            }
        }
    }
}
