using BuildingBlocks.MediatR.Abstractions.Query;
using StackOverflow.Application.Tags.Dtos;

namespace StackOverflow.Application.Tags.Queries;

public class GetTagsQuery : IQuery<PagedResultDto<TagResponseDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public string SortBy { get; set; } = string.Empty;
    public string SortDirection { get; set; } = "asc";
}

public class GetTagsQueryHandler : IQueryHandler<GetTagsQuery, PagedResultDto<TagResponseDto>>
{
    public Task<PagedResultDto<TagResponseDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
