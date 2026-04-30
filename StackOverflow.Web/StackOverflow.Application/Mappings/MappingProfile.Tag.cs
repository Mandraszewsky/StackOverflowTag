using StackOverflow.Application.Tags.Dtos;
using StackOverflow.Domain.Entities;

namespace StackOverflow.Application.Mappings;

public partial class MappingProfile
{
    private void TagMappingProfile()
    {
        CreateMap<Tag, TagResponseDto>();
    }
}
