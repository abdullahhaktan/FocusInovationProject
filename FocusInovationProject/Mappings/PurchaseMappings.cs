using AutoMapper;
using FocusInovationProject.DTOs.PurchaseDtos;
using FocusInovationProject.Entities;

namespace FocusInovationProject.Mappings
{
    public class PurchaseMappings:Profile
    {
        public PurchaseMappings()
        {
            CreateMap<Purchase, CreatePurchaseDto>().ReverseMap();
            CreateMap<Purchase, ResultPurchaseDto>().ReverseMap();
            CreateMap<Purchase, UpdatePurchaseDto>().ReverseMap();
        }
    }
}
