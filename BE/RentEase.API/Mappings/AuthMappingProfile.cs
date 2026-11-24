using RentEase.API.DTO.Auth;
using RentEase.API.Entities;

namespace RentEase.API.Mappings;
using AutoMapper;

public class AuthMappingProfile: Profile
{
    public AuthMappingProfile()
    {
        CreateMap<RegisterRequest, User>();

    }
}