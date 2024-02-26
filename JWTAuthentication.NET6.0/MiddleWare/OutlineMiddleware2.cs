using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JWTAuthentication.NET6._0.MiddleWare
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class OutlineMiddleware2
    {
        private readonly RequestDelegate _next;

        public OutlineMiddleware2(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Thực hiện các hành động trước khi gửi yêu cầu tới API
            Debug.WriteLine("Before sending request to API 2");

            // Gọi middleware tiếp theo trong pipeline
            await _next(context);

            // Thực hiện các hành động sau khi nhận phản hồi từ API
            Debug.WriteLine("After receiving response from API 2");
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class OutlineMiddleware2Extensions
    {
        public static IApplicationBuilder UseOutlineMiddleware2(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OutlineMiddleware2>();
        }
    }
}
