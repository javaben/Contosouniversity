using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web1
{
    public static class HelloMiddlewareHelper
    {
        public static void UseHelloWorld(this IApplicationBuilder app)
        {
            app.UseMiddleware<HelloMiddleware>();
        }
    }

    public class HelloMiddleware
    {
        private readonly RequestDelegate _next;

        public HelloMiddleware(RequestDelegate next, IOptions<HelloMessage> MessageOpt)
        {
            _next = next;
            this.Message = MessageOpt.Value;
        }

        public HelloMessage Message { get; }

        public async Task InvokeAsync(HttpContext context)
        {
            await context.Response.WriteAsync("Hello ");

            await _next(context);

            await context.Response.WriteAsync(Message.Message);
        }
    }
}
