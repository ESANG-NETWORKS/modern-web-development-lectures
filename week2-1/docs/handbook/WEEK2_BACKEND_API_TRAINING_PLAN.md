# 2주차 교육 세부 계획

## 주제

- 1일차: ASP.NET Core 내부 구조와 확장 메커니즘
- 2일차: Entity Framework Core 내부 동작과 성능/디버깅 관점

## 이번 계획의 방향

- 이번 문서는 "프레임워크 사용법 소개"가 아니라 "왜 이런 구조로 설계되었는가"를 설명하는 강의 계획서다.
- MSDN 문서를 읽으면 알 수 있는 API 나열은 줄이고, 프로그램 진입점, 요청 파이프라인, DI 스코프, Change Tracker, LINQ 번역, SQL 확인 같은 내부 동작 이해에 집중한다.
- 즉, 기능 소개보다 런타임 메커니즘과 코드가 그렇게 작성되는 이유를 전달하는 것이 목표다.

## 전체 구성

- 총 2일
- 각 1시간
- Day 1은 ASP.NET Core 런타임과 C# 언어 요소 중심
- Day 2는 EF Core 상태 관리, 쿼리 작성, SQL 확인, 디버깅 중심

## 선행 이해 수준

- 기본 C# 문법은 알고 있다고 가정한다.
- ASP.NET Core와 EF Core를 실무에서 써본 경험은 없어도 된다.
- 단, 클래스, 인터페이스, 생성자, async/await, 제네릭, lambda 정도는 이해하고 있어야 한다.

## 강의 운영 원칙

- 공식 문서 요약이 아니라 "왜 그렇게 만들어졌는가"를 먼저 설명한다.
- 추상 개념은 반드시 Program.cs, middleware, controller, DbContext 같은 실제 코드에 연결한다.
- 디자인 패턴 용어는 필요할 때만 쓰고, 항상 런타임 행동과 연결해서 설명한다.
- Day 2의 "엔티티 매니저" 개념은 EF Core에서는 `DbContext + ChangeTracker`로 설명한다.
  - EF Core에는 JPA의 `EntityManager`와 1:1 대응되는 단일 객체보다, 작업 단위와 상태 관리 책임이 `DbContext`와 `ChangeTracker`에 나뉘어 있다.

## Microsoft Learn 시각 자료 활용 원칙

- 구조를 설명할 때는 텍스트만 읽지 말고 Microsoft Learn의 공식 그림과 스크린샷을 같이 띄운다.
- 특히 요청 파이프라인, lifetime, Change Tracker처럼 말로만 설명하면 추상적으로 들리는 부분은 공식 시각 자료를 먼저 보여주고 코드로 내려온다.
- 단, 이미지 자체가 설명을 대신하게 두지 않고 "이 그림이 실제 코드의 어느 부분을 가리키는가"를 바로 연결한다.

## Day 1에 같이 띄울 Microsoft Learn 참고 자료

- ASP.NET Core fundamentals overview
  - 용도: `Program.cs`가 서비스 구성과 request pipeline 정의의 중심이라는 점을 설명할 때
  - 링크: [ASP.NET Core fundamentals overview](https://learn.microsoft.com/aspnet/core/fundamentals/?view=aspnetcore-10.0)
- ASP.NET Core Middleware
  - 용도: request delegate pipeline 그림으로 middleware의 전후 처리와 순서를 설명할 때
  - 링크: [ASP.NET Core Middleware](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0#create-a-middleware-pipeline-with-%60webapplication%60)
- Dependency injection in ASP.NET Core
  - 용도: service lifetime, scoped service, middleware와 DI의 관계를 설명할 때
  - 링크: [Dependency injection in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0#lifetime-and-registration-options)
- Write custom ASP.NET Core middleware
  - 용도: middleware constructor와 `InvokeAsync`에서 의존성을 어떻게 다르게 받아야 하는지 설명할 때
  - 링크: [Write custom ASP.NET Core middleware](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/write?view=aspnetcore-10.0#middleware-dependencies)

## Day 2에 같이 띄울 Microsoft Learn 참고 자료

- Change Tracker Debugging
  - 용도: Change Tracker DebugView 스크린샷으로 엔티티 상태 추적이 실제로 어떻게 보이는지 설명할 때
  - 링크: [Change Tracker Debugging](https://learn.microsoft.com/ef/core/change-tracking/debug-views#change-tracker-debug-view)
- Tracking vs. No-Tracking Queries
  - 용도: tracking과 no-tracking, identity resolution 차이를 설명할 때
  - 링크: [Tracking vs. No-Tracking Queries](https://learn.microsoft.com/ef/core/querying/tracking#identity-resolution)
- Simple Logging
  - 용도: `LogTo(...)`로 SQL과 이벤트를 확인하는 방법을 보여줄 때
  - 링크: [Simple Logging](https://learn.microsoft.com/ef/core/logging-events-diagnostics/simple-logging)
- ToQueryString
  - 용도: LINQ 뒤의 실제 SQL을 확인하는 디버깅 흐름을 설명할 때
  - 링크: [ToQueryString](https://learn.microsoft.com/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.toquerystring?view=efcore-10.0)

## 1일차: ASP.NET Core (1시간)

### 1일차 목표

- `Program.cs`에서 시작하는 ASP.NET Core 애플리케이션의 실행 진입점을 설명할 수 있다.
- DI 컨테이너, 서비스 등록, 인스턴스 스코프의 차이를 요청 흐름과 함께 설명할 수 있다.
- middleware와 endpoint 구성이 어떻게 확장 포인트로 작동하는지 이해할 수 있다.
- controller 코드가 왜 간결하게 보이는지, 그 배경에 있는 C# 문법과 프레임워크 규약을 설명할 수 있다.
- Swagger/OpenAPI를 예시로 middleware 기반 확장 메커니즘을 설명할 수 있다.

### 1일차 핵심 메시지

- `Program.cs`는 단순 시작 파일이 아니라 앱의 조립 지점이다.
- ASP.NET Core는 "객체를 직접 만들지 않고 등록하고 조합하는 방식" 위에 서 있다.
- middleware는 요청 파이프라인에 기능을 끼워 넣는 방식이며, 대부분의 프레임워크 기능이 이 모델로 확장된다.
- controller의 짧은 코드는 프레임워크가 많은 인프라를 대신 처리하기 때문에 가능하다.

### 1일차 시간 배분

- 0~10분: `Program.cs`와 앱 시작 흐름
- 10~25분: DI, 서비스 등록, lifetime, scope
- 25~40분: middleware와 요청 파이프라인
- 40~50분: controller 코드와 C# 문법 연결
- 50~60분: Swagger/OpenAPI로 middleware 확장 마무리

### 1일차 세부 구성

#### Part 1. `Program.cs`에서 시작되는 애플리케이션 조립

- 다룰 내용
  - `WebApplication.CreateBuilder(args)`가 무엇을 준비하는가
  - `builder.Services`는 왜 존재하는가
  - `builder.Build()`에서 어떤 종류의 조립이 일어나는가
  - `app.Run()`은 무엇을 시작하는가
- 설명 포인트
  - 설정, 로깅, DI, 호스팅이 초기에 묶여 올라온다는 점
  - 프로그램 시작과 요청 처리 시작은 같은 개념이 아니라는 점
- 보여줄 코드 범위
  - 최소 `Program.cs` 예제
  - 서비스 등록 2~3개
  - `MapControllers`, `MapOpenApi` 정도의 endpoint 구성
- 같이 띄울 자료
  - ASP.NET Core fundamentals overview 페이지의 `Program.cs` 설명

#### Part 2. DI와 인스턴스 스코프

- 반드시 설명할 내용
  - `Singleton`, `Scoped`, `Transient`의 차이
  - 왜 `DbContext`는 보통 scoped인가
  - 요청마다 같은 인스턴스가 유지되는 것과 전체 앱에서 하나인 것은 어떻게 다른가
  - middleware constructor와 scoped service의 관계
- 설명 포인트
  - DI는 편의 기능이 아니라 결합도 제어 장치라는 점
  - 생성자 주입, framework-provided service, open generic logger의 의미
  - 요청 단위 scope가 왜 웹 프로그래밍과 잘 맞는가
- 예시
  - 같은 요청 안에서 scoped service 인스턴스 재사용
  - 다른 요청에서 scoped service가 달라지는 시나리오
- 같이 띄울 자료
  - DI lifetime and registration options 문서

#### Part 3. Middleware와 기능 추가 메커니즘

- 반드시 설명할 내용
  - ASP.NET Core 요청 파이프라인 개념
  - `Use`, `Map`, `Run`의 역할 차이
  - middleware가 `HttpContext`를 받아 다음 middleware로 넘기는 구조
  - 순서가 왜 중요한가
- 설명 포인트
  - 인증, 예외 처리, 정적 파일, Swagger UI 등 많은 기능이 middleware로 붙는다는 점
  - endpoint routing이 middleware 파이프라인과 어떻게 연결되는가
  - custom middleware에서 scoped 의존성을 다룰 때 왜 `InvokeAsync` 주입을 써야 하는가
- 시연 후보
  - 간단한 custom middleware 하나
  - logging 또는 header 추가 예시
- 같이 띄울 자료
  - ASP.NET Core Middleware의 request delegate pipeline 그림
  - custom middleware dependencies 문서

#### Part 4. Controller 코드가 간결해지는 이유

- 반드시 설명할 내용
  - attribute routing
  - model binding
  - action result
  - async/await
  - generic `ActionResult<T>`
  - lambda, extension method, nullable reference type 정도의 문법 연결
- 설명 포인트
  - controller 메서드가 짧아 보이는 이유는 C# 문법 + ASP.NET Core conventions가 같이 작동하기 때문이라는 점
  - parameter binding, validation, serialization이 이미 파이프라인에 묶여 있다는 점
- 예시 코드
  - `HttpGet`, route parameter, DI된 service 호출, `Ok`, `NotFound`

#### Part 5. Swagger/OpenAPI로 마무리

- 반드시 설명할 내용
  - OpenAPI document 생성이 어떤 middleware/endpoint 확장 포인트에 붙는가
  - `AddOpenApi`, `MapOpenApi`가 어디에 개입하는가
  - 왜 개발 환경에서만 노출하는 경우가 많은가
- 설명 포인트
  - Swagger는 standalone 도구가 아니라 ASP.NET Core 확장 메커니즘 위에 올라간 기능이라는 점
  - middleware 설명을 추상적으로 끝내지 않고 눈에 보이는 결과물로 닫는 용도
- 같이 띄울 자료
  - OpenAPI support in ASP.NET Core API apps
  - 링크: [OpenAPI support in ASP.NET Core API apps](https://learn.microsoft.com/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-10.0)

### 1일차 실습/시연 범위

- `Program.cs` 한 파일을 기준으로 서비스 등록과 pipeline 구성을 읽어보기
- custom middleware 1개 추가
- controller action 1개 확인
- OpenAPI endpoint 노출 확인
- CLI 따라하기 시나리오: [WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md](WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md)

### 1일차 강의 후 팀이 말할 수 있어야 하는 것

- 왜 ASP.NET Core에서는 직접 `new`보다 DI 등록을 먼저 생각하는가
- 왜 middleware 순서가 기능 결과를 바꾸는가
- 왜 controller 메서드는 짧아도 실제로 많은 일이 일어나는가
- 왜 Swagger도 결국 확장 메커니즘의 한 예시인가

## 2일차: Entity Framework Core (1시간)

### 2일차 목표

- EF Core에서 엔티티 상태 관리가 어떻게 동작하는지 설명할 수 있다.
- `DbContext`와 `ChangeTracker`가 어떤 역할을 하는지 이해할 수 있다.
- `IQueryable`, LINQ, SQL 번역의 관계를 설명할 수 있다.
- `AsNoTracking`, projection, query composition 같은 성능 관련 선택지를 이유와 함께 설명할 수 있다.
- 생성된 SQL을 확인하고, 로그/디버그 뷰를 통해 문제를 추적하는 방법을 이해할 수 있다.

### 2일차 핵심 메시지

- EF Core는 단순 SQL 래퍼가 아니라 상태 추적기와 쿼리 번역기를 가진 런타임이다.
- JPA의 `EntityManager`처럼 이해하고 싶다면, EF Core에서는 `DbContext`와 `ChangeTracker`를 함께 봐야 한다.
- LINQ는 문법 설탕이 아니라 expression tree를 통한 쿼리 구성 도구다.
- 성능은 "어떤 메서드를 외웠는가"보다 "지금 tracking이 필요한가, SQL이 어떻게 나갈까"를 생각하는 데서 나온다.

### 2일차 시간 배분

- 0~15분: `DbContext`, `ChangeTracker`, 엔티티 상태
- 15~30분: `IQueryable`, LINQ 통합, 쿼리 번역
- 30~45분: 성능 관련 구문과 복잡한 쿼리 작성
- 45~55분: SQL 비교, `ToQueryString`, 로깅/디버깅
- 55~60분: 정리 및 질의응답

### 2일차 세부 구성

#### Part 1. EF Core에서 실제로 어떤 일이 일어나는가

- 반드시 설명할 내용
  - `DbContext`는 무엇을 대표하는가
  - `ChangeTracker`는 무엇을 추적하는가
  - 엔티티 상태 `Added`, `Modified`, `Deleted`, `Unchanged`의 의미
  - `SaveChanges` 호출 전후에 어떤 일이 일어나는가
- 설명 포인트
  - 조회만 하는 context와 수정하는 context의 부담이 다르다는 점
  - 상태 추적이 편의성과 비용을 동시에 가져온다는 점
  - EF Core의 내부 모델을 모르면 성능/버그를 설명하기 어렵다는 점
- 보여줄 코드 범위
  - 조회 후 속성 변경 후 `SaveChangesAsync`
  - `ChangeTracker.DebugView` 출력 예시
- 같이 띄울 자료
  - Change Tracker Debugging의 DebugView 스크린샷

#### Part 2. 성능 효율적인 엔티티 제어를 위한 구문

- 반드시 설명할 내용
  - `AsNoTracking`
  - `AsNoTrackingWithIdentityResolution`
  - `IQueryable` 기반 query composition
  - projection `Select`
  - `Include`와 filtered include 사용 시점
- 설명 포인트
  - 읽기 전용 조회에서 tracking 비용을 줄이는 이유
  - tracking을 껐을 때 identity resolution이 달라지는 점
  - entity 전체를 가져올지, 필요한 shape만 가져올지 판단하는 기준
- 보여줄 예시
  - tracking query vs no-tracking query 비교
  - projection과 entity materialization 비교
- 같이 띄울 자료
  - Tracking vs. No-Tracking Queries 문서

#### Part 3. LINQ와의 통합

- 반드시 설명할 내용
  - LINQ to Objects와 LINQ to Entities는 실행 시점과 의미가 다르다는 점
  - `IQueryable`과 `IEnumerable` 차이
  - expression tree가 SQL로 번역되는 과정의 개념
  - 클라이언트 평가와 서버 평가를 구분하는 감각
- 설명 포인트
  - 메서드 체이닝이 단순 문법이 아니라 쿼리 조합 단계라는 점
  - `Where`, `Select`, `OrderBy`, `GroupBy`, `Join`이 실제 SQL 형태와 어떻게 연결되는지
- 예시
  - 동적 필터 추가
  - 검색 조건 조합
  - 정렬/페이징 조합

#### Part 4. 복잡한 쿼리와 SQL 비교

- 반드시 설명할 내용
  - projection 기반 목록 조회
  - join 또는 navigation 기반 조회
  - 그룹핑이나 집계가 들어간 예시
  - 예상 SQL을 사람이 먼저 머릿속으로 떠올려 보는 습관
- 설명 포인트
  - 복잡한 LINQ를 작성할 때는 SQL을 상상하며 짜야 한다는 점
  - "LINQ가 되니까 쓴다"가 아니라 "이 SQL이 필요해서 이 LINQ를 쓴다"로 사고해야 한다는 점
- 보여줄 예시
  - 기간 필터 + 상태 필터 + keyword 검색 + 정렬 + 페이징
  - 접근기록 집계 조회 같은 예시

#### Part 5. SQL 확인, 디버깅, 로깅

- 반드시 설명할 내용
  - `ToQueryString()`
  - `LogTo(...)`
  - ASP.NET Core logging과 EF Core logging의 연결
  - `ChangeTracker.DebugView.ShortView`, `LongView`
  - command logging category 확인
- 설명 포인트
  - 예상 SQL과 실제 SQL을 비교하는 방법
  - 쿼리가 느릴 때 무엇부터 확인해야 하는가
  - 데이터가 왜 update 대상으로 잡혔는지 상태 추적 뷰로 보는 방법
- 보여줄 예시
  - 쿼리 SQL 출력
  - command log 확인
  - state change debug view 확인
- 같이 띄울 자료
  - Simple Logging 문서
  - ToQueryString API 문서

### 2일차 실습/시연 범위

- tracking query와 `AsNoTracking` query 비교
- 동적 검색 조건이 붙는 `IQueryable` 예시
- projection 기반 복합 조회 1개
- `ToQueryString()`으로 SQL 확인
- EF Core 로그 또는 `ChangeTracker.DebugView` 확인
- CLI 따라하기 시나리오: [WEEK2_DAY2_EFCORE_CLI_SCENARIO.md](WEEK2_DAY2_EFCORE_CLI_SCENARIO.md)

### 2일차 강의 후 팀이 말할 수 있어야 하는 것

- EF Core에서 상태 관리가 왜 성능과 직결되는가
- 왜 조회에서는 `AsNoTracking`과 projection을 먼저 고려하는가
- 왜 `IQueryable`을 조합하다가 `ToListAsync()` 시점에 실제 실행이 일어나는가
- 왜 복잡한 LINQ는 항상 예상 SQL과 함께 검토해야 하는가
- 왜 디버깅 시 로그와 `DebugView`가 중요한가

## 두 날 공통으로 강조할 것

- 프레임워크는 편의를 주지만, 내부 메커니즘을 모르면 디버깅과 성능 최적화가 막힌다.
- "이 API를 어떻게 쓰는가"보다 "왜 이렇게 설계됐는가"를 이해해야 응용이 된다.
- C# 언어 요소와 프레임워크 런타임은 분리된 주제가 아니라 함께 움직인다.

## 이번 계획에서 일부러 빼는 것

- JWT 상세 구현 실습
- 전체 CRUD 완성
- OpenAPI Generator 연동 실습
- 고급 DDD 설계 논쟁
- Repository 패턴 찬반 토론

## 산출물

- Day 1 CLI 실습 시나리오: [WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md](WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md)
- Day 2 CLI 실습 시나리오: [WEEK2_DAY2_EFCORE_CLI_SCENARIO.md](WEEK2_DAY2_EFCORE_CLI_SCENARIO.md)
- Day 1 발표용 자료: [WEEK2_BACKEND_API_SLIDE_OUTLINE.md](WEEK2_BACKEND_API_SLIDE_OUTLINE.md)
- Day 1/Day 2 발표 초안: [WEEK2_BACKEND_API_SLIDE_DRAFT.md](WEEK2_BACKEND_API_SLIDE_DRAFT.md)

## 후속 연결 포인트

- Day 1 이해가 잡혀야 이후 인증, 예외 처리, OpenAPI 확장이 쉬워진다.
- Day 2 이해가 잡혀야 이후 CRUD, 성능 이슈, SQL 튜닝 대화가 가능해진다.
- 3주차 프론트엔드 연계 전에 API 구현과 SQL 확인 기준을 팀 내부 공통 언어로 맞추는 것이 목표다.
- 필요 시 2주차 말에 OpenAPI Generator 입력 검증 세션을 30분 추가한다.

---

본 계획서는 2주차 교육 운영 기준 문서이며, 실제 구현 시작 시에는 유스케이스 우선순위와 설계 문서를 함께 참조한다.
