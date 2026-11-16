using System.Net;
using System.Text.Json;

namespace RealEstateApi.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new { Status = 500, Message = "Ocurrió un error interno en el servidor." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
