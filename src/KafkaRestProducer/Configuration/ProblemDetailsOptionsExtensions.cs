namespace KafkaRestProducer.Configuration;

using System.Runtime.Serialization;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

public static class ProblemDetailsOptionsExtensions
{
    public static Action<ProblemDetailsOptions> Options()
    {
        return options =>
        {
            options.IncludeExceptionDetails = (ctx, ex) => false;

            options.Map<ArgumentException>(BadRequestProblem);
            options.Map<SerializationException>(BadRequestProblem);
            options.Map<DllNotFoundException>(BadRequestProblem);

            options.Map<Exception>(ex => new ProblemDetails
            {
                Type = nameof(Exception),
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message
            });
        };
    }

    private static ProblemDetails BadRequestProblem(Exception exception)
        => new()
        {
            Type = exception.GetType().ToString(),
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message
        };
}
