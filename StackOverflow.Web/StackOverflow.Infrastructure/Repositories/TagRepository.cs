using Microsoft.EntityFrameworkCore;
using StackOverflow.Application.Abstractions;
using StackOverflow.Domain.Entities;
using StackOverflow.Infrastructure.Data;

namespace StackOverflow.Infrastructure.Repositories;

public class TagRepository(ApplicationDbContext dbContext) : ITagRepository
{
    public async Task<(IEnumerable<Tag> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tags.AsNoTracking();

        query = ApplySorting(query, sortBy, sortDirection);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Tags.AnyAsync(cancellationToken);
    }

    public async Task ReplaceAllAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        dbContext.Tags.RemoveRange(dbContext.Tags);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Tags.AddRangeAsync(tags, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private static IQueryable<Tag> ApplySorting(IQueryable<Tag> query, string sortBy, string sortDirection)
    {
        var isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "name" => isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
            "percentage" => isDescending ? query.OrderByDescending(t => t.Percentage) : query.OrderBy(t => t.Percentage),
            _ => isDescending ? query.OrderByDescending(t => t.Count) : query.OrderBy(t => t.Count)
        };
    }
}
