namespace EsSecrets.Day2.Models;

/// <summary>
/// DB 엔티티 — API 계층 DTO(SecretResponse)와 분리된 데이터 모델이다.
/// Procedure 중심에서는 ResultSet 행 하나가 곧 데이터였지만,
/// Entity 기반에서는 이 객체가 ChangeTracker의 추적 대상이 된다.
/// </summary>
public sealed class Secret
{
    public int SecretId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime LastModifiedAt { get; set; }

    public ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
}
