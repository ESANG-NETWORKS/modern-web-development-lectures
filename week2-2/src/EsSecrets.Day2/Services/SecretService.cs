using EsSecrets.Day2.Data;
using EsSecrets.Day2.Models;
using Microsoft.EntityFrameworkCore;

namespace EsSecrets.Day2.Services;

public sealed class SecretService : ISecretService
{
    private readonly SecretsDbContext _dbContext;
    private readonly ILogger<SecretService> _logger;

    public SecretService(SecretsDbContext dbContext, ILogger<SecretService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// no-tracking + projection 조합 — 읽기 전용 단건 조회의 기본 패턴.
    ///
    /// Procedure 중심: SELECT 후 ResultSet을 직접 DTO로 변환.
    /// Entity 기반: AsNoTracking으로 ChangeTracker 비용을 없애고,
    /// Select로 필요한 shape만 projection해서 materialization 범위를 제한한다.
    /// </summary>
    public async Task<SecretResponse?> GetByIdAsync(int id)
    {
        return await _dbContext.Secrets
            .AsNoTracking()
            .Where(secret => secret.SecretId == id && !secret.IsDeleted)
            .Select(secret => new SecretResponse
            {
                Id = secret.SecretId,
                Title = secret.Title,
                Owner = secret.OwnerName
            })
            .SingleOrDefaultAsync();
    }

    /// <summary>
    /// IQueryable 조합 — 조건을 단계적으로 추가하고 마지막에 실행한다.
    ///
    /// Procedure 중심: 조건마다 다른 SQL 또는 동적 SQL 문자열 조합.
    /// Entity 기반: IQueryable은 아직 실행되지 않은 쿼리다. Where/OrderBy/Select를
    /// 체이닝해도 ToListAsync 전까지는 DB 왕복이 없다. expression tree가 쌓이고 있을 뿐이다.
    /// </summary>
    public async Task<IReadOnlyList<SecretListItemResponse>> SearchAsync(string? keyword, bool tracked)
    {
        IQueryable<Secret> query = _dbContext.Secrets
            .Where(secret => !secret.IsDeleted);

        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(secret => secret.Title.Contains(keyword));
        }

        return await query
            .OrderByDescending(secret => secret.LastModifiedAt)
            .Select(secret => new SecretListItemResponse
            {
                Id = secret.SecretId,
                Title = secret.Title,
                Owner = secret.OwnerName,
                AccessLogCount = secret.AccessLogs.Count
            })
            .ToListAsync();
    }

    /// <summary>
    /// ToQueryString() — LINQ 뒤에 어떤 SQL이 만들어지는지 확인하는 디버깅 도구.
    ///
    /// Procedure 중심: SQL이 이미 눈앞에 있으므로 별도 확인이 필요 없다.
    /// Entity 기반: LINQ를 작성할 때는 항상 "이 SQL이 어떻게 나갈까"를 상상해야 하고,
    /// ToQueryString이 그 상상을 검증하는 수단이다.
    /// </summary>
    public string GetSearchSql(string? keyword)
    {
        IQueryable<Secret> query = _dbContext.Secrets
            .AsNoTracking()
            .Where(secret => !secret.IsDeleted);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(secret => secret.Title.Contains(keyword));
        }

        return query
            .OrderByDescending(secret => secret.LastModifiedAt)
            .Select(secret => new
            {
                secret.SecretId,
                secret.Title,
                secret.OwnerName
            })
            .ToQueryString();
    }

    /// <summary>
    /// tracking query + ChangeTracker + SaveChangesAsync — 수정의 기본 패턴.
    ///
    /// Procedure 중심: UPDATE secrets SET title = ? WHERE secret_id = ?
    ///   → 개발자가 어떤 SQL을 실행할지 직접 지정한다.
    /// Entity 기반: 객체 속성만 바꾸고 SaveChangesAsync를 호출한다.
    ///   → ChangeTracker가 Modified 상태를 감지하고 UPDATE SQL을 자동으로 만든다.
    ///   → 이 차이가 편리함과 동시에 "왜 update가 나갔는지 모르겠다"는 혼란의 원인이 된다.
    /// </summary>
    public async Task<bool> UpdateTitleAsync(int id, UpdateSecretTitleRequest request)
    {
        Secret? secret = await _dbContext.Secrets
            .SingleOrDefaultAsync(item => item.SecretId == id && !item.IsDeleted);

        if (secret is null)
        {
            return false;
        }

        secret.Title = request.Title;
        secret.LastModifiedAt = DateTime.UtcNow;

        _dbContext.ChangeTracker.DetectChanges();
        _logger.LogInformation(
            "ChangeTracker LongView:{NewLine}{DebugView}",
            Environment.NewLine,
            _dbContext.ChangeTracker.DebugView.LongView);

        await _dbContext.SaveChangesAsync();
        return true;
    }
}
