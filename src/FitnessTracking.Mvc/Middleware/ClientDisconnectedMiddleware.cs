using Microsoft.AspNetCore.Http;

namespace FitnessTracking.Mvc.Middleware;

/// <summary>
/// İstemci (tarayıcı) bağlantıyı henüz yanıt üretilmeden kapattığında
/// HttpClient/EF gibi alt katmanlardan fırlayan <see cref="OperationCanceledException"/>
/// (ve onu sarmalayan <see cref="TaskCanceledException"/>) tiplerini sessizce yutar.
///
/// Bu durum gerçek bir hata değildir; loglarda exception olarak görünmesi
/// gürültü yaratır ve telemetriyi yanıltır. 499 benzeri (Client Closed Request)
/// statüsü ile yanıtı sonlandırırız — istemci zaten gitmiş olduğundan gövdeye yazılmaz.
/// </summary>
public sealed class ClientDisconnectedMiddleware(RequestDelegate next, ILogger<ClientDisconnectedMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug("Request {Path} was aborted by the client.", context.Request.Path);

            if (!context.Response.HasStarted)
            {
                // 499 Client Closed Request (Nginx convention). Standart bir HTTP kodu değildir
                // ancak istemci-iptali durumunu temiz bir şekilde gösterir.
                context.Response.Clear();
                context.Response.StatusCode = 499;
            }
        }
    }
}

public static class ClientDisconnectedMiddlewareExtensions
{
    public static IApplicationBuilder UseClientDisconnectedHandler(this IApplicationBuilder app)
        => app.UseMiddleware<ClientDisconnectedMiddleware>();
}
