using EsSecrets.Day2.Services;
using Microsoft.AspNetCore.Mvc;

namespace EsSecrets.Day2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecretsController : ControllerBase
{
    private readonly ISecretService _secretService;

    public SecretsController(ISecretService secretService)
    {
        _secretService = secretService;
    }

    /// <summary>
    /// 단건 조회 — no-tracking + projection.
    /// ChangeTracker에 올라가지 않으므로 읽기 전용 조회 비용이 낮다.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SecretResponse>> GetById(int id)
    {
        SecretResponse? secret = await _secretService.GetByIdAsync(id);
        return secret is null ? NotFound() : Ok(secret);
    }

    /// <summary>
    /// 목록 조회 — IQueryable 조합 + 조건부 tracked.
    /// tracked=false(기본값)이면 AsNoTracking, tracked=true이면 tracking query로 실행된다.
    /// 콘솔 로그에서 SQL 차이를 확인할 수 있다.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SecretListItemResponse>>> Search(
        [FromQuery] string? keyword,
        [FromQuery] bool tracked = false)
    {
        IReadOnlyList<SecretListItemResponse> items = await _secretService.SearchAsync(keyword, tracked);
        return Ok(items);
    }

    /// <summary>
    /// 제목 수정 — tracking query + ChangeTracker.DebugView 로그 + SaveChangesAsync.
    /// 콘솔에서 DebugView LongView와 UPDATE SQL이 순서대로 출력된다.
    /// Procedure 중심의 명시적 UPDATE SQL과 비교해서 보면 관점 차이가 잘 드러난다.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTitle(int id, [FromBody] UpdateSecretTitleRequest request)
    {
        bool updated = await _secretService.UpdateTitleAsync(id, request);
        return updated ? NoContent() : NotFound();
    }
}
