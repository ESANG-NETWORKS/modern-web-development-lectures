# 2주차 백엔드 교육 발표 자료 초안

본 문서는 [2주차 백엔드 교육 슬라이드 아웃라인](WEEK2_BACKEND_API_SLIDE_OUTLINE.md)을 실제 발표용 문구와 예시 코드 중심으로 풀어쓴 초안이다.

## 사용 방법

- 본문은 PPT에 바로 옮길 수 있는 짧은 문장 위주로 작성한다.
- 코드는 "개념을 설명하기 위한 최소 예시"만 둔다.
- 설명은 항상 런타임 동작과 연결한다.

## Day 1 발표 초안

- CLI 실습 가이드는 [WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md](WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md)를 참고한다.

### 슬라이드 1. ASP.NET Core는 `Program.cs`에서 시작된다

- 본문 문구
  - ASP.NET Core 앱은 시작 파일에서 구조가 드러난다.
  - `CreateBuilder`는 설정, 로깅, DI 기반을 준비한다.
  - `Build`는 등록된 요소를 조립한다.
  - `Run`은 요청을 받을 준비를 마친다.
- 발표 멘트
  - "이 파일은 그냥 부트스트랩 코드가 아니라, 앱이 어떤 부품으로 조립되는지를 보여주는 설계도입니다."
- 예시 코드

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<ISecretService, SecretService>();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
```

### 슬라이드 2. DI와 서비스 등록

- 본문 문구
  - ASP.NET Core는 객체를 직접 만들기보다 등록하고 조립한다.
  - 의존성 주입은 편의 기능이 아니라 결합도 제어 장치다.
  - framework service와 application service가 같은 컨테이너에서 조합된다.
- 발표 멘트
  - "여기서 핵심은 new를 줄이는 게 아닙니다. 상위 코드가 하위 구현 세부사항에 묶이지 않게 만드는 겁니다."

### 슬라이드 3. Lifetime과 Scope

- 본문 문구
  - `Singleton`: 앱 전체에서 하나
  - `Scoped`: 요청 단위로 하나
  - `Transient`: 요청할 때마다 새 인스턴스
  - `DbContext`는 보통 요청 단위로 관리된다.
- 발표 멘트
  - "웹에서는 요청 하나가 논리적인 작업 단위이기 때문에 scoped가 가장 자연스럽습니다."
- 시각 자료 문구
  - Request A / Scoped instance A
  - Request B / Scoped instance B
  - App / Singleton instance 1

### 슬라이드 4. Middleware는 기능 추가 메커니즘이다

- 본문 문구
  - middleware는 요청 파이프라인의 한 조각이다.
  - 각 조각은 `HttpContext`를 보고 다음으로 넘기거나 끝낸다.
  - 기능 차이만큼 순서가 중요하다.
- 발표 멘트
  - "ASP.NET Core는 기능을 클래스 계층으로 늘리기보다, 파이프라인에 끼워 넣는 방식으로 확장합니다."
- 예시 코드

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Training", "Day1");
    await next();
});
```

### 슬라이드 5. Controller 코드와 C# 문법

- 본문 문구
  - attribute routing
  - model binding
  - `ActionResult<T>`
  - async/await
  - DI 기반 생성자 주입
- 발표 멘트
  - "controller가 짧아 보이는 이유는 개발자가 안 써도 되는 인프라를 프레임워크가 이미 pipeline 안에 넣어놨기 때문입니다."
- 예시 코드

```csharp
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
    public async Task<ActionResult<SecretResponse>> Get(int id)
    {
        SecretResponse? secret = await _secretService.GetAsync(id);
        return secret is null ? NotFound() : Ok(secret);
    }
}
```

### 슬라이드 6. Swagger/OpenAPI로 Day 1 마무리

- 본문 문구
  - Swagger도 ASP.NET Core 확장 메커니즘의 일부다.
  - OpenAPI 문서는 별도 마법이 아니라 서비스 등록과 endpoint 노출의 결과다.
  - middleware/endpoint 구조를 이해하면 부가 기능 확장이 쉬워진다.
- 발표 멘트
  - "Swagger를 설명하는 목적은 문서 도구 소개가 아니라, ASP.NET Core가 기능을 붙이는 방식을 눈에 보이게 마무리하기 위해서입니다."

## Day 2 발표 초안

- CLI 실습 가이드는 [WEEK2_DAY2_EFCORE_CLI_SCENARIO.md](WEEK2_DAY2_EFCORE_CLI_SCENARIO.md)를 참고한다.
- 예시 프로젝트: [EsSecrets.Day2](../../../../EsSecrets.Day2/)

### 슬라이드 0. Procedure 중심 vs Entity 기반 — 사고 방식의 차이

- 본문 문구
  - Procedure 중심: DB가 먼저다. 테이블과 SQL이 설계의 중심이고, 객체는 결과를 담는 그릇이다.
  - Entity 기반: 객체가 먼저다. 객체 상태 변화를 기술하면 ORM이 SQL을 만든다.
  - 이 차이를 모르면 DbContext와 ChangeTracker가 왜 필요한지 이해할 수 없다.
- 발표 멘트
  - "오늘 EF Core를 처음 보는 분들 중에 MyBatis나 직접 SQL을 써본 경험이 있다면, 지금 보여드릴 코드가 낯설게 느껴질 수 있습니다. 그 낯섦이 어디서 오는지부터 짚고 넘어가겠습니다."
- 도입 질문
  - "특정 레코드의 제목을 바꿔야 할 때, 여러분은 어떻게 했나요?"
- 코드 대비 (좌: Procedure 중심 / 우: Entity 기반)

```java
// Procedure 중심 (MyBatis)
// 개발자가 어떤 SQL을 실행할지 직접 지정한다
secretMapper.updateTitle(id, newTitle);
```

```xml
<!-- mapper XML -->
UPDATE secrets SET title = #{title} WHERE secret_id = #{id}
```

```csharp
// Entity 기반 (EF Core)
// 개발자는 객체 상태만 바꾼다. SQL 생성은 ChangeTracker가 담당한다
Secret secret = await context.Secrets.SingleAsync(x => x.SecretId == id);
secret.Title = newTitle;
await context.SaveChangesAsync();
```

- 용어 대조표 (슬라이드에 표로 삽입)

| Procedure 중심             | Entity 기반 (EF Core)            |
| -------------------------- | -------------------------------- |
| 행(Row), 결과셋(ResultSet) | 객체(Entity), 객체 그래프        |
| SQL Mapper / XML mapper    | LINQ → SQL 번역기                |
| Stored Procedure           | DbContext 메서드 + LINQ 조합     |
| 명시적 UPDATE SQL          | 객체 속성 변경 + `SaveChanges`   |
| Connection + Command       | DbContext (Unit of Work)         |
| ResultSet → DTO 변환 코드  | Projection `Select`              |
| FK 컬럼 직접 조회          | Navigation property (`Include`)  |
| 트랜잭션 BEGIN/COMMIT       | DbContext 수명 주기 + 원자성     |
| Row identity 없음          | Identity Map                     |

- 핵심 전환 메시지
  - "ChangeTracker, SaveChanges, AsNoTracking이 왜 필요한지는 전부 이 관점 차이에서 출발합니다."
  - "LINQ는 SQL을 잊기 위한 도구가 아닙니다. 지금부터 나오는 모든 코드에서 SQL을 상상하면서 보세요."
- 주의: 우열 비교로 흘러가지 않는다. 두 방식은 트레이드오프가 다를 뿐이다.

### 슬라이드 1. EF Core를 어떤 눈으로 볼 것인가

- 본문 문구
  - EF Core는 단순 쿼리 도우미가 아니다.
  - 상태 추적과 쿼리 번역이 핵심이다.
  - `DbContext`와 `ChangeTracker`를 같이 봐야 한다.
- 발표 멘트
  - "JPA의 EntityManager를 떠올리면 이해가 빠르지만, EF Core는 그 역할이 DbContext와 ChangeTracker에 나뉘어 있다고 보는 편이 더 정확합니다."

### 슬라이드 2. 엔티티 상태와 `SaveChanges`

- 본문 문구
  - 엔티티는 `Added`, `Modified`, `Deleted`, `Unchanged` 상태를 가진다.
  - `SaveChanges`는 상태를 읽고 SQL을 만든다.
  - 상태 추적이 편리함과 비용을 동시에 만든다.
- 발표 멘트
  - "변경 감지는 편하지만 공짜는 아닙니다. 그래서 tracking이 필요한지부터 판단해야 합니다."
- 예시 코드

```csharp
Secret secret = await context.Secrets.FirstAsync(x => x.SecretId == id);
secret.Title = "Updated";

context.ChangeTracker.DetectChanges();
Console.WriteLine(context.ChangeTracker.DebugView.LongView);

await context.SaveChangesAsync();
```

### 슬라이드 3. 성능 효율적인 조회 구문

- 본문 문구
  - 읽기 전용 조회는 `AsNoTracking`을 먼저 검토한다.
  - 필요한 필드만 `Select`로 투영한다.
  - `IQueryable`로 조건을 조합하고 마지막에 실행한다.
- 발표 멘트
  - "성능 최적화는 마법 메서드를 외우는 게 아니라, 지금 tracking이 왜 필요한지 스스로 묻는 데서 시작합니다."
- 예시 코드

```csharp
IQueryable<Secret> query = context.Secrets.AsNoTracking();

if (!string.IsNullOrWhiteSpace(keyword))
{
    query = query.Where(x => x.Title.Contains(keyword));
}

var items = await query
    .OrderByDescending(x => x.LastModifiedAt)
    .Select(x => new SecretSummaryResponse
    {
        SecretId = x.SecretId,
        Title = x.Title,
        OwnerName = x.OwnerName
    })
    .ToListAsync();
```

### 슬라이드 4. LINQ와 SQL 번역

- 본문 문구
  - LINQ to Objects와 LINQ to Entities는 다르다.
  - `IQueryable`은 아직 실행되지 않은 쿼리다.
  - `ToListAsync` 같은 실행 시점에 SQL이 만들어진다.
- 발표 멘트
  - "체이닝된 메서드가 그냥 함수 호출처럼 보여도, 실제로는 expression tree가 쌓이고 있는 겁니다."

### 슬라이드 5. 복잡한 쿼리 예시

- 본문 문구
  - 검색 조건 조합
  - 상태 필터
  - 정렬/페이징
  - 집계 또는 navigation 활용
- 발표 멘트
  - "복잡한 LINQ를 작성할수록 머릿속에 예상 SQL이 먼저 떠올라야 합니다. LINQ는 SQL을 잊기 위한 도구가 아닙니다."
- 예시 코드

```csharp
var query = context.AccessLogs
    .AsNoTracking()
    .Where(x => x.CreatedAt >= from && x.CreatedAt < to)
    .Where(x => actionType == null || x.ActionType == actionType)
    .GroupBy(x => new { x.ActorUserId, x.ActionType })
    .Select(group => new AccessLogSummaryResponse
    {
        ActorUserId = group.Key.ActorUserId,
        ActionType = group.Key.ActionType,
        Count = group.Count()
    })
    .OrderByDescending(x => x.Count);
```

### 슬라이드 6. 예상 SQL과 실제 SQL 비교

- 본문 문구
  - LINQ를 작성한 뒤 SQL을 확인한다.
  - SQL shape를 보고 과한 join, projection 누락, 정렬 비용을 본다.
  - `ToQueryString()`은 디버깅 도구다.
- 발표 멘트
  - "느린 쿼리를 만나면 LINQ를 더 쳐다보기보다 SQL이 어떻게 나갔는지 먼저 보십시오."
- 예시 코드

```csharp
string sql = query.ToQueryString();
Console.WriteLine(sql);
```

### 슬라이드 7. 로깅과 디버깅

- 본문 문구
  - `LogTo`로 SQL과 이벤트를 볼 수 있다.
  - ASP.NET Core logging과 EF Core logging은 연결된다.
  - `ChangeTracker.DebugView`로 상태를 눈으로 확인할 수 있다.
- 발표 멘트
  - "왜 update가 나갔는지, 왜 entity가 추적되는지 모를 때는 감으로 보지 말고 로그와 DebugView를 봐야 합니다."
- 예시 코드

```csharp
optionsBuilder
    .UseSqlServer(connectionString)
    .LogTo(Console.WriteLine);
```

## 발표 자료 제작 메모

- Day 1은 흐름도와 `Program.cs` 코드가 중심이다.
- Day 2는 entity state, query pipeline, SQL 출력 예시가 중심이다.
- 두 날 모두 "개념 -> 코드 -> 런타임 동작" 순서를 유지한다.
