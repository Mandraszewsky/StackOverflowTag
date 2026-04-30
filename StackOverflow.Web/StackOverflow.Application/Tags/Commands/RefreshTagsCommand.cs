using BuildingBlocks.MediatR.Abstractions.Command;
using MediatR;

namespace StackOverflow.Application.Tags.Commands;

public class RefreshTagsCommand : ICommand<Unit>
{

}

public class RefreshTagsCommandHandler : ICommandHandler<RefreshTagsCommand, Unit>
{
    public Task<Unit> Handle(RefreshTagsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
