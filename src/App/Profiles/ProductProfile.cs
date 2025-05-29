using App.DTOs;
using AutoMapper;
using Dom.Entities;

namespace App.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
