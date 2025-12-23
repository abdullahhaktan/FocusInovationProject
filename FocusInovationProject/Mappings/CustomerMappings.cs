using AutoMapper;
using FocusInovationProject.DTOs.CategoryDtos;
using FocusInovationProject.DTOs.CustomerDtos;
using FocusInovationProject.Entities;

namespace FocusInovationProject.Mappings
{
    public class CustomerMappings:Profile
    {
        public CustomerMappings()
        {
            CreateMap<Customer, ResultCustomerDto>().ReverseMap();

            CreateMap<Customer,CreateCustomerDto >().ReverseMap();

            CreateMap<Category, UpdateCustomerDto>().ReverseMap();
        }
    }
}
