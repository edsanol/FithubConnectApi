using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ProductsCategoryRepository : GenericRepository<ProductsCategory>, IProductsCategoryRepository
    {
        private readonly DbFithubContext _context;

        public ProductsCategoryRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ProductsCategory>> GetAllCategoriesProducts(int gymID)
        {
            return await _context.ProductsCategory.Where(x => x.IdGym == gymID || x.IdGym == null).ToListAsync();
        }

        public async Task<bool> RegisterCategoryProduct(ProductsCategory request)
        {
            await _context.AddAsync(request);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
