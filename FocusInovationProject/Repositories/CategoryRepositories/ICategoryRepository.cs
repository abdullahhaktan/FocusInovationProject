using FocusInovationProject.DTOs.CategoryDtos;
using FocusInovationProject.DTOs.CategoryDtos.FocusInovationProject.DTOs.CategoryDtos;
using FocusInovationProject.DTOs.StockDtos;
using Microsoft.AspNetCore.Mvc;

namespace FocusInovationProject.Repositories.CategoryRepositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<ResultCategoryDto>> GetAllAsync();
        Task<UpdateCategoryDto> GetByIdAsync(int id);
        Task CreateAsync(CreateCategoryDto categoryDto);
        Task UpdateAsync(UpdateCategoryDto categoryDto);
        Task DeleteAsync(int id);
        List<CategorySalesReportDto> GetCategorySalesReport(DateTime? startDate, DateTime? endDate);
        List<StockReportDto> GetStockReport();

    }
}
