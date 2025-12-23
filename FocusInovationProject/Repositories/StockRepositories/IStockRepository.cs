using FocusInovationProject.DTOs.StockDtos;

namespace FocusInovationProject.Repositories.StockRepositories
{
    public interface IStockRepository
    {
        Task<IEnumerable<ResultStockDto>> GetAllAsync();
        Task<IEnumerable<ResultStockDto>> GetStocksWithProduct();
        Task<UpdateStockDto> GetByIdAsync(int id);
        Task CreateAsync(CreateStockDto stockDto);
        Task UpdateAsync(UpdateStockDto stockDto);
        Task<UpdateStockDto> GetByProductIdAsync(int id);
        Task UpdateQuantityAsync(int productId, double quantityChange);
        Task UpdateQuantityIncreasingAsync(int productId, double quantityChange);
        Task DeleteAsync(int id);
    }
}
