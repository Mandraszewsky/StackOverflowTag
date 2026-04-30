using BuildingBlocks.MediatR.Abstractions.Command;
using MediatR;

namespace StackOverflow.Application.Tags.Commands;

public class FetchTagsCommand : ICommand<Unit>
{

}

public class FetchTagsCommandHandler : ICommandHandler<FetchTagsCommand, Unit>
{
    public Task<Unit> Handle(FetchTagsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}   