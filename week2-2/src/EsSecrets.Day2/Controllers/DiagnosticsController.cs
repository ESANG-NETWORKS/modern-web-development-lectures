using EsSecrets.Day2.Services;
using Microsoft.AspNetCore.Mvc;

namespace EsSecrets.Day2.Controllers;

/// <summary>
/// 교육용 진단 endpoint — LINQ 뒤의 SQL을 Swagger UI에서 직접 눈으로 확인하는 용도.
/// 프로덕션 코드에서는 ToQueryString()을 API 응답으로 노출하지 않는다.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly ISecretService _secretService;

    public DiagnosticsController(ISecretService secretService)
    {
        _secretService = secretService;
    }

    /// <summary>
    /// LINQ 쿼리가 어떤 SQL로 번역되는지 문자열로 반환한다.
    /// keyword 유무에 따라 WHERE 절이 달라지는 것을 직접 확인할 수 있다.
    /// </summary>
    [HttpGet("secrets-sql")]
    public ActionResult<string> GetSecretsSql([FromQuery] string? keyword)
    {
        return Ok(_secretService.GetSearchSql(keyword));
    }
}
