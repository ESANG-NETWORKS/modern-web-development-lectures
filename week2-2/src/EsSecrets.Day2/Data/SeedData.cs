using EsSecrets.Day2.Models;
using Microsoft.EntityFrameworkCore;

namespace EsSecrets.Day2.Data;

/// <summary>
/// EnsureCreatedAsync 기반 초기화 — migrations 미사용.
/// Day 2는 런타임 메커니즘 이해가 우선이므로 스키마 관리보다 동작 확인에 집중한다.
/// seed 데이터가 있어야 조회, grouping, SQL 확인 시나리오가 자연스럽게 이어진다.
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(SecretsDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Secrets.AnyAsync())
        {
            return;
        }

        var secret1 = new Secret
        {
            Title = "ERP Admin",
            OwnerName = "Infra Team",
            IsDeleted = false,
            LastModifiedAt = DateTime.UtcNow.AddDays(-2)
        };

        var secret2 = new Secret
        {
            Title = "Jenkins Deploy",
            OwnerName = "Platform Team",
            IsDeleted = false,
            LastModifiedAt = DateTime.UtcNow.AddDays(-1)
        };

        dbContext.Secrets.AddRange(secret1, secret2);
        await dbContext.SaveChangesAsync();

        dbContext.AccessLogs.AddRange(
            new AccessLog
            {
                SecretId = secret1.SecretId,
                ActionType = "DETAIL_VIEW",
                ActorUserName = "hong",
                CreatedAt = DateTime.UtcNow.AddHours(-4)
            },
            new AccessLog
            {
                SecretId = secret1.SecretId,
                ActionType = "DETAIL_VIEW",
                ActorUserName = "kim",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new AccessLog
            {
                SecretId = secret2.SecretId,
                ActionType = "DETAIL_VIEW",
                ActorUserName = "hong",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            });

        await dbContext.SaveChangesAsync();
    }
}
