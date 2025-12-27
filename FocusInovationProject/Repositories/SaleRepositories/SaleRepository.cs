using AutoMapper;
using FocusInovationProject.Context;
using FocusInovationProject.DTOs.SaleDtos;
using FocusInovationProject.Entities;
using FocusInovationProject.Repositories.SaleRepositories;
using Microsoft.EntityFrameworkCore;

namespace FocusInovationProject.Repositories.SaleRepositories
{
    // Satış işlemlerini ve buna bağlı hesaplamaları yönettiğimiz repository sınıfı
    public class SaleRepository(AppDbContext _context, IMapper _mapper) : ISaleRepository
    {
        private readonly DbSet<Sale> _db = _context.Sales;

        public async Task CreateAsync(CreateSaleDto saleDto)
        {
            // Yeni satış kaydını entity'ye çevirip DB'ye ekliyoruz
            var sale = _mapper.Map<Sale>(saleDto);
            await _db.AddAsync(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Silinecek kaydı ID üzerinden bulup kaldırıyoruz
            var sale = await _db.FindAsync(id);
            if (sale != null)
            {
                _db.Remove(sale);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ResultSaleDto>> GetAllAsync()
        {
            // Basit listeleme işlemlerinde tracking'i kapatarak performans kazanıyoruz
            var values = await _db.AsNoTracking().ToListAsync();
            var sales = _mapper.Map<List<ResultSaleDto>>(values);
            return sales;
        }

        public async Task<UpdateSaleDto> GetByIdAsync(int id)
        {
            // Tek bir kaydı bulup güncelleme DTO'su şeklinde döndürüyoruz
            var value = await _db.FindAsync(id);
            var sale = _mapper.Map<UpdateSaleDto>(value);
            return sale;
        }

        public async Task<IEnumerable<ResultSaleDto>> GetSalesWithCustomerAndProduct()
        {
            // Satış listesini çekerken müşteri ve ürün bilgilerini de (Join) dahil ediyoruz
            var values = await _db.Include(s => s.CUSTOMER)
                                .Include(s => s.PRODUCT)
                                .AsNoTracking()
                                .ToListAsync();

            foreach(var value in values)
            {
                value.DISCOUNTRATE = ((value.LISTPRICE - value.SALESPRICE) / value.LISTPRICE) * 100;
            }

            var products = _mapper.Map<List<ResultSaleDto>>(values);
            return products;
        }

        public async Task SaveChanges()
        {
            // Yapılan değişiklikleri toplu olarak kaydetmek için kullandığımız metod
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateSaleDto saleDto)
        {
            // Gelen güncel verileri entity'ye map edip güncelliyoruz
            var sale = _mapper.Map<Sale>(saleDto);
            _db.Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateListPriceAsync(int id, double calculatedPrice)
        {
            // İlgili satışın liste fiyatını güncelleyip kaydediyoruz
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return;

            sale.LISTPRICE = calculatedPrice;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int productId, double quantityChange)
        {
            // Satış yapıldığında stoktan miktar düşürme işlemini burada yapıyoruz
            var stock = await _context.Stock.FirstOrDefaultAsync(s => s.PRODUCT_ID == productId);
            if (stock == null) return;

            stock.QUANTITY -= quantityChange;

            // Değişiklikleri SQL tarafına yansıtıyoruz
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSalesPriceAsync(int id, double salesPrice)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return;

            // Yeni satış fiyatını sisteme işliyoruz
            sale.SALESPRICE = salesPrice;

            // Liste fiyatı üzerinden otomatik iskonto oranını hesaplıyoruz
            double? listPrice = sale.LISTPRICE;
            double? discountRate = 0;

            if (listPrice > 0)
                discountRate = ((listPrice - salesPrice) / (double)listPrice) * 100;

            sale.DISCOUNTRATE = discountRate ?? 0;

            // Hesaplanan yeni değerleri kaydediyoruz
            await _context.SaveChangesAsync();
        }
    }
}