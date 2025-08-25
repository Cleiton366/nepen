using System.Text.Json;
using Desafio_NEPEN.Com.Nepen.Core.Exceptions;

namespace Desafio_NEPEN.Com.Nepen.Core.Middlewares;

 public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message = exception.Message;
            
            if (exception is BadHttpRequestException)
                statusCode = StatusCodes.Status400BadRequest;
            else if (exception is ConflictException)
                statusCode = StatusCodes.Status409Conflict;
            else if (exception is UnprocessableEntityException)
                statusCode = StatusCodes.Status422UnprocessableEntity;
            else if (exception is NotFoundException)
                statusCode = StatusCodes.Status404NotFound;
            else
                statusCode = StatusCodes.Status500InternalServerError;

            var correlationId = context.TraceIdentifier;
            
            _logger.LogError(exception,
                "Exception occurred | CorrelationId: {CorrelationId} | StatusCode: {StatusCode} | Path: {Path}",
                correlationId, statusCode, context.Request.Path);

            var response = new
            {
                StatusCode = statusCode,
                Message = message,
                CorrelationId = correlationId
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }