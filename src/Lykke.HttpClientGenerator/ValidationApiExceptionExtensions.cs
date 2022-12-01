using Common;
using Lykke.HttpClientGenerator.Infrastructure;
using Refit;

namespace Lykke.HttpClientGenerator
{
    /// <summary>
    /// Text extensions for <see cref="ValidationApiException"/>
    /// </summary>
    public static class ValidationApiExceptionExtensions
    {
        private const string DomainErrorKey = "DomainError";
        
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
        /// Gets the mapped domain error code as string if <see cref="ProblemDetailsExceptionHandlerCallsWrapper"/>
        /// was used and API produced an error according to RFC 7807
        /// </summary>
        /// <returns>Domain error code</returns>
        public static string GetDomainErrorCode(this ValidationApiException exception)
        {
            if (exception.Data.Contains(DomainErrorKey))
                return exception.Data[DomainErrorKey]?.ToString() ?? string.Empty;

            return string.Empty;
        }

        /// <summary>
        /// Gets the mapped typed domain error if <see cref="ProblemDetailsExceptionHandlerCallsWrapper"/>
        /// was used and API produced an error according to RFC 7807
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Domain error</returns>
        public static T GetDomainError<T>(this ValidationApiException exception, T defaultValue = default)
        {
            if (exception.Data.Contains(DomainErrorKey))
            {
                if (exception.Data[DomainErrorKey] is T value)
                    return value;

                return defaultValue;
            }

            return defaultValue;
        }
        
        /// <summary>
        /// Sets the mapped from problem details domain error code as string
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="code">Error code</param>
        public static void SetDomainErrorCode(this ValidationApiException exception, string code)
        {
            exception.Data[DomainErrorKey] = code;
        }
        
        /// <summary>
        /// Sets the mapped from problem details typed domain error
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="value"></param>
        /// <typeparam name="T">Error</typeparam>
        public static void SetDomainError<T>(this ValidationApiException exception, T value)
        {
            exception.Data[DomainErrorKey] = value;
        }
    }
}