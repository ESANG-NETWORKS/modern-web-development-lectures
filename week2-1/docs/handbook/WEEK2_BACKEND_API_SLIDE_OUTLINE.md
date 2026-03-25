# 2주차 백엔드 교육 슬라이드 아웃라인

본 문서는 [2주차 교육 세부 계획](WEEK2_BACKEND_API_TRAINING_PLAN.md)을 기준으로, 1일차 ASP.NET Core와 2일차 EF Core 강의 흐름을 슬라이드 단위로 정리한 아웃라인이다.

발표 문구와 예시 코드를 포함한 확장 자료는 [WEEK2_BACKEND_API_SLIDE_DRAFT.md](WEEK2_BACKEND_API_SLIDE_DRAFT.md)를 참고한다.

## 전체 구성

- Day 1: 6장
- Day 2: 8장 (Part 0 개념 전환 슬라이드 포함)
- 각 1시간 기준

## Day 1. ASP.NET Core

### 슬라이드 1. 왜 `Program.cs`부터 봐야 하는가

- 제목
  - ASP.NET Core는 시작 파일에서 아키텍처가 드러난다
- 핵심 포인트
  - `CreateBuilder`
  - `Services`
  - `Build`
  - `Run`

### 슬라이드 2. DI 컨테이너와 서비스 등록

- 제목
  - 객체를 직접 만들지 않고 조립하는 이유
- 핵심 포인트
  - 서비스 등록
  - 생성자 주입
  - framework-provided service

### 슬라이드 3. Lifetime과 Scope

- 제목
  - `Singleton`, `Scoped`, `Transient`는 언제 달라지는가
- 핵심 포인트
  - 요청 단위 scope
  - `DbContext`가 scoped인 이유
  - middleware와 scoped service 주의점

### 슬라이드 4. Middleware와 요청 파이프라인

- 제목
  - 기능은 어떻게 파이프라인에 끼워 넣는가
- 핵심 포인트
  - `Use`, `Map`, `Run`
  - 순서의 의미
  - `HttpContext`

### 슬라이드 5. Controller 코드와 C# 문법

- 제목
  - controller 메서드가 짧아 보이는 이유
- 핵심 포인트
  - attribute routing
  - model binding
  - `ActionResult<T>`
  - async/await

### 슬라이드 6. Swagger/OpenAPI로 마무리

- 제목
  - Swagger도 ASP.NET Core 확장 메커니즘의 일부다
- 핵심 포인트
  - `AddOpenApi`
  - `MapOpenApi`
  - 개발 환경 노출

## Day 2. Entity Framework Core

### 슬라이드 0. Procedure 중심 vs Entity 기반 — 왜 사고 방식이 달라지는가

- 제목
  - ORM은 SQL 편의 도구가 아니라 데이터 관점 자체를 바꾸는 방식이다
- 핵심 포인트
  - Procedure 중심: DB가 먼저, SQL이 중심, 객체는 결과를 담는 그릇
  - Entity 기반: 객체가 먼저, 상태 변화가 중심, SQL은 ORM이 생성
  - 용어 대조표: ResultSet↔Entity, SQL Mapper↔LINQ 번역기, 명시적 UPDATE↔`SaveChanges`
  - 코드 대비: MyBatis `updateTitle` vs EF Core `secret.Title = ...; SaveChangesAsync()`
- 강의 목적
  - `ChangeTracker`, `SaveChanges`, `AsNoTracking`이 왜 필요한지의 출발점을 제공한다
  - 우열 비교가 아닌 트레이드오프 관점으로 설명한다

### 슬라이드 1. EF Core를 어떤 눈으로 봐야 하는가

- 제목
  - EF Core는 단순 ORM이 아니라 상태 추적기와 쿼리 번역기다
- 핵심 포인트
  - `DbContext`
  - `ChangeTracker`
  - entity state

### 슬라이드 2. 엔티티 매니저 관점에서 보기

- 제목
  - JPA의 `EntityManager`와 비슷한 역할은 어디에 있는가
- 핵심 포인트
  - `DbContext + ChangeTracker`
  - `Added`, `Modified`, `Deleted`, `Unchanged`
  - `SaveChanges`

### 슬라이드 3. 성능 효율적인 조회 구문

- 제목
  - tracking이 필요한가부터 먼저 묻기
- 핵심 포인트
  - `AsNoTracking`
  - `AsNoTrackingWithIdentityResolution`
  - projection
  - `IQueryable`

### 슬라이드 4. LINQ와 SQL 번역

- 제목
  - LINQ는 언제 C#이고, 언제 SQL이 되는가
- 핵심 포인트
  - `IQueryable` vs `IEnumerable`
  - expression tree
  - query composition

### 슬라이드 5. 복잡한 쿼리 작성 예시

- 제목
  - 검색, 정렬, 집계가 섞인 쿼리를 어떻게 읽을 것인가
- 핵심 포인트
  - filter 조합
  - join/navigation
  - grouping/aggregation
  - paging

### 슬라이드 6. 예상 SQL과 실제 SQL 비교

- 제목
  - LINQ를 쓰더라도 SQL을 상상해야 한다
- 핵심 포인트
  - `ToQueryString()`
  - SQL shape 확인
  - 성능 감각

### 슬라이드 7. 디버깅과 로깅

- 제목
  - 느린 쿼리와 이상한 update를 어떻게 추적할 것인가
- 핵심 포인트
  - `LogTo`
  - ASP.NET logging 통합
  - `ChangeTracker.DebugView`
  - command log category
