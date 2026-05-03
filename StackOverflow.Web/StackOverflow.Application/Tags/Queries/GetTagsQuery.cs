using AutoMapper;
using BuildingBlocks.MediatR.Abstractions.Query;
using StackOverflow.Application.Abstractions;
using StackOverflow.Application.Tags.Dtos;

namespace StackOverflow.Application.Tags.Queries;

public class GetTagsQuery : IQuery<PagedResultDto<TagResponseDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public string SortBy { get; set; } = string.Empty;
    public string SortDirection { get; set; } = "asc";
}

public class GetTagsQueryHandler(
    ITagRepository tagRepository,
    IMapper mapper) : IQueryHandler<GetTagsQuery, PagedResultDto<TagResponseDto>>
{
    public async Task<PagedResultDto<TagResponseDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await tagRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var dtos = mapper.Map<IEnumerable<TagResponseDto>>(items);

        return new PagedResultDto<TagResponseDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
