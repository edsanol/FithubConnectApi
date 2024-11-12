using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IStockMovementsRepository
    {
        Task<bool> RegisterEntryAndExitProduct(StockMovements request);
    }
}
