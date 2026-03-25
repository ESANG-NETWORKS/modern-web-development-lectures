# Day 1 CLI 실습 시나리오

본 문서는 Day 1 강의를 듣는 사람이 ASP.NET Core Web API 프로젝트를 CLI로 직접 생성하고, `Program.cs`, DI, middleware, controller, OpenAPI, Swagger UI까지 순서대로 따라갈 수 있도록 만든 실습 시나리오다.

## 이 시나리오의 목표

- `dotnet new`로 controller 기반 Web API 프로젝트를 만든다.
- `Program.cs`를 읽으면서 ASP.NET Core 앱이 어떻게 조립되는지 설명한다.
- service 등록과 DI를 직접 추가한다.
- custom middleware를 하나 추가한다.
- controller endpoint를 하나 만든다.
- OpenAPI JSON과 Swagger UI를 둘 다 띄운다.
- 마지막에 Swagger UI에서 직접 호출해 본다.

## 최종 도착 상태

- `dotnet run`으로 API 서버가 실행된다.
- `Program.cs` 안에서 service 등록, middleware, controller mapping, OpenAPI 구성을 읽을 수 있다.
- `GET /api/secrets/{id}` endpoint가 동작한다.
- `/openapi/v1.json`으로 OpenAPI 문서를 볼 수 있다.
- Swagger UI 화면에서 해당 endpoint를 테스트할 수 있다.

## 권장 진행 시간

- 총 60분
- 0~10분: 프로젝트 생성과 기본 구조 확인
- 10~20분: `Program.cs`와 DI 읽기
- 20~35분: service와 controller 추가
- 35~45분: custom middleware 추가
- 45~60분: OpenAPI + Swagger UI 구성 및 테스트

## 강의 중 같이 띄울 Microsoft Learn 시각 자료

- ASP.NET Core fundamentals overview
  - `Program.cs`가 services 구성과 request pipeline 정의의 중심이라는 점 설명용
  - [ASP.NET Core fundamentals overview](https://learn.microsoft.com/aspnet/core/fundamentals/?view=aspnetcore-10.0)
- ASP.NET Core Middleware
  - request delegate pipeline 그림 설명용
  - [ASP.NET Core Middleware](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0#create-a-middleware-pipeline-with-%60webapplication%60)
- Dependency injection in ASP.NET Core
  - lifetime 비교와 middleware에서 scoped service를 다루는 방식 설명용
  - [Dependency injection in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0#lifetime-and-registration-options)
- Write custom ASP.NET Core middleware
  - constructor 주입과 `InvokeAsync` 주입 차이 설명용
  - [Write custom ASP.NET Core middleware](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/write?view=aspnetcore-10.0#middleware-dependencies)

## 사전 준비

- .NET SDK 설치 확인

```powershell
dotnet --version
```

- 개발용 HTTPS 인증서가 없으면 아래 명령으로 준비

```powershell
dotnet dev-certs https --trust
```

## 1) 프로젝트 생성

### 1단계 실행 명령

```powershell
dotnet new webapi --use-controllers -n EsSecrets.Day1
cd EsSecrets.Day1
```

### 1단계 설명 포인트

- `webapi` 템플릿은 ASP.NET Core Web API용 기본 골격을 만든다.
- `--use-controllers`를 주면 Minimal API가 아니라 controller 기반 프로젝트로 시작한다.
- 이 시점의 핵심 파일은 `Program.cs`, `Controllers`, `.csproj`, `appsettings.json`이다.

### 바로 확인할 것

```powershell
dotnet run
```

- 실행 후 콘솔에 출력되는 `https://localhost:xxxx` 주소를 확인한다.
- 기본 템플릿은 `WeatherForecast` 예제를 제공할 수 있다.

## 2) Swagger UI 패키지 추가

기본 OpenAPI JSON만으로도 문서는 볼 수 있지만, Day 1 마무리는 Swagger UI에서 호출하는 데 두므로 UI 패키지를 하나 추가한다.

### 2단계 실행 명령

```powershell
dotnet add package NSwag.AspNetCore
```

### 2단계 설명 포인트

- OpenAPI 문서 생성과 UI 제공은 분리해서 볼 수 있다.
- `AddOpenApi`와 `MapOpenApi`는 문서 생성/노출 축이다.
- Swagger UI는 그 문서를 사람이 테스트하기 쉽게 렌더링하는 도구다.

## 3) `Program.cs`를 첫 번째 강의 포인트로 읽기

### 3단계 같이 띄울 Microsoft Learn 참고 자료

- ASP.NET Core fundamentals overview
- 링크: [ASP.NET Core fundamentals overview](https://learn.microsoft.com/aspnet/core/fundamentals/?view=aspnetcore-10.0)

### 현재 코드에서 설명할 항목

- `WebApplication.CreateBuilder(args)`
  - 설정, 로깅, DI 컨테이너, 호스팅 준비
- `builder.Services`
  - 애플리케이션 부품 등록 지점
- `builder.Build()`
  - 등록된 구성 요소를 앱으로 조립
- `app.Run()`
  - 실제 요청 수신 시작

### 이 단계에서 전달할 메시지

- ASP.NET Core는 객체를 직접 만들기보다 등록하고 조립하는 모델이다.
- `Program.cs`는 시작 파일이 아니라 앱 조립 설계도다.

## 4) 예제 서비스 추가

### 4단계 새 파일 생성

- `Services/ISecretService.cs`
- `Services/SecretService.cs`

### `Services/ISecretService.cs`

```csharp
namespace EsSecrets.Day1.Services;

public interface ISecretService
{
    SecretResponse? GetById(int id);
}

public sealed class SecretResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
}
```

### `Services/SecretService.cs`

```csharp
namespace EsSecrets.Day1.Services;

public sealed class SecretService : ISecretService
{
    private static readonly List<SecretResponse> Secrets =
    [
        new SecretResponse { Id = 1, Title = "ERP Admin", Owner = "Infra Team" },
        new SecretResponse { Id = 2, Title = "Jenkins Deploy", Owner = "Platform Team" }
    ];

    public SecretResponse? GetById(int id)
    {
        return Secrets.SingleOrDefault(secret => secret.Id == id);
    }
}
```

### 4단계 설명 포인트

- interface와 implementation을 나누는 이유
- controller가 concrete type이 아니라 interface에 의존하는 이유
- 이 구조가 DI와 의존성 역전에 어떻게 연결되는지

## 5) `Program.cs`에 service 등록 추가

### 5단계 같이 띄울 Microsoft Learn 참고 자료

- Dependency injection in ASP.NET Core
- 링크: [Dependency injection in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0#lifetime-and-registration-options)

### 수정 코드

```csharp
using EsSecrets.Day1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<ISecretService, SecretService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 5단계 설명 포인트

- `AddScoped<ISecretService, SecretService>()`의 의미
- 왜 요청 단위 scope가 web request와 잘 맞는가
- 왜 controller 안에서 `new SecretService()`를 하지 않는가

### 강의 포인트

- `Singleton`, `Scoped`, `Transient` 차이를 짧게 비교한다.
- middleware constructor에서 scoped service를 직접 받으면 왜 문제가 될 수 있는지 예고한다.

### `Scoped`를 직관적으로 설명하는 비교 예시

- `Scoped`는 "요청 하나를 처리하는 동안 함께 살아야 하는 상태"에 잘 맞는다.
- 인증 문맥에서는 같은 요청 안에서 `HttpContext.User`를 기준으로 파생되는 값이 대표적인 `Scoped` 후보다.
- 반대로 `Singleton`은 사용자나 요청과 무관하게 앱 전체에서 하나면 되는 값에 잘 맞는다.

### 인증 문맥의 `Scoped` 예시

```csharp
public interface ICurrentUserContext
{
    string? UserName { get; }
    bool IsAuthenticated { get; }
}

public sealed class CurrentUserContext : ICurrentUserContext
{
    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;
        UserName = user?.Identity?.Name;
        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;
    }

    public string? UserName { get; }
    public bool IsAuthenticated { get; }
}
```

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
```

- 설명 포인트
  - 로그인 사용자는 요청마다 달라질 수 있다.
  - 같은 요청 안에서는 controller, service, logger enrichment 같은 여러 지점에서 같은 사용자 문맥을 공유하고 싶다.
  - 그래서 `CurrentUserContext` 같은 값은 요청 단위로 한 번 만들어지는 `Scoped`가 자연스럽다.
  - 요청 A의 사용자와 요청 B의 사용자가 섞이면 안 되므로 `Singleton`이면 위험하다.

### 요청과 무관한 `Singleton` 예시

```csharp
public interface IAppMetadataProvider
{
    string ApplicationName { get; }
    DateTime StartedAtUtc { get; }
}

public sealed class AppMetadataProvider : IAppMetadataProvider
{
    public string ApplicationName { get; } = "EsSecrets.Day1";
    public DateTime StartedAtUtc { get; } = DateTime.UtcNow;
}
```

```csharp
builder.Services.AddSingleton<IAppMetadataProvider, AppMetadataProvider>();
```

- 설명 포인트
  - 앱 이름, 시작 시각, 빌드 정보처럼 요청과 무관한 값은 매 요청마다 새로 만들 필요가 없다.
  - 이런 값은 모든 요청이 함께 써도 문제가 없으므로 `Singleton`이 자연스럽다.

### 둘을 비교하며 강조할 메시지

- `Scoped`
  - 요청별로 달라지는 정보
  - 인증 사용자, 요청 추적 ID, Unit of Work, `DbContext`
- `Singleton`
  - 요청과 무관한 공용 정보
  - 앱 메타데이터, 고정 설정 해석 결과, 순수 계산 서비스
- 강의 한 줄 정리
  - "사용자와 요청에 붙어 다녀야 하면 `Scoped`, 앱 전체에서 하나여도 안전하면 `Singleton`으로 생각하면 된다."

## 6) Controller 추가

### 6단계 새 파일 생성

- `Controllers/SecretsController.cs`

### 6단계 코드

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
    public ActionResult<SecretResponse> GetById(int id)
    {
        SecretResponse? secret = _secretService.GetById(id);

        if (secret is null)
        {
            return NotFound();
        }

        return Ok(secret);
    }
}
```

### 6단계 설명 포인트

- `[ApiController]`가 주는 의미
- `[Route("api/[controller]")]`가 route를 어떻게 만드는가
- `ActionResult<T>`가 왜 유용한가
- 생성자 주입이 controller를 어떻게 간결하게 만드는가
- model binding, serialization, HTTP response 생성이 프레임워크 규약으로 어떻게 처리되는가

## 7) Custom middleware 추가

### 7단계 같이 띄울 Microsoft Learn 참고 자료

- ASP.NET Core Middleware
- 링크: [ASP.NET Core Middleware](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0#create-a-middleware-pipeline-with-%60webapplication%60)
- Write custom ASP.NET Core middleware
- 링크: [Write custom ASP.NET Core middleware](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/write?view=aspnetcore-10.0#middleware-dependencies)

### 7단계 새 파일 생성

- `Middleware/RequestAuditMiddleware.cs`

### 7단계 코드

```csharp
namespace EsSecrets.Day1.Middleware;

public sealed class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestAuditMiddleware> _logger;

    public RequestAuditMiddleware(RequestDelegate next, ILogger<RequestAuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Request started: {Method} {Path}", context.Request.Method, context.Request.Path);

        context.Response.Headers.Append("X-Training-Day", "Day1");

        await _next(context);
    }
}
```

### `Program.cs`에 등록 추가

```csharp
using EsSecrets.Day1.Middleware;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestAuditMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 7단계 설명 포인트

- middleware는 `HttpContext`를 받고 다음 단계로 넘기는 파이프라인 조각이라는 점
- 왜 순서가 중요한지
- logging, auth, exception handling, Swagger UI도 같은 확장 모델로 붙는다는 점

### middleware 설명에서 꼭 강조할 메시지

- middleware는 "모든 요청을 지나가게 하는 공용 파이프라인 조각"이다.
- 따라서 middleware 자체가 특정 요청의 사용자 상태를 내부 필드에 붙잡고 있으면 안 된다.
- 요청마다 달라지는 정보는 `HttpContext` 또는 `InvokeAsync` 매개변수 주입을 통해 그때그때 받아야 한다.
- 이 점 때문에 middleware 설명은 단순히 "순서가 중요하다"에서 끝나지 않고, "각 요청을 독립적으로 흘려보내야 한다"까지 가야 한다.

### 왜 생성자에 `Scoped`를 바로 넣으면 어색한가

- middleware 인스턴스는 보통 앱 시작 시 만들어지고 오래 살아간다.
- 하지만 인증 사용자, 요청 추적 ID, `DbContext` 같은 값은 요청마다 달라진다.
- 오래 사는 객체가 요청별 상태를 생성자 필드로 붙잡으면, middleware의 역할과 요청 독립성 모델에 어긋난다.

### 잘못 설명하면 안 되는 포인트

- 문제를 "DI 규칙 암기"로만 설명하지 않기
- 더 본질적인 이유는 middleware가 요청 공용 파이프라인 조각이라는 점
- 그래서 요청별 상태는 constructor보다 request execution 시점에 받아야 한다는 점

### 비교 예시

좋지 않은 예:

```csharp
public sealed class RequestAuditMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ICurrentUserContext _currentUserContext;

  public RequestAuditMiddleware(RequestDelegate next, ICurrentUserContext currentUserContext)
  {
    _next = next;
    _currentUserContext = currentUserContext;
  }
}
```

- 설명 포인트
  - `ICurrentUserContext`는 요청마다 달라져야 하는 값이다.
  - 그런데 middleware 생성자 필드에 붙잡으면 요청 독립성이 흐려진다.

더 자연스러운 예:

```csharp
public sealed class RequestAuditMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<RequestAuditMiddleware> _logger;
  private readonly IAppMetadataProvider _appMetadataProvider;

  public RequestAuditMiddleware(
    RequestDelegate next,
    ILogger<RequestAuditMiddleware> logger,
    IAppMetadataProvider appMetadataProvider)
  {
    _next = next;
    _logger = logger;
    _appMetadataProvider = appMetadataProvider;
  }

  public async Task InvokeAsync(HttpContext context, ICurrentUserContext currentUserContext)
  {
    _logger.LogInformation(
      "App: {AppName}, User: {UserName}",
      _appMetadataProvider.ApplicationName,
      currentUserContext.UserName ?? "anonymous");

    await _next(context);
  }
}
```

- 설명 포인트
  - 앱 메타데이터처럼 요청과 무관한 값은 constructor 주입이 자연스럽다.
  - 현재 사용자처럼 요청마다 달라지는 값은 `InvokeAsync` 시점에 받는 편이 자연스럽다.

### 강의 한 줄 정리

- "middleware는 요청을 통과시키는 공용 파이프라인이기 때문에, 요청별 상태는 들고 있는 것이 아니라 지나갈 때 받아야 한다."

## 8) Swagger UI 연결

### 8단계 같이 띄울 Microsoft Learn 참고 자료

- Tutorial: Create a controller-based web API with ASP.NET Core
- 링크: [Tutorial: Create a controller-based web API with ASP.NET Core](https://learn.microsoft.com/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0#create-api-testing-ui-with-swagger)

### `Program.cs` 개발 환경 블록 수정

```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}
```

### 8단계 설명 포인트

- `MapOpenApi()`는 OpenAPI JSON endpoint를 노출한다.
- `UseSwaggerUi(...)`는 그 문서를 사람이 테스트할 수 있는 UI로 연결한다.
- OpenAPI 문서 생성과 UI 렌더링은 분리된 관심사다.

## 9) 실행 및 확인

### 9단계 실행 명령

```powershell
dotnet run
```

### 확인 URL

- OpenAPI JSON
  - `https://localhost:<port>/openapi/v1.json`
- Swagger UI
  - `https://localhost:<port>/swagger`
- API endpoint
  - `https://localhost:<port>/api/secrets/1`

### Swagger UI에서 테스트할 항목

1. `GET /api/secrets/{id}` endpoint를 연다.
2. `Try it out`을 누른다.
3. `id`에 `1`을 입력한다.
4. `Execute`를 눌러 응답을 확인한다.
5. 응답 JSON과 status code를 본다.

### 추가로 확인할 것

- response header에 `X-Training-Day: Day1`가 있는지
- 존재하지 않는 `id` 예: `999`를 호출했을 때 `404`가 나오는지

## 10) Day 1 마무리 질문

- 왜 `Program.cs`를 읽으면 앱 구조가 보이는가
- 왜 service를 등록하고 controller에는 interface를 주입하는가
- 왜 middleware는 기능 추가 메커니즘이라고 말할 수 있는가
- 왜 controller 코드는 짧지만 실제로는 많은 런타임 기능에 기대고 있는가
- 왜 OpenAPI와 Swagger UI도 같은 확장 모델 위에 올라가는가

## 11) 강의 중 강조할 함정 포인트

- controller에서 service를 직접 `new` 하지 않기
- middleware 순서를 대충 두지 않기
- Swagger UI와 OpenAPI JSON을 같은 개념으로 섞어 말하지 않기
- scoped service와 singleton/service lifetime 혼동하지 않기

## 12) 이 시나리오 다음 단계

- Day 2에서 `DbContext`를 등록해 DI와 lifetime 이야기를 EF Core로 연결한다.
- 이후 `SecretService`를 in-memory list 기반 구현에서 EF Core 기반 구현으로 교체한다.
- 그러면 Day 1에서 배운 `Program.cs`, DI, controller 구조가 Day 2의 EF Core 내용과 자연스럽게 이어진다.
