# ESSecrets 도메인/키워드 관리 가이드

본 문서는 `docs` 전반의 용어 정합성을 유지하기 위한 기준 문서입니다.

## 1) 목적

- 도메인 용어의 **단일 기준(SSOT)** 제공
- 문서/화면/API/클래스 간 표현 불일치 최소화
- 신규 문서 작성 시 용어 선택 기준 명확화

## 2) 적용 범위

- 대상: `docs/*.md`, `docs/*.drawio`
- 우선순위: **본 문서 > Usecase/Sequence/Class/UI 계획 문서 > 기타 회의/아이디어 문서**

## 3) 핵심 용어 사전 (표준)

| 도메인 개념 | 표준 용어(권장) | 허용 표기 | 비권장/대체 용어 |
|---|---|---|---|
| 보호 대상 정보 | 시크릿(민감정보) | Secret | 정보, 비밀정보 |
| 목록 조회 | 시크릿 목록 조회 | 목록 조회 | 정보조회 |
| 상세 민감값 확인 | 원문 조회(평문) | 원문 조회 | 평문 조회(단독), 상세조회 |
| 조회 이벤트 기록 | 접근기록 | AuditLog, 감사로그 | 조회로그, 조회기록 |
| 삭제 요청 | 삭제요청 | DeleteRequest 생성 | 제거요청 |
| 삭제 승인/반려 | 삭제 승인/반려 | 승인/반려 | 제거 승인/반려 |
| 실제 삭제 실행 | 실제 삭제(Soft Delete) | 삭제 실행 | 실제 제거, 제거 실행 |
| 정보 단위 권한 | 정보 접근권한 관리 | 접근권한 관리 | 권한관리(단독) |
| 직원 단위 권한 | 직원 계정 권한 관리 | 계정 권한 관리 | 권한관리(단독) |
| 재인증 | 세션 재인증 | 비밀번호 재확인 | 재로그인(오해 소지) |

## 4) 네이밍 규칙

### 4.1 UI/화면

- 탭/페이지명은 영어 PascalCase 유지
  - 예: `SecretPlainView`, `DeleteRequestModal`
- 사용자 노출 문구는 한글 표준 용어 우선
  - 예: "삭제요청", "접근기록", "원문 조회"

### 4.2 클래스/엔티티

- 엔티티명은 영어 도메인 명사 유지
  - `Secret`, `SecretAccess`, `DeleteRequest`, `AuditLog`
- 상태값은 Enum으로 고정
  - `Pending`, `Approved`, `Rejected`

### 4.3 API/이벤트

- API 설명 문구는 "삭제" 계열 용어로 통일
- 감사 이벤트 타입은 `AuditLog.actionType` 기준 유지
  - `LIST_VIEW`, `DETAIL_VIEW`, `DELETE_REQUEST`, `DELETE_APPROVE`, `DELETE_EXECUTE` 등

## 5) 문서 작성 체크리스트

- "제거"를 "삭제"로 통일했는가
- "조회로그/조회기록"을 "접근기록"으로 통일했는가
- "정보조회"를 "시크릿 목록 조회" 또는 "목록 조회"로 표현했는가
- "권한관리" 단독 표현 대신 범주를 명시했는가
  - 정보 접근권한 관리 / 직원 계정 권한 관리
- 원문 조회 맥락에서 "세션 재인증" 선행이 명시되었는가

## 6) 변경 관리 규칙

용어 변경이 발생하면 아래 문서를 함께 갱신한다.

- `docs/USECASE_IMPLEMENTATION_GUIDE.md`
- `docs/SEQUENCE_CORE_SCENARIOS.md`
- `docs/CLASS_DIAGRAM_IMPLEMENTATION_GUIDE.md`
- `docs/UI_PAGE_CREATION_PLAN.md`
- `docs/GITHUB_DOC_INDEX.md`

---

본 가이드는 도메인 모델/화면 흐름/시퀀스 구현의 용어 기준으로 사용한다.
