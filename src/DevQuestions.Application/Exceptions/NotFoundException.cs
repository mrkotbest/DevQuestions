using System.Text.Json;
using Shared;

namespace DevQuestions.Application.Exceptions;

public class NotFoundException : Exception
{
    protected NotFoundException(IEnumerable<Error> errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }
}
