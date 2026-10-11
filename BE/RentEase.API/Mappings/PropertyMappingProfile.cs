using AutoMapper;
using RentEase.API.DTOs.Properties;
using RentEase.API.Entities;

namespace RentEase.API.Mappings;

public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        // Create property
        CreateMap<CreatePropertyRequest, Property>()
            .ForMember(dest => dest.Owner, opt => opt.Ignore());

        // Update property
        CreateMap<UpdatePropertyRequest, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore());

        // Convert property entity to response DTO
        CreateMap<Property, PropertyResponse>();
    }
}