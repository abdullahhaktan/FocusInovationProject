using FocusInovationProject.DTOs.CustomerDtos;

namespace FocusInovationProject.Repositories.CustomerRepositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<ResultCustomerDto>> GetAllAsync();
        Task<UpdateCustomerDto> GetByIdAsync(int id);
        Task CreateAsync(CreateCustomerDto customerDto);
        Task UpdateAsync(UpdateCustomerDto customerDto);
        Task DeleteAsync(int id);
    }
}
