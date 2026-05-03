using StackOverflow.Domain.Entities;

namespace StackOverflow.Application.Abstractions;

public interface ITagRepository
{
    Task<(IEnumerable<Tag> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    Task ReplaceAllAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken = default);
}
