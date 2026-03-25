# Day 2 CLI 실습 시나리오

본 문서는 Day 2 강의를 듣는 사람이 Day 1에서 만든 ASP.NET Core Web API 프로젝트를 이어서 Entity Framework Core 기반 프로젝트로 확장하고, `DbContext`, `ChangeTracker`, `AsNoTracking`, `IQueryable`, `ToQueryString`, 로깅/디버깅까지 순서대로 따라갈 수 있도록 만든 실습 시나리오다.

## 이 시나리오의 목표

- Day 1의 in-memory 기반 `SecretService`를 EF Core 기반 구현으로 교체한다.
- SQLite를 사용해 실제 SQL이 만들어지고 실행되는 흐름을 확인한다.
- `DbContext`와 `ChangeTracker`가 어떤 역할을 하는지 직접 본다.
- `AsNoTracking`, projection, `IQueryable` 조합을 실제 코드로 비교한다.
- `ToQueryString()`과 EF Core 로깅을 통해 LINQ 뒤의 SQL을 확인한다.
- Swagger UI에서 조회/수정 endpoint를 호출하고, 콘솔 로그와 DebugView를 함께 확인한다.

## 최종 도착 상태

- 프로젝트가 SQLite 기반 `DbContext`를 사용한다.
- `GET /api/secrets/{id}`와 `GET /api/secrets`가 EF Core 조회를 사용한다.
- `PUT /api/secrets/{id}` 호출 시 tracking 상태 변화와 `SaveChangesAsync()`를 확인할 수 있다.
- `GET /api/diagnostics/secrets-sql`로 생성될 SQL을 확인할 수 있다.
- 콘솔에서 EF Core SQL 로그를 볼 수 있다.
- Swagger UI에서 조회/수정/SQL 확인 endpoint를 직접 테스트할 수 있다.

## 권장 진행 시간

- 총 60분
- 0~10분: 패키지 추가와 SQLite 설정
- 10~20분: Entity와 `DbContext` 작성
- 20~35분: EF Core 기반 service로 교체
- 35~45분: 복합 조회와 `ToQueryString()` 추가
- 45~55분: update와 `ChangeTracker.DebugView` 확인
- 55~60분: Swagger UI 호출과 정리

## 이 시나리오가 이어받는 출발점

- Day 1 문서: [WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md](WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md)
- 같은 프로젝트 `EsSecrets.Day1`를 계속 사용한다.
- 이미 controller, service, middleware, OpenAPI, Swagger UI가 준비되어 있다고 가정한다.

## 강의 중 같이 띄울 Microsoft Learn 시각 자료

- Change Tracker Debugging
  - `ChangeTracker.DebugView` 스크린샷 설명용
  - [Change Tracker Debugging](https://learn.microsoft.com/ef/core/change-tracking/debug-views#change-tracker-debug-view)
- Tracking vs. No-Tracking Queries
  - tracking, no-tracking, identity resolution 비교 설명용
  - [Tracking vs. No-Tracking Queries](https://learn.microsoft.com/ef/core/querying/tracking#identity-resolution)
- Simple Logging
  - `LogTo(...)`로 SQL과 이벤트를 보는 흐름 설명용
  - [Simple Logging](https://learn.microsoft.com/ef/core/logging-events-diagnostics/simple-logging)
- ToQueryString API 문서
  - LINQ 뒤의 실제 SQL을 확인하는 디버깅 흐름 설명용
  - [ToQueryString](https://learn.microsoft.com/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.toquerystring?view=efcore-10.0)

## 사전 준비

- Day 1 프로젝트 폴더로 이동

```powershell
cd EsSecrets.Day1
```

- 프로젝트가 정상 실행되는지 확인

```powershell
dotnet run
```

## 1) EF Core SQLite 패키지 추가

### 1단계 실행 명령

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### 1단계 설명 포인트

- Day 2는 실제 SQL을 보고 싶기 때문에 InMemory provider 대신 SQLite provider를 쓴다.
- SQLite는 로컬 파일 하나로 돌릴 수 있어서 교육용으로 단순하다.
- 이 선택 덕분에 `ToQueryString()`과 SQL 로그가 직관적으로 연결된다.

## 2) Entity 추가

### 2단계 새 파일 생성

- `Models/Secret.cs`
- `Models/AccessLog.cs`

### `Models/Secret.cs`

```csharp
namespace EsSecrets.Day1.Models;

public sealed class Secret
{
    public int SecretId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime LastModifiedAt { get; set; }

    public ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
}
```

### `Models/AccessLog.cs`

```csharp
namespace EsSecrets.Day1.Models;

public sealed class AccessLog
{
    public int AccessLogId { get; set; }
    public int SecretId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string ActorUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Secret Secret { get; set; } = null!;
}
```

### 2단계 설명 포인트

- Day 1의 `SecretResponse`는 API 계약 모델이었고, Day 2의 `Secret`은 DB 엔티티다.
- EF Core 설명에서는 entity와 DTO를 다시 분리해서 보여주는 편이 좋다.
- `AccessLog`를 추가하는 이유는 join/navigation/grouping 예시를 만들기 위해서다.

## 3) `DbContext` 추가

### 3단계 새 파일 생성

- `Data/SecretsDbContext.cs`

### 3단계 같이 띄울 Microsoft Learn 참고 자료

- Change Tracker Debugging
- 링크: [Change Tracker Debugging](https://learn.microsoft.com/ef/core/change-tracking/debug-views#change-tracker-debug-view)

### 코드

```csharp
using EsSecrets.Day1.Models;
using Microsoft.EntityFrameworkCore;

namespace EsSecrets.Day1.Data;

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
```

### 3단계 설명 포인트

- `DbContext`는 단순 연결 객체가 아니라 작업 단위이자 상태 추적 루트다.
- `DbSet<T>`는 테이블 그 자체보다, query root로 보는 편이 더 정확하다.
- `OnModelCreating`은 엔티티 구조를 런타임 모델로 설명하는 지점이다.

## 4) Seed 데이터 추가

### 4단계 새 파일 생성

- `Data/SeedData.cs`

### 4단계 코드

```csharp
using EsSecrets.Day1.Models;

namespace EsSecrets.Day1.Data;

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
```

### 4단계 설명 포인트

- Day 2는 migrations보다 런타임 메커니즘 이해가 우선이라 `EnsureCreated`로 간단히 시작한다.
- seed 데이터가 있어야 조회, grouping, SQL 확인 시나리오가 자연스럽다.

## 5) `Program.cs`에 SQLite와 로깅 등록

### 5단계 같이 띄울 Microsoft Learn 참고 자료

- Simple Logging
- 링크: [Simple Logging](https://learn.microsoft.com/ef/core/logging-events-diagnostics/simple-logging)

### 수정 코드

```csharp
using EsSecrets.Day1.Data;
using EsSecrets.Day1.Middleware;
using EsSecrets.Day1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddSingleton<IAppMetadataProvider, AppMetadataProvider>();
builder.Services.AddScoped<ISecretService, SecretService>();

builder.Services.AddDbContext<SecretsDbContext>(options =>
{
    options.UseSqlite("Data Source=essecrets-day2.db");
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SecretsDbContext>();
    await SeedData.InitializeAsync(dbContext);
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestAuditMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 5단계 설명 포인트

- `AddDbContext`는 기본적으로 scoped lifetime을 사용한다.
- 여기서 console logging을 켜면 SQL과 command 실행이 눈에 보이기 시작한다.
- Day 1의 DI/lifetime 설명이 Day 2에서 `DbContext`와 자연스럽게 연결된다는 점을 강조한다.

## 6) Service를 EF Core 기반으로 교체

### 6단계 수정 파일

- `Services/ISecretService.cs`
- `Services/SecretService.cs`

### `Services/ISecretService.cs`

```csharp
namespace EsSecrets.Day1.Services;

public interface ISecretService
{
    Task<SecretResponse?> GetByIdAsync(int id);
    Task<IReadOnlyList<SecretListItemResponse>> SearchAsync(string? keyword, bool tracked);
    string GetSearchSql(string? keyword);
    Task<bool> UpdateTitleAsync(int id, UpdateSecretTitleRequest request);
}

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
```

### `Services/SecretService.cs`

```csharp
using EsSecrets.Day1.Data;
using EsSecrets.Day1.Models;
using Microsoft.EntityFrameworkCore;

namespace EsSecrets.Day1.Services;

public sealed class SecretService : ISecretService
{
    private readonly SecretsDbContext _dbContext;
    private readonly ILogger<SecretService> _logger;

    public SecretService(SecretsDbContext dbContext, ILogger<SecretService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

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
        _logger.LogInformation("ChangeTracker LongView:{NewLine}{DebugView}", Environment.NewLine, _dbContext.ChangeTracker.DebugView.LongView);

        await _dbContext.SaveChangesAsync();
        return true;
    }
}
```

### 6단계 설명 포인트

- 이제 service는 단순 리스트 조회가 아니라 LINQ를 조합해 SQL로 번역시키는 역할을 한다.
- `GetByIdAsync`는 no-tracking + projection의 전형적인 조회 예시다.
- `SearchAsync`는 `IQueryable` 조합 예시다.
- `UpdateTitleAsync`는 tracking 상태와 `SaveChangesAsync`를 보여주기 위한 예시다.

## 7) Controller를 Day 2 시나리오에 맞게 확장

### 7단계 수정 파일

- `Controllers/SecretsController.cs`

### 7단계 코드

```csharp
using EsSecrets.Day1.Services;
using Microsoft.AspNetCore.Mvc;

namespace EsSecrets.Day1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecretsController : ControllerBase
{
    private readonly ISecretService _secretService;

    public SecretsController(ISecretService secretService)
    {
        _secretService = secretService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SecretResponse>> GetById(int id)
    {
        SecretResponse? secret = await _secretService.GetByIdAsync(id);
        return secret is null ? NotFound() : Ok(secret);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SecretListItemResponse>>> Search([FromQuery] string? keyword, [FromQuery] bool tracked = false)
    {
        IReadOnlyList<SecretListItemResponse> items = await _secretService.SearchAsync(keyword, tracked);
        return Ok(items);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTitle(int id, [FromBody] UpdateSecretTitleRequest request)
    {
        bool updated = await _secretService.UpdateTitleAsync(id, request);
        return updated ? NoContent() : NotFound();
    }
}
```

### 7단계 새 파일 생성

- `Controllers/DiagnosticsController.cs`

### `Controllers/DiagnosticsController.cs`

```csharp
using EsSecrets.Day1.Services;
using Microsoft.AspNetCore.Mvc;

namespace EsSecrets.Day1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly ISecretService _secretService;

    public DiagnosticsController(ISecretService secretService)
    {
        _secretService = secretService;
    }

    [HttpGet("secrets-sql")]
    public ActionResult<string> GetSecretsSql([FromQuery] string? keyword)
    {
        return Ok(_secretService.GetSearchSql(keyword));
    }
}
```

### 7단계 설명 포인트

- Day 2는 endpoint 수를 늘리는 게 목적이 아니라, 다른 종류의 EF Core 동작을 보기 위한 진입점을 마련하는 게 목적이다.
- `DiagnosticsController`는 교육용으로 SQL을 눈으로 보기 위한 endpoint다.

## 8) 실행 및 데이터베이스 생성 확인

### 8단계 실행 명령

```powershell
dotnet run
```

### 8단계 설명 포인트

- 첫 실행에서 `essecrets-day2.db` 파일이 만들어진다.
- 콘솔에 EF Core SQL 로그가 출력되기 시작한다.
- seed 데이터가 들어갔는지 Swagger UI에서 바로 검증할 수 있다.

## 9) Swagger UI에서 조회 시나리오 테스트

### 확인 URL

- Swagger UI
  - `https://localhost:<port>/swagger`
- OpenAPI JSON
  - `https://localhost:<port>/openapi/v1.json`

### 테스트 순서 A: 단건 조회

1. `GET /api/secrets/{id}`를 연다.
2. `id`에 `1`을 넣고 실행한다.
3. 응답 JSON을 본다.
4. 콘솔에서 SQL 로그가 출력되는지 본다.

### 테스트 순서 B: 목록 조회 + no-tracking

1. `GET /api/secrets`를 연다.
2. `tracked=false`로 실행한다.
3. `keyword=ERP`도 넣어 다시 실행한다.
4. projection과 filtering이 어떻게 동작하는지 설명한다.

### 테스트 순서 C: SQL 확인

1. `GET /api/diagnostics/secrets-sql`를 연다.
2. `keyword=ERP`로 실행한다.
3. 반환된 SQL 문자열을 읽는다.
4. `WHERE`, `ORDER BY`, projection이 예상과 맞는지 확인한다.

## 10) `AsNoTracking`과 `tracked=true` 비교 포인트

### 10단계 같이 띄울 Microsoft Learn 참고 자료

- Tracking vs. No-Tracking Queries
- 링크: [Tracking vs. No-Tracking Queries](https://learn.microsoft.com/ef/core/querying/tracking#identity-resolution)

### 강의 포인트

- 이 시나리오에서는 `tracked=false`가 기본값이다.
- 읽기 전용 조회에서는 보통 tracking이 필요 없다.
- 추후 필요하면 `tracked=true`를 넣고 query behavior 차이를 비교 설명할 수 있다.

## 11) 수정 시나리오와 ChangeTracker 확인

### 11단계 같이 띄울 Microsoft Learn 참고 자료

- Change Tracker Debugging
- 링크: [Change Tracker Debugging](https://learn.microsoft.com/ef/core/change-tracking/debug-views#change-tracker-debug-view)

### 테스트 순서

1. `PUT /api/secrets/{id}`를 연다.
2. `id=1`로 두고 body에 아래 JSON을 넣는다.

     ```json
     {
         "title": "ERP Admin Updated"
     }
     ```

3. 실행 후 `204 No Content`를 확인한다.
4. 콘솔 로그에서 `ChangeTracker.DebugView.LongView` 출력과 `UPDATE` SQL을 확인한다.
5. 다시 `GET /api/secrets/1`을 호출해 값이 바뀌었는지 확인한다.

### 여기서 설명할 것

- update는 tracking query가 필요하다.
- 엔티티를 수정하면 ChangeTracker가 상태를 `Modified`로 인식한다.
- `SaveChangesAsync()`는 이 상태를 보고 필요한 SQL만 만든다.

## 12) 복합 조회 예시를 어떻게 해석할 것인가

### 12단계 강의 포인트

- `SearchAsync`는 단순 조회이지만 이미 중요한 요소가 들어 있다.
  - `IQueryable` 조합
  - 조건부 `Where`
  - `OrderByDescending`
  - projection `Select`
  - navigation count
- 복잡한 LINQ를 볼 때는 아래 순서로 읽는다.
  - query root는 무엇인가
  - 어디서 filtering이 들어가는가
  - 어디서 shape가 바뀌는가
  - 최종 SQL은 어떻게 생겼을 것 같은가

## 13) Day 2 마무리 질문

- 왜 `DbContext`와 `ChangeTracker`를 같이 봐야 하는가
- 왜 조회에서는 `AsNoTracking`과 projection을 먼저 생각하는가
- 왜 `IQueryable`은 조합 중이고, `ToListAsync()`에서 실행된다고 말하는가
- 왜 복잡한 LINQ를 보면 SQL을 먼저 떠올려야 하는가
- 왜 성능 이슈를 볼 때 LINQ만 보지 말고 로그와 SQL을 같이 봐야 하는가

## 14) 강의 중 강조할 함정 포인트

- InMemory provider로 SQL 설명을 하려고 하지 않기
- 조회 endpoint에서 무조건 tracking query를 쓰지 않기
- entity 전체 반환과 projection을 섞어 설명하지 않기
- `IQueryable`과 `IEnumerable` 실행 시점을 혼동하지 않기
- `ToQueryString()` 결과가 디버깅용이라는 점을 놓치지 않기

## 15) Day 2 이후 연결 포인트

- Day 3에서는 이 `DbContext` 기반 구조 위에 실제 프로젝트 도메인과 API 명세를 붙인다.
- 이후 인증/권한/감사 로그를 붙여도, 기본 흐름은 Day 1의 DI와 Day 2의 EF Core 메커니즘 위에 올라간다.
- 즉, Day 1은 웹 런타임 이해, Day 2는 데이터 런타임 이해, 이후는 도메인 적용 단계로 이어진다.
