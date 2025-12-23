using FocusInovationProject.DTOs.SaleDtos;

namespace FocusInovationProject.Repositories.SaleRepositories
{
    public interface ISaleRepository
    {
        Task<IEnumerable<ResultSaleDto>> GetAllAsync();
        Task<IEnumerable<ResultSaleDto>> GetSalesWithCustomerAndProduct();
        Task<UpdateSaleDto> GetByIdAsync(int id);
        Task CreateAsync(CreateSaleDto saleDto);
        Task UpdateAsync(UpdateSaleDto saleDto);
        Task DeleteAsync(int id);
        Task UpdateListPriceAsync(int id, double calculatedPrice);
        Task UpdateSalesPriceAsync(int id, double salesPrice);
        Task UpdateQuantityAsync(int productId, double quantityChange);
    }
}
