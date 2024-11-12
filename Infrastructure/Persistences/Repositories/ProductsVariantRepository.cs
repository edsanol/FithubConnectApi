using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ProductsVariantRepository : GenericRepository<ProductsVariant>, IProductsVariantRepository
    {
        private readonly DbFithubContext _context;

        public ProductsVariantRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> EditProductVariant(ProductsVariant request)
        {
            _context.Update(request);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<ProductsVariant> GetProductVariantByProductId(int productId)
        {
            return await _context.ProductsVariant.FirstOrDefaultAsync(x => x.IdProduct == productId) ?? throw new Exception("Producto no encontrado");
        }

        public async Task<bool> RegisterProductVariant(ProductsVariant request)
        {
            await _context.AddAsync(request);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
