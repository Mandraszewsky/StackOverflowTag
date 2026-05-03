using StackOverflow.Domain.Entities;

namespace StackOverflow.Application.Abstractions;

public interface IStackOverflowApiService
{
    Task<List<Tag>> FetchTagsAsync(int minCount = 1000, CancellationToken cancellationToken = default);
}
