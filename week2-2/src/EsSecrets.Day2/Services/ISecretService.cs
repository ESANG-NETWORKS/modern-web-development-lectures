namespace EsSecrets.Day2.Services;

public interface ISecretService
{
    Task<SecretResponse?> GetByIdAsync(int id);
    Task<IReadOnlyList<SecretListItemResponse>> SearchAsync(string? keyword, bool tracked);
    string GetSearchSql(string? keyword);
    Task<bool> UpdateTitleAsync(int id, UpdateSecretTitleRequest request);
}

/// <summary>
/// API 계층 DTO — DbContext가 추적하는 Secret 엔티티와 분리한다.
/// Procedure 중심의 ResultSet row → 객체 변환 코드 대신,
/// EF Core에서는 projection Select가 이 변환 책임을 담당한다.
/// </summary>
public sealed class SecretResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
}

public sealed class SecretListItemResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public int AccessLogCount { get; set; }
}

public sealed class UpdateSecretTitleRequest
{
    public string Title { get; set; } = string.Empty;
}
