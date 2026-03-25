# Gemini Pro + Canvas 기반 PPT 자동 생성 가이드

본 문서는 2주차 2회차(EF Core) 교육 자료를 Google Gemini Pro + Canvas로 슬라이드 자동 생성할 때,
어떤 파일을 어떤 순서로 첨부하고 어떤 프롬프트로 생성할지 실무 기준으로 정리한 가이드다.

## 목표

- 최소 수정으로 발표 가능한 슬라이드 초안을 빠르게 만든다.
- Procedure 중심 vs Entity 기반 개념 전환(Part 0)을 반드시 반영한다.
- 기존 산출물(계획/아웃라인/초안/시나리오/예시 프로젝트) 간 불일치를 줄인다.

## 권장 워크플로우 (실행 순서)

1. Gemini에서 모델을 Pro로 선택한다.
2. Canvas 모드를 연다.
3. 아래 "필수 첨부 파일"을 순서대로 업로드한다.
4. "1차 아웃라인 생성 프롬프트"를 실행한다.
5. 결과를 검토하고 "2차 본문 확장 프롬프트"를 실행한다.
6. 마지막으로 "3차 디자인/발표자 멘트 정리 프롬프트"를 실행한다.
7. 생성 결과를 Google Slides 초안으로 변환한다.
8. 최종 PPTX로 내보낸다.

## 첨부 파일 가이드

### 필수 첨부 파일 (우선순위 높음)

1. [docs/training/handbook/WEEK2_DAY2_EFCORE_DELIVERY_PLAN.md](docs/training/handbook/WEEK2_DAY2_EFCORE_DELIVERY_PLAN.md)

- 역할: 이번 산출물의 기준 문서
- 특히 중요: Part 0(Procedure vs Entity), 완료 기준, 예시 프로젝트 범위

1. [docs/training/handbook/WEEK2_BACKEND_API_SLIDE_OUTLINE.md](docs/training/handbook/WEEK2_BACKEND_API_SLIDE_OUTLINE.md)

- 역할: 슬라이드 목차/장수 기준
- 특히 중요: Day 2가 8장(Part 0 포함)으로 구성되어야 한다는 기준

1. [docs/training/handbook/WEEK2_BACKEND_API_SLIDE_DRAFT.md](docs/training/handbook/WEEK2_BACKEND_API_SLIDE_DRAFT.md)

- 역할: 발표 문구 + 코드 예시 소스
- 특히 중요: 슬라이드별 발표 멘트 톤 통일

### 보조 첨부 파일 (상황별)

1. [docs/training/handbook/WEEK2_BACKEND_API_TRAINING_PLAN.md](docs/training/handbook/WEEK2_BACKEND_API_TRAINING_PLAN.md)

- 역할: 수업 시간 배분/핵심 메시지 검증
- 사용 시점: 슬라이드 흐름이 시간 배분과 어긋날 때

1. [docs/training/handbook/WEEK2_DAY2_EFCORE_CLI_SCENARIO.md](docs/training/handbook/WEEK2_DAY2_EFCORE_CLI_SCENARIO.md)

- 역할: 실습 단계와 코드 흐름 근거
- 사용 시점: 데모 슬라이드(실행 순서, 검증 포인트) 강화 시

### 코드 샘플 첨부 (강력 권장)

1. [EsSecrets.Day2/Program.cs](EsSecrets.Day2/Program.cs)

- 역할: DI, DbContext, OpenAPI, SeedData 연결 구조를 한 장에서 설명할 근거

1. [EsSecrets.Day2/Services/SecretService.cs](EsSecrets.Day2/Services/SecretService.cs)

- 역할: AsNoTracking, IQueryable, ToQueryString, ChangeTracker 메시지의 핵심 근거

1. [EsSecrets.Day2/Data/SecretsDbContext.cs](EsSecrets.Day2/Data/SecretsDbContext.cs)

- 역할: Entity 관계와 DbContext 역할 설명 근거

1. [EsSecrets.Day2/Controllers/SecretsController.cs](EsSecrets.Day2/Controllers/SecretsController.cs)

- 역할: 조회/수정 endpoint 시연 슬라이드 근거

1. [EsSecrets.Day2/Controllers/DiagnosticsController.cs](EsSecrets.Day2/Controllers/DiagnosticsController.cs)

- 역할: SQL 확인 endpoint(ToQueryString 반환) 근거

## 업로드 순서 템플릿

- 1차: Delivery Plan + Slide Outline
- 2차: Slide Draft
- 3차: Training Plan + CLI Scenario
- 4차: Day2 코드 파일 5개

이 순서를 쓰면 "구조 먼저 확정 → 문구 확장 → 코드 근거 보강" 순으로 품질이 안정된다.

## 프로시저 패턴 병행 사용 가이드 (추가 예제)

EF Core 전환을 설명할 때도 기존 Stored Procedure 패턴은 충분히 병행 가능하다는 점을 명시하면,
기존 실무 습관을 가진 청중의 저항감을 줄이고 현실적인 전환 전략을 제시할 수 있다.

### 강의 메시지 권장 문구

- "Entity 기반으로 전환해도 모든 SQL을 즉시 버릴 필요는 없습니다."
- "복잡 집계, 레거시 검증 로직, DB 튜닝이 끝난 쿼리는 프로시저를 유지하고, 일반 CRUD/조회는 EF Core로 이관하는 하이브리드가 가능합니다."
- "핵심은 우열이 아니라 경계 설정입니다."

### 슬라이드 삽입용 예제 (Procedure도 사용 가능)

```sql
-- SQL Server 예시: 기존 프로시저 유지
CREATE PROCEDURE dbo.usp_SecretSummary
 @Keyword NVARCHAR(100)
AS
BEGIN
 SET NOCOUNT ON;

 SELECT s.SecretId,
     s.Title,
     s.OwnerName,
     COUNT(al.AccessLogId) AS AccessLogCount
 FROM Secrets s
 LEFT JOIN AccessLogs al ON al.SecretId = s.SecretId
 WHERE s.IsDeleted = 0
   AND (@Keyword IS NULL OR s.Title LIKE '%' + @Keyword + '%')
 GROUP BY s.SecretId, s.Title, s.OwnerName
 ORDER BY MAX(s.LastModifiedAt) DESC;
END
```

```csharp
// EF Core에서 프로시저 결과를 호출하는 하이브리드 예시
public async Task<IReadOnlyList<SecretSummaryRow>> GetSummaryByProcedureAsync(string? keyword)
{
 var param = new SqlParameter("@Keyword", (object?)keyword ?? DBNull.Value);

 return await _dbContext
  .Set<SecretSummaryRow>()
  .FromSqlRaw("EXEC dbo.usp_SecretSummary @Keyword", param)
  .AsNoTracking()
  .ToListAsync();
}

public sealed class SecretSummaryRow
{
 public int SecretId { get; set; }
 public string Title { get; set; } = string.Empty;
 public string OwnerName { get; set; } = string.Empty;
 public int AccessLogCount { get; set; }
}
```

### 언제 프로시저를 유지할지 (슬라이드 불릿 추천)

- 다중 조인 + 집계 + 힌트 최적화가 이미 DB에서 안정화된 쿼리
- 감사/정산 등 DBA와 공동 관리되는 핵심 로직
- 동일 로직을 여러 시스템이 공통으로 재사용하는 경우

### 언제 Entity 기반으로 옮길지 (슬라이드 불릿 추천)

- 일반 조회/CRUD와 화면 중심 필터 조합
- 도메인 변경에 맞춰 코드 레벨 리팩터링이 자주 필요한 영역
- 테스트 자동화와 코드 리뷰 가시성이 중요한 영역

## Gemini Canvas 프롬프트 템플릿

### 1차 아웃라인 생성 프롬프트

```text
첨부한 문서를 기준으로 2주차 2회차 EF Core 발표 슬라이드 아웃라인을 작성해줘.
조건:
- 한국어
- Day 2는 총 8장(Part 0 포함)
- Part 0은 Procedure 중심 vs Entity 기반 개념 전환을 다룰 것
- 슬라이드마다 제목, 핵심 메시지 1문장, 본문 불릿 3개 작성
- 과장 없이 실무형 톤
```

### 2차 본문 확장 프롬프트

```text
방금 만든 아웃라인을 바탕으로 각 슬라이드를 확장해줘.
슬라이드별 출력 형식:
1) 제목
2) 본문(불릿 3~5개)
3) 발표자 멘트(2문장)
4) 코드/도식 제안(1개)
제약:
- Procedure 중심 방식과 Entity 기반 방식의 우열 비교로 쓰지 말 것
- 반드시 트레이드오프 관점으로 서술
- "LINQ는 SQL을 잊는 도구가 아니다" 메시지를 슬라이드 0 또는 1에 포함
- "기존 프로시저 패턴도 병행 가능" 메시지와 하이브리드 예시 1개를 포함
```

### 3차 디자인/완성본 정리 프롬프트

```text
지금 결과를 Google Slides 자동 생성용 브리프로 변환해줘.
형식:
- 전체 테마 1줄
- 폰트/색상 가이드 1줄
- 슬라이드별 제목 + 본문 요약(각 3줄 이내)
- 슬라이드별 시각자료 키워드 1개
추가 제약:
- 텍스트 과밀 금지
- 한 슬라이드 핵심 메시지 1개 원칙 유지
```

## 품질 점검 체크리스트 (업로드 전/후)

### 업로드 전

- [ ] [docs/training/handbook/WEEK2_DAY2_EFCORE_DELIVERY_PLAN.md](docs/training/handbook/WEEK2_DAY2_EFCORE_DELIVERY_PLAN.md)의 Part 0 문구가 최신이다.
- [ ] [docs/training/handbook/WEEK2_BACKEND_API_SLIDE_OUTLINE.md](docs/training/handbook/WEEK2_BACKEND_API_SLIDE_OUTLINE.md)에 Day 2 8장 기준이 반영되어 있다.
- [ ] [docs/training/handbook/WEEK2_BACKEND_API_SLIDE_DRAFT.md](docs/training/handbook/WEEK2_BACKEND_API_SLIDE_DRAFT.md)에 슬라이드 0 문구가 있다.

### 생성 후

- [ ] 슬라이드 0에 용어 대조표(ResultSet↔Entity, SQL Mapper↔LINQ 번역기 등)가 포함되어 있다.
- [ ] MyBatis/직접 SQL 예시와 EF Core 예시가 나란히 들어가 있다.
- [ ] 기존 Stored Procedure 패턴을 병행할 수 있다는 하이브리드 예시가 최소 1개 포함되어 있다.
- [ ] ChangeTracker, SaveChanges, AsNoTracking의 필요성이 관점 전환으로 연결된다.
- [ ] 데모 슬라이드가 실제 코드([EsSecrets.Day2/Program.cs](EsSecrets.Day2/Program.cs), [EsSecrets.Day2/Services/SecretService.cs](EsSecrets.Day2/Services/SecretService.cs))와 모순되지 않는다.

## 실패 패턴과 대응

1. 현상: 슬라이드가 일반 ORM 소개로 흐려짐

- 대응: Delivery Plan을 최상단 첨부하고 Part 0 강제 조건을 프롬프트 첫 줄에 둔다.

1. 현상: 코드 없는 추상 설명만 출력됨

- 대응: 코드 파일 5개를 추가 첨부하고 "슬라이드별 코드/도식 제안 1개" 조건을 강제한다.

1. 현상: 장수가 늘어나거나 구조가 바뀜

- 대응: "Day 2 총 8장(Part 0 포함) 고정"을 프롬프트 맨 앞에 재명시한다.

1. 현상: 우열 비교(Procedure는 구식, ORM이 정답) 톤으로 출력됨

- 대응: "우열 금지, 트레이드오프 관점" 제약을 별도 항목으로 재지시한다.

## 최종 산출물 권장 형태

- Canvas 최종 원고
- Google Slides 초안
- PPTX 다운로드본

실무에서는 "Canvas 원고 + PPTX"를 함께 보관하면 이후 재생성/개정이 빠르다.
