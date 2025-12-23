using AutoMapper;
using FocusInovationProject.Context;
using FocusInovationProject.DTOs.CustomerDtos;
using FocusInovationProject.Entities;
using FocusInovationProject.Repositories.CustomerRepositories;
using Microsoft.EntityFrameworkCore;

namespace FocusInovationProject.Repositories.CustomerRepositories
{
    // Müşteri verileriyle ilgili veritabanı işlemlerini yürüten repository sınıfımız
    public class CustomerRepository(AppDbContext _context, IMapper _mapper) : ICustomerRepository
    {
        private readonly DbSet<Customer> _db = _context.Customer;

        public async Task CreateAsync(CreateCustomerDto customerDto)
        {
            // DTO olarak gelen veriyi veritabanı nesnesine (Entity) dönüştürüyoruz
            var customer = _mapper.Map<Customer>(customerDto);
            await _db.AddAsync(customer);
            // Yapılan değişikliği veritabanına fiziksel olarak yansıtıyoruz
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Silinecek müşteriyi id üzerinden buluyoruz
            var customer = await _db.FindAsync(id);
            // Eğer kayıt varsa silme işaretini koyup işlemi bitiriyoruz
            _db.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResultCustomerDto>> GetAllAsync()
        {
            // Performans için verileri sadece okuma amaçlı (tracking kapalı) çekiyoruz
            var values = await _db.AsNoTracking().ToListAsync();
            // Listeyi UI tarafında kullanılacak DTO formatına map'liyoruz
            var customers = _mapper.Map<List<ResultCustomerDto>>(values);
            return customers;
        }

        public async Task<UpdateCustomerDto> GetByIdAsync(int id)
        {
            // Tekil müşteri sorgusu yaparak primary key üzerinden veriyi getiriyoruz
            var value = await _db.FindAsync(id);
            // Gelen entity'yi güncelleme formuna uygun DTO'ya çeviriyoruz
            var customer = _mapper.Map<UpdateCustomerDto>(value);
            return customer;
        }

        public async Task UpdateAsync(UpdateCustomerDto customerDto)
        {
            // Güncellenecek veriyi map edip veritabanındaki durumunu güncelliyoruz
            var customer = _mapper.Map<Customer>(customerDto);
            _db.Update(customer);
            // SaveChanges ile update sorgusunu SQL'e gönderiyoruz
            await _context.SaveChangesAsync();
        }
    }
}