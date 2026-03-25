# UI 페이지 생성 계획 (Context 고정용)

본 문서는 [docs/UI.drawio](docs/UI.drawio)의 Main 페이지를 기준 템플릿으로 사용하여, 이후 필요한 화면을 일관되게 생성하기 위한 기준 문서입니다.

용어 기준은 [docs/DOMAIN_KEYWORD_GUIDE.md](docs/DOMAIN_KEYWORD_GUIDE.md)를 우선 적용합니다.

## 1) 목표

- Main의 UI 패턴(헤더/검색/목록/행 액션/모달)을 재사용해 화면 일관성 유지
- Usecase 및 Sequence 우선순위(P1) 기준으로 화면 구현 순서 고정
- 각 페이지마다 정상/예외/권한 상태를 사전에 정의

## 2) 생성 원칙

- 모든 신규 페이지는 Main 탭 스타일을 복제해서 시작
- 파괴적 액션(삭제/권한회수/비활성화)은 반드시 확인 모달 분리
- 민감정보 원문 조회는 반드시 재인증 모달 선행
- 상태 화면 최소 3종 포함: 정상 / 권한오류 / 실패(서버·검증)

## 3) 페이지 생성 우선순위

## 3.1 P1 (핵심)

1. `Login` 고도화
   - 상태: 기본, 입력오류, 인증실패, 잠금, 로딩
2. `Main` 고도화
   - 상태: 검색결과 없음, 권한 없음, 서버 오류, 세션 만료
3. `SecretCreate`
   - 정보 추가 화면
4. `SecretEdit`
   - 정보 수정 + 동시성 충돌 안내
5. `SecretPlainView` + `SessionReauthModal`
   - 재인증 후 원문 노출(제한 시간)
6. `DeleteRequestModal`
   - 삭제요청 사유 입력
7. `DeleteApproveModal`
   - 팀장 승인/반려 + 코멘트
8. `DeleteExecuteConfirm`
   - 승인 상태 확인 후 실제 삭제

## 3.2 P1 확장

1. (보류) 접근권한 화면군
   - `AccessGrantModal`, `AccessRevokeConfirm`, `AccessLogPage`
   - P1 필수 플로우 완료 후 재추가

## 3.3 P2

1. (보류) 직원관리 화면군
   - `MembersPage`, `MemberStatusConfirm`
   - P2 단계에서 진행

## 4) 탭 생성 규칙 (UI.drawio)

- 탭 이름은 아래와 같이 고정
  - `Main` (기준)
  - `SecretCreate`
  - `SecretEdit`
  - `SecretPlainView`
  - `SessionReauthModal`
  - `DeleteRequestModal`
  - `DeleteApproveModal`
  - `DeleteExecuteConfirm`

### 현재 생성 범위 (P1)

- `Login`, `Main`, `SecretCreate`, `SecretEdit`, `SecretPlainView`
- `SessionReauthModal`, `DeleteRequestModal`, `DeleteApproveModal`, `DeleteExecuteConfirm`

### 보류 범위 (P1 확장/P2)

- `AccessGrantModal`, `AccessRevokeConfirm`, `AccessLog`
- `Members`, `MemberStatusConfirm`

## 5) 화면 ↔ 시퀀스 매핑

- 로그인/잠금: [docs/SEQUENCE_CORE_SCENARIOS.md](docs/SEQUENCE_CORE_SCENARIOS.md) 의 1번
- 시크릿 목록 조회/접근기록: 2번
- 원문 조회/세션 재인증: 3번
- 추가/수정: 4번
- 삭제요청: 5번
- 승인/반려: 6번
- 실제 삭제: 7번
- 권한 부여/회수: 8번

## 6) 화면 완료 기준 (DoD)

- 진입 조건, 주요 액션, 성공/실패 후 상태가 다이어그램에 표현됨
- 권한 분기(팀원/팀장)가 컴포넌트 수준에서 식별 가능함
- 모달 사용 흐름(호출 주체/확인/취소)이 명확함
- Usecase, Class, Sequence와 용어가 일치함

## 7) 변경 관리

- 화면 플로우 변경 시 아래 문서를 함께 업데이트
  - [docs/UI.drawio](docs/UI.drawio)
  - [docs/USECASE_IMPLEMENTATION_GUIDE.md](docs/USECASE_IMPLEMENTATION_GUIDE.md)
  - [docs/SEQUENCE_CORE_SCENARIOS.md](docs/SEQUENCE_CORE_SCENARIOS.md)

---

본 문서를 기준으로 이후 UI 탭 생성 및 상세 흐름 확장을 진행한다.
