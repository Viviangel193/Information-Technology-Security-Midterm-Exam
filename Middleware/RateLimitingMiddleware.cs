using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;


namespace SampleSecureWeb.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private const int MaxAttempts = 5; // Maksimal 5 percobaan login
        private readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(1); // Blokir selama 1 menit


        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            // Middleware hanya mengecek endpoint Login dengan method POST
            if (context.Request.Path.StartsWithSegments("/Account/Login") && context.Request.Method == "POST")
            {
                // Memastikan RemoteIpAddress tidak null, jika null gunakan "unknown"
                var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var cacheKey = $"LoginAttempts_{remoteIp}";


                // Mengecek jumlah percobaan login dari cache
                if (_cache.TryGetValue(cacheKey, out int attempts))
                {
                    if (attempts >= MaxAttempts)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsync("Terlalu banyak percobaan login. Silakan coba lagi dalam 1 menit.");
                        return;
                    }
                }


                // Memproses permintaan berikutnya
                await _next(context);


                // Update jumlah percobaan login setelah permintaan login selesai diproses
                _cache.TryGetValue(cacheKey, out attempts);
                _cache.Set(cacheKey, ++attempts, BlockDuration);
            }
            else
            {
                await _next(context); // Lanjutkan ke middleware berikutnya jika bukan login
            }
        }
    }
}


