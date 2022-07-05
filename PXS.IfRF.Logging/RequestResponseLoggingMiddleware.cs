using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace PXS.IfRF.Logging
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly ILoggerManager _logger;

        private readonly RequestDelegate _next;


        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //code dealing with the request
            await _next(context);
            //code dealing with the response
            _logger.LogWebRequest(null, context, true, sw.ElapsedMilliseconds);
        }



    }
}
