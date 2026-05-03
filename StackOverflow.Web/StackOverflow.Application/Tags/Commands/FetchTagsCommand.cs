using BuildingBlocks.MediatR.Abstractions.Command;
using MediatR;
using StackOverflow.Application.Abstractions;

namespace StackOverflow.Application.Tags.Commands;

public class FetchTagsCommand : ICommand<Unit>
{
}

public class FetchTagsCommandHandler(
    IStackOverflowApiService stackOverflowApiService,
    ITagRepository tagRepository) : ICommandHandler<FetchTagsCommand, Unit>
{
    public async Task<Unit> Handle(FetchTagsCommand request, CancellationToken cancellationToken)
    {
        var tagsExist = await tagRepository.AnyAsync(cancellationToken);

        if (tagsExist)
            return Unit.Value;

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