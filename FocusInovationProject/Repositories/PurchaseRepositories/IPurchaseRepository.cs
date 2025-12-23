using FocusInovationProject.DTOs.PurchaseDtos;

namespace FocusInovationProject.Repositories.PurchaseRepositories
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<ResultPurchaseDto>> GetAllAsync();
        Task<IEnumerable<ResultPurchaseDto>> GetPurchaseWithProductAndCustomer();
        Task<UpdatePurchaseDto> GetByIdAsync(int id);
        Task CreateAsync(CreatePurchaseDto purchaseDto);
        Task UpdateAsync(UpdatePurchaseDto purchaseDto);
        Task DeleteAsync(int id);
    }
}
