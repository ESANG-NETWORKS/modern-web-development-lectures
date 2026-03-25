namespace EsSecrets.Day2.Models;

/// <summary>
/// DB 엔티티 — Secret과의 관계를 Navigation property로 표현한다.
/// Procedure 중심에서는 JOIN 쿼리와 FK 컬럼 직접 참조로 표현하던 관계를
/// Entity 기반에서는 이 navigation과 Include로 다룬다.
/// </summary>
public sealed class AccessLog
{
    public int AccessLogId { get; set; }
    public int SecretId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string ActorUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Secret Secret { get; set; } = null!;
}
