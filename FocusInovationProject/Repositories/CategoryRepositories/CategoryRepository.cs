using AutoMapper;
using FocusInovationProject.Context;
using FocusInovationProject.DTOs.CategoryDtos;
using FocusInovationProject.DTOs.CategoryDtos.FocusInovationProject.DTOs.CategoryDtos;
using FocusInovationProject.DTOs.StockDtos;
using FocusInovationProject.Entities;
using FocusInovationProject.Repositories.CategoryRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FocusInovationProject.Repositories.CategoryRepositories
{
    // Kategori işlemleri ve raporlama verilerinin hazırlandığı veri erişim katmanı
    public class CategoryRepository(AppDbContext _context, IMapper _mapper) : ICategoryRepository
    {
        private readonly DbSet<Category> _db = _context.Category;

        public List<CategorySalesReportDto> GetCategorySalesReport(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
                return new List<CategorySalesReportDto>();

            var result = _context.Sales
                .Where(s => s.DATE >= startDate && s.DATE <= endDate)
                .GroupBy(s => s.PRODUCT.CATEGORY.NAME)
                .Select(g => new CategorySalesReportDto
                {
                    CategoryName = g.Key,
                    TotalQuantity = g.Sum(x => x.QUANTITY ?? 0),
                    TotalAmount = g.Sum(x => (x.QUANTITY ?? 0) * (x.SALESPRICE ?? 0))
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            return result;
        }


        public List<StockReportDto> GetStockReport()
        {
            var result = _context.Stock
                .GroupBy(s => s.PRODUCT.NAME)
                .Select(g => new StockReportDto
                {
                    ProductName = g.Key,
                    TotalStock = g.Sum(x => x.QUANTITY)
                })
                .OrderByDescending(x => x.TotalStock)
                .ToList();

            return result;
        }


        public async Task CreateAsync(CreateCategoryDto categoryDto)
        {
            // DTO'dan gelen veriyi AutoMapper ile Entity modeline dönüştürüp ekliyoruz
            var category = _mapper.Map<Category>(categoryDto);
            await _db.AddAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            // ID üzerinden kaydı bulup veritabanı takip listesinden çıkarıyoruz
            var category = await _db.FindAsync(id);
            if (category != null)
                _db.Remove(category);
        }

        public async Task<IEnumerable<ResultCategoryDto>> GetAllAsync()
        {
            // Salt okuma (AsNoTracking) yaparak performansı optimize ediyoruz
            var values = await _db.AsNoTracking().ToListAsync();
            return _mapper.Map<List<ResultCategoryDto>>(values);
        }

        public async Task<UpdateCategoryDto> GetByIdAsync(int id)
        {
            // Güncelleme işlemi için tekil kayıt getirip DTO'ya çeviriyoruz
            var value = await _db.FindAsync(id);
            return _mapper.Map<UpdateCategoryDto>(value);
        }

        public async Task UpdateAsync(UpdateCategoryDto categoryDto)
        {
            // Mevcut entity modelini gelen güncel bilgilerle güncelliyoruz
            var category = _mapper.Map<Category>(categoryDto);
            _db.Update(category);
        }
    }
}