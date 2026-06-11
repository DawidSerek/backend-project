using ApplicationCore.Models;
using AutoMapper;
namespace ApplicationCore.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Contact, ContactResultDto>();
        CreateMap<ContactCreateDto, Contact>();
    }
}
