using AutoMapper;
using FocusInovationProject.Context;
using FocusInovationProject.DTOs.SaleDtos;
using FocusInovationProject.DTOs.StockDtos;
using FocusInovationProject.Entities;
using FocusInovationProject.Repositories.StockRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FocusInovationProject.Repositories.StockRepositories
{
    // Stok miktarlarını ve ürün eşleşmelerini yöneten repository katmanı
    public class StockRepository(AppDbContext _context, IMapper _mapper) : IStockRepository
    {
        private readonly DbSet<Stock> _db = _context.Stock;

        public async Task CreateAsync(CreateStockDto stockDto)
        {
            // Yeni stok kartını oluşturup veritabanına kaydediyoruz
            var stock = _mapper.Map<Stock>(stockDto);
            await _db.AddAsync(stock);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // İlgili stok kaydını bulup sistemden kaldırıyoruz
            var stock = await _db.FindAsync(id);
            if (stock != null)
            {
                _db.Remove(stock);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ResultStockDto>> GetAllAsync()
        {
            // Tüm stok listesini performans için takip etmeden (AsNoTracking) çekiyoruz
            var values = await _db.AsNoTracking().ToListAsync();
            var stocks = _mapper.Map<List<ResultStockDto>>(values);
            return stocks;
        }

        public async Task<UpdateStockDto> GetByIdAsync(int id)
        {
            // Stok kaydını kendi ID'si üzerinden sorguluyoruz
            var value = await _db.FindAsync(id);
            var stock = _mapper.Map<UpdateStockDto>(value);
            return stock;
        }

        public async Task<UpdateStockDto> GetByProductIdAsync(int id)
        {
            // Ürün ID'sine göre stok bilgisini getiren yardımcı metod
            var value = await _db.FirstOrDefaultAsync(s => s.PRODUCT_ID == id);
            return _mapper.Map<UpdateStockDto>(value);
        }

        public async Task<IEnumerable<ResultStockDto>> GetStocksWithProduct()
        {
            // Stokları listelerken ürün detaylarını da beraberinde getiriyoruz (Join işlemi) fakat listeleme işleminde trackinge gerek olmadığı için AsNoTracking ekledik
            var values = await _db.Include(s => s.PRODUCT).AsNoTracking().ToListAsync();
            var stocks = _mapper.Map<List<ResultStockDto>>(values);
            return stocks;
        }

        public async Task UpdateAsync(UpdateStockDto stockDto)
        {
            // Stok bilgilerini güncelleyip değişiklikleri kaydediyoruz
            var stock = _mapper.Map<Stock>(stockDto);
            _db.Update(stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int productId, double quantityChange)
        {
            // Satış gibi durumlarda stok miktarını düşürmek için kullanıyoruz
            var stock = await _db.FirstOrDefaultAsync(s => s.PRODUCT_ID == productId);
            if (stock == null) return;

            stock.QUANTITY -= quantityChange;

            // Stok miktarının eksiye düşmemesi için kontrol mekanizması (Business Rule)
            if (stock.QUANTITY < 0)
            {
                stock.QUANTITY = 0; // Stok negatif değer almasın diye sıfıra set ediyoruz
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityIncreasingAsync(int productId, double quantityChange)
        {
            // Satınalma durumunda stok miktarını artırmak için kullanıyoruz
            var stock = await _db.FirstOrDefaultAsync(s => s.PRODUCT_ID == productId);
            if (stock == null) return;

            stock.QUANTITY += quantityChange;

            await _context.SaveChangesAsync();
        }
    }
}
