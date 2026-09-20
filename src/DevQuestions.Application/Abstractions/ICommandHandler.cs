using CSharpFunctionalExtensions;
using Shared;

namespace DevQuestions.Application.Abstractions;

public interface ICommandHandler<TResponse, in TCommand>
    where TCommand : ICommand
    where TResponse : notnull
{
    Task<Result<TResponse, Failure>> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<Failure>> Handle(TCommand command, CancellationToken cancellationToken);
}
