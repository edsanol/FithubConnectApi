using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ProductsRepository : GenericRepository<Products>, IProductsRepository
    {
        private readonly DbFithubContext _context;

        public ProductsRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> EditProduct(Products request)
        {
            _context.Update(request);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<BaseEntityResponse<Products>> GetAllProducts(BaseFiltersRequest filters, int gymID)
        {
            var response = new BaseEntityResponse<Products>();

            var products = _context.Products
                .Include(x => x.IdGymNavigation)
                .Include(x => x.IdProductsCategoryNavigation)
                .Include(x => x.ProductsVariants)
                .Where(x => x.IdGym == gymID && x.IsActive == true)
                .AsNoTracking()
                .AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                var filterTextLower = filters.TextFilter.ToLower();

                switch (filters.NumFilter)
                {
                    case 1:
                        products = products.Where(x => x.IdGym.Equals(gymID) && x.Name.ToLower().Contains(filterTextLower));
                        break;
                    case 2:
                        products = products.Where(x => x.IdGym.Equals(gymID) && x.Description.ToLower().Contains(filterTextLower));
                        break;

                }
            }

            if (filters.StateFilter is not null)
            {
                products = products.Where(x => x.IsActive.Equals(filters.StateFilter));
            }

            filters.Sort ??= "ProductId";
            response.TotalRecords = await products.CountAsync();
            response.Items = await Ordering(filters, products, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<Products> GetProductById(int productId, int gymID)
        {
            return await _context.Products
                .Include(x => x.IdGymNavigation)
                .Include(x => x.IdProductsCategoryNavigation)
                .Include(x => x.ProductsVariants)
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.IdGym == gymID) ?? throw new Exception("Producto no encontrado");
        }

        public async Task<bool> RegisterProduct(Products request)
        {
            await _context.AddAsync(request);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
