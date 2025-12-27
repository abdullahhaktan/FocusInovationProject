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
            // Tarih aralığı boş gelirse boş liste dönerek uygulamanın kırılmasını engelliyoruz
            if (!startDate.HasValue || !endDate.HasValue)
                return new List<CategorySalesReportDto>();

            // Satış tablosu üzerinden ürün ve kategori bilgilerini birleştirerek (Join) ham veriyi çekiyoruz (eager loading)
            var rawData = _context.Sales
                .Include(s => s.PRODUCT)
                .ThenInclude(p => p.CATEGORY)
                .Where(s => s.DATE >= startDate && s.DATE <= endDate)
                .Select(s => new
                {
                    CategoryName = s.PRODUCT.CATEGORY.NAME,
                    Qty = s.QUANTITY ?? 0, // Miktar null gelirse 0 olarak işleme al
                    Price = s.SALESPRICE ?? 0 // Satış fiyatı null gelirse 0 olarak işleme al
                })
                .ToList(); // Gruplama işlemi öncesi veriyi belleğe alıyoruz

            // Bellekteki veriyi kategori bazlı gruplayıp toplam tutar ve miktarları hesaplıyoruz
            var result = rawData
                .GroupBy(x => x.CategoryName)
                .Select(g => new CategorySalesReportDto
                {
                    CategoryName = g.Key,
                    TotalQuantity = g.Sum(x => x.Qty),
                    TotalAmount = g.Sum(x => x.Qty * x.Price)
                })
                .OrderByDescending(x => x.TotalAmount) // Ciroya göre en yüksekten düşüğe sıralama
                .ToList();

            return result;
        }

        public List<StockReportDto> GetStockReport()
        {
            // Mevcut stok durumunu ürün isimleriyle birlikte sorguluyoruz
            var rawStocks = _context.Stock
                .Select(s => new
                {
                    ProductName = s.PRODUCT.NAME,
                    Quantity = s.QUANTITY
                })
                .ToList();

            // Aynı ürün ismine sahip kayıtları birleştirerek toplam stok miktarını buluyoruz
            var result = rawStocks
                .GroupBy(x => x.ProductName)
                .Select(g => new StockReportDto
                {
                    ProductName = g.Key,
                    TotalStock = (double)g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalStock) // Stoğu en fazla olandan başlayarak listele
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