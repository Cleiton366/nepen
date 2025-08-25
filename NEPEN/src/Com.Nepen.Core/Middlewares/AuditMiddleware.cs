using System.Diagnostics;
using Serilog;

namespace Desafio_NEPEN.Com.Nepen.Core.Middlewares;

public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = context.TraceIdentifier;

            var originalBody = context.Response.Body ?? new MemoryStream();
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                stopwatch.Stop();

                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                Log.Information(
                    "Request completed | CorrelationId: {CorrelationId} | Method: {Method} | Path: {Path} | StatusCode: {StatusCode} | DurationMs: {Duration} | Response: {Response}",
                    correlationId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    responseText
                );
                
                await responseBody.CopyToAsync(originalBody);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                Log.Error(ex,
                    "Request failed | CorrelationId: {CorrelationId} | Method: {Method} | Path: {Path} | DurationMs: {Duration}",
                    correlationId,
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds
                );

                throw;
            }
            finally
            {
                context.Response.Body = originalBody; 
            }
        }
    }