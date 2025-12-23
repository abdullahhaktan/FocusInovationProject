using AutoMapper;
using FocusInovationProject.Context;
using FocusInovationProject.DTOs.PurchaseDtos;
using FocusInovationProject.Entities;
using FocusInovationProject.Repositories.PurchaseRepositories;
using Microsoft.EntityFrameworkCore;

namespace FocusInovationProject.Repositories.PurchaseRepositories
{
    // Satınalma işlemlerinin veritabanı yönetimini üstlenen repository sınıfı
    public class PurchaseRepository(AppDbContext _context, IMapper _mapper) : IPurchaseRepository
    {
        private readonly DbSet<Purchase> _db = _context.Purchase;

        public async Task CreateAsync(CreatePurchaseDto purchaseDto)
        {
            // Gelen DTO'yu veritabanı modeline (Entity) çevirip asenkron olarak kaydediyoruz
            var purchase = _mapper.Map<Purchase>(purchaseDto);
            await _db.AddAsync(purchase);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // ID üzerinden kaydı bulup veritabanından siliyoruz
            var purchase = await _db.FindAsync(id);
            if (purchase != null)
            {
                _db.Remove(purchase);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ResultPurchaseDto>> GetAllAsync()
        {
            // Ham satınalma verilerini performans için takip etmeden (AsNoTracking) çekiyoruz
            var values = await _db.AsNoTracking().ToListAsync();
            var purchases = _mapper.Map<List<ResultPurchaseDto>>(values);
            return purchases;
        }

        public async Task<UpdatePurchaseDto> GetByIdAsync(int id)
        {
            // Güncelleme ekranı için tekil kayıt bilgisini getiriyoruz
            var value = await _db.FindAsync(id);
            var purchase = _mapper.Map<UpdatePurchaseDto>(value);
            return purchase;
        }

        public async Task<IEnumerable<ResultPurchaseDto>> GetPurchaseWithProductAndCustomer()
        {
            // UI tarafında hem Ürün hem de Müşteri bilgilerini göstermek için Eager Loading (Include) yapıyoruz
            var values = await _db.Include(p => p.CUSTOMER)
                                .Include(p => p.PRODUCT)
                                .AsNoTracking()
                                .ToListAsync();

            var purchases = _mapper.Map<List<ResultPurchaseDto>>(values);
            return purchases;
        }

        public async Task UpdateAsync(UpdatePurchaseDto purchaseDto)
        {
            // Gelen güncel verileri map edip kaydı güncelliyoruz
            var purchase = _mapper.Map<Purchase>(purchaseDto);
            _db.Update(purchase);
            await _context.SaveChangesAsync();
        }
    }
}