using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace Lykke.HttpClientGenerator.Infrastructure
{
    /// <summary>
    /// A delegate to map API error code (RFC 7807) to domain error
    /// </summary>
    [PublicAPI]
    public delegate object MapProblemDetailsErrorDelegate(string errorCode);
    
    /// <summary>
    /// Calls wrapper to read API error code (RFC 7807) and map it to domain error.
    /// Only first error will be used.
    /// The mapping is optional and done by <see cref="MapProblemDetailsErrorDelegate"/> delegate.
    /// The original exception will be thrown in any case.
    /// If mapper was not specified, the original error code will be used instead. 
    /// If the API error code was successfully mapped to domain error or mapper was not specified,
    /// the mapped error code / API error code will be added to the exception's
    /// Data collection and can be read later with
    /// <see cref="ValidationApiExceptionExtensions.GetDomainErrorCode"/>
    /// or <see cref="ValidationApiExceptionExtensions.GetDomainError"/> methods.
    /// Otherwise, the original API error code can be taken from <see cref="ProblemDetails.Errors"/> dictionary.
    /// </summary>
    [PublicAPI]
    public sealed class ProblemDetailsExceptionHandlerCallsWrapper : ICallsWrapper
    {
        [CanBeNull] private readonly MapProblemDetailsErrorDelegate _errorCodeMapper;
        
        /// <summary>
        /// Creates an instance of <see cref="ProblemDetailsExceptionHandlerCallsWrapper"/>
        /// </summary>
        /// <param name="errorCodeMapper">Optional API error to domain error mapper</param>
        public ProblemDetailsExceptionHandlerCallsWrapper([CanBeNull] MapProblemDetailsErrorDelegate errorCodeMapper)
        {
            _errorCodeMapper = errorCodeMapper;
        }
        
        /// <summary>
        /// Handles API error code (RFC 7807) and maps it to domain error if
        /// mapper was specified.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <param name="innerHandler"></param>
        /// <returns></returns>
        public async Task<object> HandleMethodCall(MethodInfo targetMethod, 
            object[] args, 
            Func<Task<object>> innerHandler)
        {
            try
            {
                return await innerHandler();
            }
            catch (ValidationApiException ex)
            {
                var problemDetails = await ex.GetContentAsAsync<ProblemDetails>();

                var apiErrorCode = problemDetails?.Errors.Keys.FirstOrDefault();

                if (apiErrorCode != null)
                {
                    if (_errorCodeMapper == null)
                    {
                        ex.SetDomainError(apiErrorCode);
                    }
                    else
                    {
                        var mappedError = _errorCodeMapper.Invoke(apiErrorCode);
                        ex.SetDomainError(mappedError);
                    }
                }

                throw;
            }
        }
    }
}