using System.Text.Json.Serialization;

namespace Shared;

public record Error
{
    public string Code { get; }
    public string Message { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Errortype Type { get; }
    public string? InvalidField { get; }

    [JsonConstructor]
    private Error(string code, string message, Errortype type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error Validation(string? code, string message, string? invalidField = null)
    {
        return new Error(code ?? "value.invalid", message, Errortype.Validation, invalidField);
    }

    public static Error NotFound(string? code, string message, Guid? id)
    {
        return new Error(code ?? "record.not.found", message, Errortype.NotFound);
    }

    public static Error Unauthorized(string? code, string message)
    {
        return new Error(code ?? "unauthorized", message, Errortype.Unauthorized);
    }

    public static Error Forbidden(string? code, string message)
    {
        return new Error(code ?? "forbidden", message, Errortype.Forbidden);
    }

    public static Error Conflict(string? code, string message)
    {
        return new Error(code ?? "conflict", message, Errortype.Conflict);
    }

    public static Error InternalServerError(string? code, string message)
    {
        return new Error(code ?? "internal.server.error", message, Errortype.InternalServerError);
    }
}

public enum Errortype
{
    /// <summary> The request was invalid due to validation errors. /// </summary>
    Validation,

    /// <summary> The requested resource was not found. This could be due to an invalid identifier or the resource being deleted. </summary>
    NotFound,

    /// <summary> The request was unauthorized. This could be due to missing or invalid authentication credentials, or the user not having the necessary permissions to access the resource. </summary>
    Unauthorized,

    /// <summary> The request was forbidden. This could be due to the user not having the necessary permissions to perform the requested action, or the resource being restricted. </summary>
    Forbidden,

    /// <summary> The request could not be completed due to a conflict with the current state of the resource. This could be due to a version mismatch, or the resource being modified by another user or process. </summary>
    Conflict,

    /// <summary> The server encountered an unexpected condition that prevented it from fulfilling the request. This could be due to a bug in the application, a misconfiguration, or an external dependency failing. </summary>
    InternalServerError
}
