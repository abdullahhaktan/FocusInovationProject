using FocusInovationProject.DTOs.ProductDtos;

namespace FocusInovationProject.Repositories.ProductRepositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ResultProductDto>> GetAllAsync();
        Task<IEnumerable<ResultProductDto>> GetProductsWithCategories();
        Task<UpdateProductDto> GetByIdAsync(int id);
        Task CreateAsync(CreateProductDto productDto);
        Task UpdateAsync(UpdateProductDto productDto);
        Task DeleteAsync(int id);
    }
}
