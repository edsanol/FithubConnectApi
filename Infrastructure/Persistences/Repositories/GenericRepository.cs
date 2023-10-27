using Infrastructure.Commons.Bases.Request;
using Infrastructure.Helpers;
using Infrastructure.Persistences.Interfaces;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Persistences.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected IQueryable<TDto> Ordering<TDto>(BasePaginationRequest request, IQueryable<TDto> queryable, bool pagination = false) where TDto : class
        {
            IQueryable<TDto> queryDto = request.Order == "desc" ? queryable.OrderBy($"{request.Sort} descending") : queryable.OrderBy($"{request.Sort} ascending");

            if (pagination) queryDto = queryDto.Paginate(request);

            return queryDto;
        }
    }
}
