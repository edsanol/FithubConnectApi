using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class OrdersPaymentsRepository : GenericRepository<Orders>, IOrdersPaymentsRepository
    {
        private readonly DbFithubContext _context;

        public OrdersPaymentsRepository(DbFithubContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterOrder(Orders order)
        {
            _context.Orders.Add(order);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
