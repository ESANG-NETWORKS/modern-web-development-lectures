using EsSecrets.Day2.Models;
using Microsoft.EntityFrameworkCore;

namespace EsSecrets.Day2.Data;

/// <summary>
/// DbContext는 단순 DB 연결 객체가 아니다.
///
/// Procedure 중심: Connection + Command를 열고, SQL을 직접 실행하고, ResultSet을 직접 매핑했다.
/// Entity 기반: DbContext 하나가 연결 + 명령 + 상태 추적 + 트랜잭션 경계를 모두 담는다.
///
/// ChangeTracker가 조회된 엔티티의 상태(Added/Modified/Deleted/Unchanged)를 추적하고,
/// SaveChangesAsync가 변경된 상태를 읽어 필요한 SQL만 생성한다.
/// </summary>
public sealed class SecretsDbContext : DbContext
{
    public SecretsDbContext(DbContextOptions<SecretsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Secret> Secrets => Set<Secret>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Secret>(entity =>
        {
            entity.HasKey(secret => secret.SecretId);
            entity.Property(secret => secret.Title).HasMaxLength(100);
            entity.Property(secret => secret.OwnerName).HasMaxLength(50);
        });

        modelBuilder.Entity<AccessLog>(entity =>
        {
            entity.HasKey(accessLog => accessLog.AccessLogId);
            entity.Property(accessLog => accessLog.ActionType).HasMaxLength(30);
            entity.Property(accessLog => accessLog.ActorUserName).HasMaxLength(50);

            entity.HasOne(accessLog => accessLog.Secret)
                .WithMany(secret => secret.AccessLogs)
                .HasForeignKey(accessLog => accessLog.SecretId);
        });
    }
}
