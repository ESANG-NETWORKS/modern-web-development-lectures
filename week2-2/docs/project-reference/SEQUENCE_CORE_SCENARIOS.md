# ESSecrets 핵심 Sequence 시나리오 (우선순위)

본 문서는 팀이 반드시 먼저 맞춰야 하는 핵심 기능 8개의 시퀀스 초안입니다.  
아래 블록은 그대로 `Mermaid` 렌더러에 붙여 다이어그램으로 변환 가능합니다.

용어 기준은 [도메인/키워드 가이드](DOMAIN_KEYWORD_GUIDE.md)를 우선 적용합니다.

## 공통 참여자 표기

- `User`: 팀원/팀장
- `UI`: Vue 화면
- `API`: ASP.NET Core Controller
- `Service`: `AuthAppService` 또는 `SecretAppService`
- `DB`: EF Core + DB
- `Audit`: `AuditLog` 기록 처리

---

## 1) 로그인 / 잠금 처리

```mermaid
sequenceDiagram
actor User
participant UI
participant API
participant Service as AuthAppService
participant DB
participant Audit
User->>UI: Submit login form
UI->>API: POST /auth/login
API->>Service: Login(fmsId,password)
Service->>DB: Load user by fmsId
alt Invalid password
Service->>DB: Increase fail count and lock if needed
Service->>Audit: Write LOGIN_FAIL
API-->>UI: 401 Unauthorized
else Success
Service->>DB: Reset fail count
Service->>Audit: Write LOGIN_SUCCESS
API-->>UI: 200 Token
end
```

## 2) 시크릿 목록 조회 (권한 필터 포함)

```mermaid
sequenceDiagram
actor User
participant UI
participant API
participant Service as SecretAppService
participant DB
participant Audit
User->>UI: Enter search filter
UI->>API: GET /secrets
API->>Service: SearchSecrets(criteria,actor)
Service->>DB: Query by permission and filter
Service->>Audit: Write LIST_VIEW
API-->>UI: 200 List with masked fields
```

## 3) 민감정보 원문 조회 (세션 재인증 포함)

```mermaid
sequenceDiagram
actor User
participant UI
participant API
participant Auth as AuthAppService
participant Secret as SecretAppService
participant DB
participant Audit
User->>UI: Click view plain secret
UI->>API: POST /auth/reauth
API->>Auth: Reauthenticate(userId,password)
alt Reauth failed
API-->>UI: 401 Unauthorized
else Reauth success
UI->>API: GET /secrets/{id}/plain
API->>Secret: GetPlainSecret(secretId,actor)
Secret->>DB: Verify access and decrypt
Secret->>Audit: Write DETAIL_VIEW
API-->>UI: 200 Plain secret with short TTL
end
```

## 4) 정보 추가/수정

```mermaid
sequenceDiagram
actor User
participant UI
participant API
participant Service as SecretAppService
participant DB
participant Audit
User->>UI: Click save
UI->>API: POST or PUT /secrets
API->>Service: CreateOrUpdateSecret
Service->>DB: Validate and persist with concurrency check
alt Concurrency conflict
API-->>UI: 409 Conflict
else Success
Service->>Audit: Write ADD or UPDATE
API-->>UI: 200 or 201
end
```

## 5) 삭제요청 생성

```mermaid
sequenceDiagram
actor User
participant UI
participant API
participant Service as SecretAppService
participant DB
participant Audit
User->>UI: Enter delete request reason
UI->>API: POST /secrets/{id}/delete-requests
API->>Service: RequestDelete(secretId,reason)
Service->>DB: Create DeleteRequest Pending
Service->>Audit: Write DELETE_REQUEST
API-->>UI: 201 Created
```

## 6) 삭제요청 승인/반려

```mermaid
sequenceDiagram
actor Leader
participant UI
participant API
participant Service as SecretAppService
participant DB
participant Audit
Leader->>UI: Review delete request
UI->>API: POST /delete-requests/{id}/approve or reject
API->>Service: ApproveDelete or RejectDelete
Service->>DB: Verify leader role and update status
Service->>Audit: Write DELETE_APPROVE or DELETE_REJECT
API-->>UI: 200 Result
```

## 7) 실제 삭제 실행 (Soft Delete)

```mermaid
sequenceDiagram
actor Leader
participant UI
participant API
participant Service as SecretAppService
participant DB
participant Audit
Leader->>UI: Execute delete
UI->>API: DELETE /secrets/{id}
API->>Service: ExecuteDelete(secretId)
Service->>DB: Verify approved request
alt Not approved
API-->>UI: 400 or 403
else Approved
Service->>DB: Set Secret.IsDeleted true
Service->>Audit: Write DELETE_EXECUTE
API-->>UI: 200 OK
end
```

## 8) 정보 접근권한 부여/회수

```mermaid
sequenceDiagram
actor Leader
participant UI
participant API
participant Service as SecretAppService
participant DB
participant Audit
Leader->>UI: Change access permission
UI->>API: POST or DELETE /secrets/{id}/accesses
API->>Service: GrantAccess or RevokeAccess
Service->>DB: Verify leader role and update SecretAccess
Service->>Audit: Write GRANT or REVOKE
API-->>UI: 200 OK
```

---

## 필수 예외 분기(모든 시퀀스 공통)

- 권한 없음: `403`
- 인증/세션 문제: `401`
- 대상 없음/삭제됨: `404`
- 동시성 충돌: `409`
- 유효성 오류: `400`
