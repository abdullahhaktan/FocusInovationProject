using AutoMapper;
using DevExpress.DirectX.Common.Direct2D;
using FocusInovationProject.Context;
using FocusInovationProject.DTOs.ProductDtos;
using FocusInovationProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace FocusInovationProject.Repositories.ProductRepositories
{
    // Ürünlerin veritabanı operasyonlarını yöneten Repository sınıfı
    public class ProductRepository(AppDbContext _context, IMapper _mapper) : IProductRepository
    {
        private readonly DbSet<Product> _db = _context.Product;

        public async Task CreateAsync(CreateProductDto productDto)
        {
            // AutoMapper kullanarak DTO'dan asıl Product entity'sine dönüşüm yapıyoruz
            var product = _mapper.Map<Product>(productDto);
            await _db.AddAsync(product);
            // Transaction'ı tamamlayıp veriyi fiziksel tabloya yazıyoruz
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Silinecek ürünü asenkron olarak bulup bellek üzerine alıyoruz
            var product = await _db.FindAsync(id);
            // Bellekteki nesneyi silinecek olarak işaretleyip DB'den kaldırıyoruz
            _db.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResultProductDto>> GetAllAsync()
        {
            // Sadece listeleme yapacağımız için tracking mekanizmasını kapatıp performansı artırıyoruz
            var values = await _db.AsNoTracking().ToListAsync();
            var products = _mapper.Map<List<ResultProductDto>>(values);
            return products;
        }

        public async Task<UpdateProductDto> GetByIdAsync(int id)
        {
            // Güncelleme işlemi öncesi mevcut ürünü ID üzerinden getiriyoruz
            var value = await _db.FindAsync(id);
            var product = _mapper.Map<UpdateProductDto>(value);
            return product;
        }

        public async Task<IEnumerable<ResultProductDto>> GetProductsWithCategories()
        {
            // Eager Loading kullanarak ürünle birlikte bağlı olduğu kategori bilgisini de tek sorguda çekiyoruz
            var values = await _db.Include(p => p.CATEGORY).AsNoTracking().ToListAsync();
            var products = _mapper.Map<List<ResultProductDto>>(values);
            return products;
        }

        public async Task UpdateAsync(UpdateProductDto productDto)
        {
            // Gelen güncel verileri entity modeline aktarıp durumunu 'Modified' olarak güncelliyoruz
            var product = _mapper.Map<Product>(productDto);
            _db.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}