using System.Net;
using System.Text.Json;

namespace RailwayBooking.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType =
                    "application/json";

                context.Response.StatusCode =
                    (int)HttpStatusCode
                        .InternalServerError;

                var response = new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.ToString()
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        response));
            }
        }
    }
}
