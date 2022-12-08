using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Refit;

namespace Lykke.HttpClientGenerator.Infrastructure
{
    /// <summary>
    /// Handles <see cref="ValidationApiException"/>
    /// </summary>
    public interface IValidationApiExceptionHandler
    {
        /// <summary>
        /// The implementation is supposed to handle <see cref="ValidationApiException"/>
        /// and optionally write a response
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task HandleAsync(HttpContext context, ValidationApiException exception);
    }
}