using System.Net;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Http;
using Refit;

namespace Lykke.HttpClientGenerator.Infrastructure
{
    /// <summary>
    /// Default implementation of <see cref="IValidationApiExceptionHandler"/>.
    /// Takes the first error and message from <see cref="ProblemDetails"/> object
    /// and writes it to the response with <see cref="HttpStatusCode.Conflict"/>
    /// </summary>
    public sealed class DefaultValidationApiExceptionHandler : IValidationApiExceptionHandler
    {
        private const string JsonContentType = "application/json";

        struct ErrorResponse
        {
            public string Message { get; set; }
            public string ErrorCode { get; set; }
        }
        
        public async Task HandleAsync(HttpContext context, ValidationApiException exception)
        {
            var firstErrorResponse = CreateFirstErrorResponse(exception);

            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            context.Response.ContentType = JsonContentType;
            
            await context.Response.WriteAsync(firstErrorResponse.ToJson());
        }

        private static ErrorResponse CreateFirstErrorResponse(ValidationApiException exception)
        {
            var (errorCode, errorMessage) = exception.GetFirstError();
            return new ErrorResponse { ErrorCode = errorCode, Message = errorMessage };
        }
    }
}