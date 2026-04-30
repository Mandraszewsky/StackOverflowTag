using AutoMapper;

namespace StackOverflow.Application.Mappings;

public partial class MappingProfile : Profile
{
    public MappingProfile()
    {
        TagMappingProfile();
    }
}
