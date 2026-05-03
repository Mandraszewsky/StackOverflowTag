using BuildingBlocks.MediatR.Abstractions.Command;
using MediatR;
using StackOverflow.Application.Abstractions;

namespace StackOverflow.Application.Tags.Commands;

public class RefreshTagsCommand : ICommand<Unit>
{
}

public class RefreshTagsCommandHandler(
    IStackOverflowApiService stackOverflowApiService,
    ITagRepository tagRepository) : ICommandHandler<RefreshTagsCommand, Unit>
{
    public async Task<Unit> Handle(RefreshTagsCommand request, CancellationToken cancellationToken)
    {
        var tags = await stackOverflowApiService.FetchTagsAsync(1000, cancellationToken);
        var totalCount = tags.Sum(t => t.Count);

        foreach (var tag in tags)
        {
            tag.Percentage = totalCount > 0 ? (double)tag.Count / totalCount * 100 : 0;
            tag.FetchedAt = DateTime.UtcNow;
        }

        await tagRepository.ReplaceAllAsync(tags, cancellationToken);

        return Unit.Value;
    }
}
