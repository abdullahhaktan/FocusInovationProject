using AutoMapper;
using FocusInovationProject.DTOs.StockDtos;
using FocusInovationProject.Entities;

namespace FocusInovationProject.Mappings
{
    public class StockMappings : Profile
    {
        public StockMappings()
        {
            CreateMap<Stock, ResultStockDto>().ReverseMap();

            CreateMap<Stock, UpdateStockDto>().ReverseMap();

            CreateMap<Stock, CreateStockDto>().ReverseMap();
        }
    }
}
