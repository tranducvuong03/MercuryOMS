namespace MercuryOMS.API.Middlewares
{
    public class RequestTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            await _next(context);
            var endTime = DateTime.UtcNow;
            var processingTime = endTime - startTime;
            Console.WriteLine($"Request processed in {processingTime.TotalMilliseconds} ms");
        }
    }
}
