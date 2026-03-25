namespace EsSecrets.Day2.Middleware;

/// <summary>
/// Day 1에서 이어받은 custom middleware.
/// InvokeAsync에서 scoped 의존성을 주입받는 패턴을 유지한다.
/// Day 1 middleware 설명과 Day 2 DbContext(scoped) 설명의 연결 고리다.
/// </summary>
public sealed class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestAuditMiddleware> _logger;

    public RequestAuditMiddleware(RequestDelegate next, ILogger<RequestAuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation(
            "[Audit] {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);
    }
}
