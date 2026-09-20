using CSharpFunctionalExtensions;
using DevQuestions.Application.Abstractions;
using Shared;

namespace DevQuestions.Application.Questions.Features.SelectSolution;

public class SelectSolutionHandler : ICommandHandler<Guid, SelectSolutionCommand>
{
    public SelectSolutionHandler()
    {
    }

    public Task<Result<Guid, Failure>> Handle(SelectSolutionCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
