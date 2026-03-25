# ESSecrets Class Diagram 지시서 (경량 버전)

본 문서는 ASP.NET Core 기준으로 **과한 구조화를 피하고**, 프로젝트 핵심 기능에 필요한 클래스만 유지하기 위한 지시서입니다.

용어 기준은 [도메인/키워드 가이드](DOMAIN_KEYWORD_GUIDE.md)를 우선 적용합니다.

## 1) 설계 원칙

- Controller → Application Service → Entity(EF Core) 흐름을 기본으로 한다.
- 서비스는 `AuthAppService`, `SecretAppService` 두 개로 제한한다.
- 세부 정책은 서비스 내부 메서드로 구현하고, 서비스 클래스를 과도하게 분리하지 않는다.
- 접근기록(AuditLog)은 별도 서비스 분리 대신 핵심 유스케이스 메서드에서 직접 기록한다.

## 2) 핵심 클래스

## 2.1 Entity

- `User`
  - 계정/역할/활성상태/로그인 잠금 관리
- `Secret`
  - 민감정보(암호화 비밀번호), 소유자, Soft Delete 상태 관리
- `SecretAccess`
  - 사용자별 정보 접근 권한(조회/수정/관리) 관리
- `DeleteRequest`
  - 삭제요청, 승인/반려 상태 관리
- `AuditLog`
  - 행위 접근기록(누가, 무엇을, 결과)

## 2.2 Enum

- `RoleType`: `Member`, `Leader`
- `DeleteRequestStatus`: `Pending`, `Approved`, `Rejected`

## 2.3 Application Service

- `AuthAppService`
  - 로그인, 로그아웃, 원문 조회 전 세션 재인증
- `SecretAppService`
  - 목록 조회/추가/수정/삭제요청/승인/반려/삭제 실행/권한 부여/권한 회수/접근기록 조회

## 3) Usecase 매핑

- 로그인/로그아웃 → `AuthAppService`
- 시크릿 목록 조회(접근기록 생성 포함) → `SecretAppService`
- 민감정보 원문 조회(세션 재인증 포함) → `AuthAppService` + `SecretAppService`
- 정보추가/변경/권한부여/권한회수 → `SecretAppService`
- 정보삭제 요청/승인/반려/실행 → `SecretAppService` + `DeleteRequest`

## 4) 구현 필수 규칙

- 비밀번호 평문 저장 금지 (`Secret.EncryptedPassword`)
- 팀원 직접 삭제 금지 (항상 403)
- 승인되지 않은 건 삭제 실행 금지
- 목록 조회/추가/수정/삭제요청/승인/삭제/로그인/로그아웃 접근기록 필수
- 삭제는 Soft Delete 기본

## 5) DB 매핑(최소)

- `Users` ↔ `User`
- `Secrets` ↔ `Secret`
- `SecretAccesses` ↔ `SecretAccess`
- `DeleteRequests` ↔ `DeleteRequest`
- `AuditLogs` ↔ `AuditLog`

---

Class Diagram 변경 시, 클래스 추가 전에 “핵심 기능을 위해 꼭 필요한가”를 먼저 검토한다.
