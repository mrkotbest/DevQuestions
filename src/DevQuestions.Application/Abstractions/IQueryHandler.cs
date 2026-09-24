namespace DevQuestions.Application.Abstractions;

public interface IQueryHandler<TResponse, in TQuery>
    where TQuery : IQuery
    where TResponse : notnull
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
