using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Infrastructure
{
    public class CustomResponseHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _key;
        private readonly string _value;

        public CustomResponseHeaderMiddleware(RequestDelegate next, string key, string value)
        {
            _next = next;
            _key = key;
            _value = value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting((state) =>
            {
                var httpContext = (HttpContext)state;
                if (!httpContext.Response.Headers.TryGetValue(_key.ToLowerInvariant(), out StringValues headerValue))
                {
                    httpContext.Response.Headers.Add(_key.ToLowerInvariant(), _value);
                }
                return Task.FromResult(0);
            }, context);

            await _next(context);
        }
    }

    public static class CustomResponseHeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomHeader(
            this IApplicationBuilder builder, string key, string value)
        {
            return builder.UseMiddleware<CustomResponseHeaderMiddleware>(key, value);
        }
    }
}
