using System.Collections;

namespace Shared;

public class Failure : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    public Failure(IEnumerable<Error> errors)
    {
        _errors = new List<Error>(errors);
    }

    public IEnumerator<Error> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static implicit operator Failure(Error[] errors)
    {
        return new Failure(errors);
    }

    public static implicit operator Failure(Error error)
    {
        return new Failure(new List<Error> { error });
    }
}
