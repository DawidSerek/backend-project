using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Pesel;
using AutoMapper;
namespace ApplicationCore.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Contact, ResultContactDto>();
        CreateMap<CreateContactDto, Contact>();
        CreateMap<CreateContactDto, Organization>();
        CreateMap<Person, ResultPersonDto>()
            .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src =>
                src.Organization != null ? (Guid?)src.Organization.Id : null));
        CreateMap<CreatePersonDto, Person>()
            .ForMember(dest => dest.PESEL, opt => opt.MapFrom(src => new PESEL(src.Pesel)))
            .ForMember(dest => dest.Organization, opt => opt.Ignore());
    }
}
