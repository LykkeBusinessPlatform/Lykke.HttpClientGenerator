using System.Linq;
using Common;
using Refit;

namespace Lykke.HttpClientGenerator
{
    /// <summary>
    /// Text extensions for <see cref="ValidationApiException"/>
    /// </summary>
    public static class ValidationApiExceptionExtensions
    {
        /// <summary>
        /// Get http request caused the exception problem details as text 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetProblemDetailsPhrase(this ValidationApiException exception)
        {
            return exception.Content != null ? $"Problem details: {exception.Content.ToJson()}." : string.Empty;
        }

        /// <summary>
        /// Get exception description including all the details about request and error, as text
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetDescription(this ValidationApiException exception)
        {
            return $"Couldn't execute http request. {exception.GetProblemDetailsPhrase()} {exception.GetRequestPhrase()}";
        }

        /// <summary>
        /// Gets the first error code and message from the Errors collection
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static (string errorCode, string message) GetFirstError(this ValidationApiException exception)
        {
            var firstErrorCode = exception?.Content.Errors.FirstOrDefault().Key;
            
            if (firstErrorCode == null)
                return (string.Empty, string.Empty);
            
            var firstErrorMessage = exception.Content.Errors[firstErrorCode].FirstOrDefault();
            
            return (firstErrorCode, firstErrorMessage);
        }
    }
}