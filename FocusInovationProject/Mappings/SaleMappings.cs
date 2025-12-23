using AutoMapper;
using FocusInovationProject.DTOs.SaleDtos;
using FocusInovationProject.Entities;

namespace FocusInovationProject.Mappings
{
    public class SaleMappings:Profile
    {
        public SaleMappings()
        {
            CreateMap<Sale, ResultSaleDto>().ReverseMap();

            CreateMap<Sale, UpdateSaleDto>().ReverseMap();

            CreateMap<Sale, CreateSaleDto>().ReverseMap();
        }
    }
}
