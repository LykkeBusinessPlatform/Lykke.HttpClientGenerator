using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Refit;

namespace Lykke.HttpClientGenerator.Exceptions
{
    /// <summary>
    /// Refit specific exceptions handling middleware.
    /// The following exceptions are handled:
    /// <list type="bullet">
    ///     <item>
    ///         <see cref="ApiException"/>
    ///     </item>
    ///     <item>
    ///         <see cref="ValidationApiException"/>
    ///     </item>
    ///     <item>
    ///         <see cref="HttpClientApiException"/>
    ///     </item>
    /// </list>
    ///
    /// Default behaviour is to log exception details with
    /// <see cref="LogLevel.Error"/> level and optionally rethrow exception.
    ///
    /// If <see cref="IValidationApiExceptionHandler"/> implementation is
    /// registered in DI container, it will be used to handle
    /// <see cref="ValidationApiException"/> instead of default behaviour. In this
    /// case exception will not be rethrown.
    /// </summary>
    public class RefitExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RefitExceptionHandlingOptions _options;
        private readonly ILogger<RefitExceptionHandlerMiddleware> _logger;
        [CanBeNull] private readonly IValidationApiExceptionHandler _validationApiExceptionHandler;

        /// <summary>
        /// Creates middleware
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="validationApiExceptionHandler"></param>
        public RefitExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<RefitExceptionHandlerMiddleware> logger,
            RefitExceptionHandlingOptions options,
            IValidationApiExceptionHandler validationApiExceptionHandler)
        {
            _next = next;
            _logger = logger;
            _options = options;
            _validationApiExceptionHandler = validationApiExceptionHandler;
        }

        /// <summary>
        /// Executes upon middleware invocation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var options = _options ?? RefitExceptionHandlingOptions.CreateDefault();
                
                switch (exception)
                {
                    case ValidationApiException ex:
                        if (_validationApiExceptionHandler != null)
                        {
                            await _validationApiExceptionHandler.HandleAsync(context, ex);
                            return;
                        }
                        _logger.LogError(ex, ex.GetDescription());
                        break;
                    case ApiException ex:
                        _logger.LogError(ex, ex.GetDescription());
                        break;
                    case HttpClientApiException ex:
                        _logger.LogError(ex, ex.GetDescription());
                        break;
                    default: throw;
                }
                
                if (options.ReThrow)
                    throw;
            }
        }
    }
}