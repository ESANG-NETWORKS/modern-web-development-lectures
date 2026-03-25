# 2주차 2회차 EF Core 교육 자료 및 예시 프로젝트 제작 계획

본 문서는 2주차 2회차 Entity Framework Core 교육에 필요한 자료와 예시 프로젝트를 준비하기 위한 제작 계획서다.
기존 문서 현황을 확인하고, 남은 작업과 새로 만들어야 할 예시 프로젝트 구조를 정의한다.

## 이 계획의 방향

- 교육 내용은 이미 기존 문서에 충분히 서술되어 있으므로, 제작 계획은 **전달 형태와 예시 프로젝트 구성**에 집중한다.
- 예시 프로젝트는 강의 중 라이브 또는 사전 완성 코드 모두로 쓸 수 있도록, 단계별로 명확히 분리된 구조로 만든다.
- 기존 `EsSecrets.Day1` 프로젝트를 그대로 이어 사용하는 CLI 시나리오와 별개로, 강의 참석자가 최종 결과를 바로 실행해 볼 수 있는 **독립 완성 프로젝트 `EsSecrets.Day2`** 를 별도로 제공한다.
- EF Core의 내부 동작을 설명하기 전에, **Procedure 중심 개발 방법론과 Entity 기반 개발 방법론의 용어 및 개념 차이**를 먼저 짚는다.
  - 팀 구성원 대부분이 MyBatis, Stored Procedure, 직접 SQL 작성 방식에 익숙하다고 가정한다.
  - "왜 이렇게 코드를 짜야 하는가"에 대한 맥락 없이 API를 소개하면, DbContext와 ChangeTracker가 추상적인 기능 덩어리로만 보인다.
  - 두 방법론의 사고 방식 차이를 먼저 정리한 뒤 EF Core 코드로 내려오는 것이 이 강의의 핵심 흐름이다.

## 기존 자료 현황

| 문서                                                                     | 상태 | 비고                              |
| ------------------------------------------------------------------------ | ---- | --------------------------------- |
| [WEEK2_BACKEND_API_TRAINING_PLAN.md](WEEK2_BACKEND_API_TRAINING_PLAN.md) | 완료 | Day 2 EF Core 강의 계획 포함      |
| [WEEK2_BACKEND_API_SLIDE_OUTLINE.md](WEEK2_BACKEND_API_SLIDE_OUTLINE.md) | 완료 | Day 2 슬라이드 7장 아웃라인 포함  |
| [WEEK2_BACKEND_API_SLIDE_DRAFT.md](WEEK2_BACKEND_API_SLIDE_DRAFT.md)     | 완료 | Day 2 발표 멘트 및 예시 코드 포함 |
| [WEEK2_DAY2_EFCORE_CLI_SCENARIO.md](WEEK2_DAY2_EFCORE_CLI_SCENARIO.md)   | 완료 | 15단계 CLI 따라하기 시나리오      |

## 남은 작업 목록

| 작업                                         | 산출물                                 | 우선순위 |
| -------------------------------------------- | -------------------------------------- | -------- |
| Procedure vs Entity 개념 비교 섹션 강의 설계 | 슬라이드 Part 0 신규 작성              | 높음     |
| 용어 대조표 작성                             | 슬라이드/문서 삽입용 표                | 높음     |
| 예시 프로젝트 생성                           | `EsSecrets.Day2/`                      | 높음     |
| 미들웨어 파일 이전                           | `Middleware/RequestAuditMiddleware.cs` | 높음     |
| 패키지 및 설정 확인                          | `.csproj`, `appsettings.json`          | 높음     |

---

## Procedure 중심 vs Entity 기반 개념 차이 교육 설계

### 이 섹션이 필요한 이유

- SQL Mapper(MyBatis) 또는 Stored Procedure 중심으로 개발해온 경우, ORM은 "SQL을 대신 써주는 편의 도구"로 오해하기 쉽다.
- 이 오해를 가진 채로 EF Core를 쓰면, tracking 비용을 무시하거나 `SaveChanges` 없이 변경이 반영되지 않는 이유를 이해하지 못한다.
- Procedure 중심과 Entity 기반은 **데이터를 바라보는 관점 자체**가 다르다. 이 차이를 짚는 것이 Day 2 전체의 입구다.

### 사고 방식 차이: 핵심 대비

| 관점                   | Procedure 중심                               | Entity 기반 (ORM)                                  |
| ---------------------- | -------------------------------------------- | -------------------------------------------------- |
| 데이터의 기본 단위     | 행(Row), 결과셋(ResultSet)                   | 객체(Entity), 객체 그래프                           |
| 변경 반영 방법         | `UPDATE` SQL을 직접 실행                     | 객체 상태 변경 → `SaveChanges`가 SQL을 생성        |
| 관계 표현 방식         | JOIN 쿼리, FK 컬럼 직접 참조                 | Navigation property, Include                       |
| 트랜잭션 경계          | 명시적 `BEGIN/COMMIT` 또는 stored proc 안    | `DbContext` 인스턴스 단위가 작업 단위(Unit of Work) |
| 쿼리 작성 위치         | SQL 파일, XML mapper, stored procedure       | LINQ expression tree → SQL 번역                    |
| 객체와 DB 간 매핑 책임 | 개발자가 ResultSet → 객체 변환 코드 직접 작성 | ORM runtime이 자동 materialization                 |
| 상태 추적              | 없음. 변경은 항상 명시적 SQL                | ChangeTracker가 객체 상태를 `Added/Modified/Deleted/Unchanged`로 추적 |
| 동일 객체 보장         | 없음. 같은 FK로 2번 조회하면 2개의 독립 결과 | Identity Map — 같은 PK는 같은 인스턴스로 반환     |

### 용어 대조표

| Procedure 중심 용어        | Entity 기반 대응 용어 (EF Core)             | 설명                                                              |
| -------------------------- | ------------------------------------------- | ----------------------------------------------------------------- |
| SQL 결과셋(ResultSet)       | Materialized entity / projection DTO        | 쿼리 결과를 담는 형태. ORM은 객체로 직접 채운다.                  |
| SQL Mapper / XML mapper    | LINQ → SQL 번역기 (EF Core query pipeline)  | 쿼리 표현 방식의 차이. expression tree vs SQL 문자열.             |
| Stored Procedure           | DbContext 메서드 + LINQ 조합                | 로직을 DB에 두느냐, 애플리케이션 계층에 두느냐의 차이.           |
| 명시적 UPDATE SQL          | 객체 속성 변경 + `SaveChangesAsync`         | 변경 감지(change detection)가 UPDATE SQL을 자동 생성.            |
| Connection + Command       | DbContext (Unit of Work)                    | 연결 + 명령 + 상태 추적 + 트랜잭션 경계가 DbContext에 집약.      |
| ResultSet row → DTO 변환   | Projection `Select` 또는 entity materialization | ORM이 변환 책임을 담당. projection은 shape를 명시해서 제어.  |
| FK 컬럼 직접 조회          | Navigation property (`Include`)             | 관계 탐색을 객체 그래프로 표현.                                  |
| 트랜잭션 BEGIN/COMMIT       | DbContext 수명 주기 + `SaveChangesAsync` 원자성 | scoped DbContext가 요청 단위 작업 경계와 일치.               |
| Row identity (없음)         | Identity Map (EF Core 캐시)                 | 같은 PK로 2번 조회 시 1개 인스턴스 반환. tracking context 범위 안에서만 동작. |
| N+1 문제 (루프 안 쿼리)    | N+1 문제 (lazy loading 또는 반복 조회)      | 두 방식 모두 발생 가능. EF Core는 `Include`와 projection으로 제어. |

### 강의에서 전달할 핵심 전환 메시지

- "SQL을 직접 쓰던 방식"과 "ORM을 쓰는 방식"은 단순히 코드 스타일이 다른 게 아니다.
- Procedure 중심은 **DB가 먼저다**. 테이블 구조와 SQL이 중심이고 객체는 결과를 담는 그릇이다.
- Entity 기반은 **객체가 먼저다**. 객체 상태 변화를 기술하면 ORM이 SQL을 만든다.
- EF Core의 `ChangeTracker`, `SaveChanges`, `AsNoTracking`이 왜 필요한지는 이 관점 차이에서 출발한다.
- 성능 이슈도 이 차이에서 비롯된다. tracking이 필요 없는 조회에서 tracking이 켜져 있으면, 없던 비용이 생기는 것이다.

### 강의 도입 시나리오 (`Part 0`으로 편성)

- **예상 소요 시간**: 10분
- **도입 질문**: "여러분이 지금까지 특정 행을 수정할 때 어떻게 했나요?"
- **Procedure 중심 코드 예시** (MyBatis/직접 SQL 스타일):

  ```java
  // MyBatis 스타일: 개발자가 어떤 SQL을 실행할지 직접 지정
  secretMapper.updateTitle(id, newTitle);
  ```

  ```sql
  -- mapper XML
  UPDATE secrets SET title = #{title} WHERE secret_id = #{id}
  ```

- **Entity 기반 코드 예시** (EF Core 스타일):

  ```csharp
  // EF Core 스타일: 개발자는 객체 상태만 바꾼다. SQL 생성은 ChangeTracker가 담당
  Secret secret = await context.Secrets.SingleAsync(x => x.SecretId == id);
  secret.Title = newTitle;
  await context.SaveChangesAsync();
  ```

- **대비 포인트**:
  - MyBatis: 개발자가 "어떤 SQL을 실행할지" 직접 지정한다.
  - EF Core: 개발자는 "객체 상태만 바꾼다". SQL 생성은 ChangeTracker가 담당한다.
  - 이 차이가 편리함과 동시에 "왜 update가 나갔는지 모르겠다"는 혼란의 원인이 된다.
  - 그래서 ChangeTracker를 이해하는 게 EF Core 학습의 출발점이다.

### 강의 중 주의할 함정

- Procedure 중심이 나쁘고 Entity 기반이 좋다는 식으로 설명하지 않는다.
- 두 방식은 트레이드오프가 다르며, 맥락에 따라 선택이 달라진다는 관점을 유지한다.
- SQL을 외워야 할 것과 ORM에 맡겨야 할 것의 경계를 설명하는 것이 목표다.
- "LINQ를 쓰면 SQL을 몰라도 된다"는 잘못된 인식을 강화하지 않는다. LINQ는 SQL을 잊는 도구가 아니다.

---

## 예시 프로젝트 계획: `EsSecrets.Day2`

### 프로젝트 위치

```
EsSecrets.Day2/
```

### 프로젝트 목적

- Day 2 강의가 끝난 결과물을 실행 가능한 형태로 복기하거나 리뷰할 때 쓰는 기준 코드다.
- CLI 시나리오가 `EsSecrets.Day1`을 이어 확장하는 반면, `EsSecrets.Day2`는 Day 2 도착 상태를 독립적으로 담은 완성본이다.
- DbContext, ChangeTracker, IQueryable, AsNoTracking, ToQueryString, LogTo 등 Day 2 핵심 개념이 모두 연결된 코드가 있어야 한다.

### 최종 도착 상태

- `dotnet run`으로 Day 2 서버가 바로 실행된다.
- 첫 실행 시 `EnsureCreatedAsync`로 SQLite DB와 seed 데이터가 생성된다.
- 콘솔에 EF Core SQL 로그가 출력된다.
- Swagger UI에서 아래 endpoint를 모두 호출할 수 있다.
  - `GET /api/secrets/{id}` — 단건 조회 (no-tracking + projection)
  - `GET /api/secrets` — 목록/검색 (IQueryable 조합 + 조건부 tracked)
  - `PUT /api/secrets/{id}` — 제목 수정 (tracking + ChangeTracker.DebugView 로그)
  - `GET /api/diagnostics/secrets-sql` — 쿼리 SQL 문자열 반환 (ToQueryString)

### 의존 패키지

```
Microsoft.EntityFrameworkCore.Sqlite
Scalar.AspNetCore
```

### 프로젝트 파일 구성

```
EsSecrets.Day2/
├── EsSecrets.Day2.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Properties/
│   └── launchSettings.json
├── Models/
│   ├── Secret.cs
│   └── AccessLog.cs
├── Data/
│   ├── SecretsDbContext.cs
│   └── SeedData.cs
├── Services/
│   ├── ISecretService.cs
│   └── SecretService.cs
├── Controllers/
│   ├── SecretsController.cs
│   └── DiagnosticsController.cs
└── Middleware/
    └── RequestAuditMiddleware.cs
```

---

## 파일별 정의

### `EsSecrets.Day2.csproj`

- 대상 프레임워크: `net10.0`
- 참조 패키지: `Microsoft.EntityFrameworkCore.Sqlite`, `Scalar.AspNetCore`
- `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>` 포함

### `Models/Secret.cs`

- DB 엔티티. `SecretId`, `Title`, `OwnerName`, `IsDeleted`, `LastModifiedAt`, `AccessLogs` 포함.
- Day 1의 `SecretResponse` (API 계층 DTO)와 분리되어 있다는 점을 강조하는 코드 예시다.

### `Models/AccessLog.cs`

- DB 엔티티. `AccessLogId`, `SecretId`, `ActionType`, `ActorUserName`, `CreatedAt`, `Secret` navigation 포함.
- join/navigation/grouping 예시를 위해 존재한다.

### `Data/SecretsDbContext.cs`

- `DbContext` 구현체. `DbSet<Secret>`, `DbSet<AccessLog>` 포함.
- `OnModelCreating`에서 key, maxLength, FK 관계 구성.
- 강의 포인트: `DbContext`는 단순 연결 객체가 아니라 작업 단위이자 상태 추적 루트.

### `Data/SeedData.cs`

- `EnsureCreatedAsync` 기반 초기화. migrations 미사용(Day 2는 런타임 이해 우선).
- `Secret` 2건 + `AccessLog` 3건 초기 적재.

### `Program.cs`

- `AddDbContext`, `UseSqlite`, `EnableSensitiveDataLogging`, `LogTo(Console.WriteLine, LogLevel.Information)` 포함.
- 앱 시작 시 `SeedData.InitializeAsync` 호출.
- `AddControllers`, `AddOpenApi`, `AddScoped<ISecretService>` 포함.
- Day 1의 DI/lifetime 설명이 `AddDbContext(scoped)` 흐름과 연결됨을 강조.

### `Services/ISecretService.cs`

- 인터페이스 및 DTO 정의:
  - `GetByIdAsync(int id)` → `SecretResponse?`
  - `SearchAsync(string? keyword, bool tracked)` → `IReadOnlyList<SecretListItemResponse>`
  - `GetSearchSql(string? keyword)` → `string`
  - `UpdateTitleAsync(int id, UpdateSecretTitleRequest request)` → `bool`
- 함께 정의되는 DTO: `SecretResponse`, `SecretListItemResponse`, `UpdateSecretTitleRequest`

### `Services/SecretService.cs`

- `GetByIdAsync`: no-tracking + projection → 읽기 전용 단건 조회 패턴
- `SearchAsync`: `IQueryable` 조합 + 조건부 `AsNoTracking` + navigation count
- `GetSearchSql`: `ToQueryString()` 활용한 디버깅 전용 메서드
- `UpdateTitleAsync`: tracking query + `ChangeTracker.DebugView.LongView` 로그 + `SaveChangesAsync`
- 강의 포인트: 같은 DbContext 안에서 tracking 여부에 따라 부하와 동작이 다르다는 점

### `Controllers/SecretsController.cs`

- `GET /api/secrets/{id:int}` → `GetByIdAsync`
- `GET /api/secrets` → `SearchAsync(keyword, tracked)`
- `PUT /api/secrets/{id:int}` → `UpdateTitleAsync`

### `Controllers/DiagnosticsController.cs`

- `GET /api/diagnostics/secrets-sql` → `GetSearchSql(keyword)` 반환
- 강의 포인트: 교육용 디버깅 endpoint. LINQ 뒤의 SQL을 Swagger UI에서 직접 확인하는 용도.

### `Middleware/RequestAuditMiddleware.cs`

- Day 1 코드 그대로 이전.
- `ICurrentUserContext`를 `InvokeAsync`에서 주입받는 방식 유지.
- Day 1 middleware 설명과 Day 2 DbContext 설명의 연결 고리로 활용.

---

## 강의 흐름과 예시 프로젝트 연결

| 강의 파트                                 | 관련 파일                                                 | 강의 시 보여줄 것                                    |
| ----------------------------------------- | --------------------------------------------------------- | ---------------------------------------------------- |
| **Part 0. Procedure vs Entity 개념 전환** | 슬라이드 (용어 대조표, 코드 비교)                         | MyBatis UPDATE vs EF Core SaveChangesAsync 코드 대비 |
| Part 1. DbContext와 ChangeTracker         | `SecretsDbContext.cs`, `SecretService.UpdateTitleAsync`   | DebugView 로그 + UPDATE SQL                          |
| Part 2. AsNoTracking과 projection         | `SecretService.GetByIdAsync`, `SecretService.SearchAsync` | no-tracking 쿼리 vs tracking 쿼리 콘솔 비교          |
| Part 3. IQueryable 조합                   | `SecretService.SearchAsync`                               | keyword 조건 분기 LINQ 코드                          |
| Part 4. 복잡한 쿼리                       | `SecretService.SearchAsync` + `DiagnosticsController`     | AccessLogCount + OrderByDescending                   |
| Part 5. SQL 확인, 로깅, 디버깅            | `Program.cs`, `SecretService.GetSearchSql`                | ToQueryString 반환값, LogTo 콘솔 출력                |

## 담당 시각 자료 연동

| 자료 종류                        | 파일                                                                     | 비고                         |
| -------------------------------- | ------------------------------------------------------------------------ | ---------------------------- |
| 강의 계획                        | [WEEK2_BACKEND_API_TRAINING_PLAN.md](WEEK2_BACKEND_API_TRAINING_PLAN.md) | Day 2 섹션 참고              |
| 슬라이드 아웃라인                | [WEEK2_BACKEND_API_SLIDE_OUTLINE.md](WEEK2_BACKEND_API_SLIDE_OUTLINE.md) | Day 2 슬라이드 7장           |
| 슬라이드 발표 초안               | [WEEK2_BACKEND_API_SLIDE_DRAFT.md](WEEK2_BACKEND_API_SLIDE_DRAFT.md)     | Day 2 발표 멘트 및 예시 코드 |
| CLI 따라하기 시나리오            | [WEEK2_DAY2_EFCORE_CLI_SCENARIO.md](WEEK2_DAY2_EFCORE_CLI_SCENARIO.md)   | EsSecrets.Day1 확장 기준     |
| 예시 프로젝트 (이 계획의 결과물) | `EsSecrets.Day2/`                                                        | 완성 코드 독립 실행용        |

## 완료 기준

**개념 교육 자료**

- [x] Procedure 중심 vs Entity 기반 사고 방식 차이를 설명하는 슬라이드 Part 0이 존재한다.
- [x] 용어 대조표 (ResultSet↔Entity, SQL Mapper↔LINQ 번역기, 명시적 UPDATE↔SaveChanges 등)가 강의 자료에 포함된다.
- [x] MyBatis 스타일 코드와 EF Core 스타일 코드를 나란히 비교하는 예시가 있다.
- [x] "LINQ는 SQL을 잊는 도구가 아니다"는 메시지가 강의 흐름 안에 명시된다.

**예시 프로젝트**

- [x] `EsSecrets.Day2/` 프로젝트가 `dotnet run`으로 실행된다. (`dotnet build` 클린 성공 확인)
- [x] 첫 실행 시 SQLite DB와 seed 데이터가 자동 생성된다. (`SeedData.InitializeAsync` + `EnsureCreatedAsync`)
- [x] Swagger UI에서 `GET`, `PUT` endpoint 4개가 모두 노출된다. (Scalar UI + OpenAPI 등록 완료)
- [x] `PUT /api/secrets/{id}` 호출 시 콘솔에 ChangeTracker DebugView와 UPDATE SQL이 출력된다.
- [x] `GET /api/diagnostics/secrets-sql` 호출 시 SQL 문자열이 반환된다.
- [x] 슬라이드 8장(Part 0 포함)과 예시 프로젝트 파일이 1:1로 연결 가능하다.
