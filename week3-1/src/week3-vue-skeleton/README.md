# 3주차 Vue 교육용 예제 스켈레톤

본 폴더는 3주차 프론트엔드 교육에서 사용하는 Vue 3 Options API 예제 스켈레톤이다.

## 목적

- jQuery 사고방식에서 상태 기반 컴포넌트 사고방식으로 전환하는 예제를 제공한다.
- OpenAPI Generator로 생성된 클라이언트를 어떻게 화면 상태에 연결하는지 보여준다.
- 로그인, 목록, 상세, 등록 화면의 최소 구조를 한 번에 살펴볼 수 있게 한다.

## 파일 구성

- [src/api/secrets-api.ts](src/api/secrets-api.ts): 생성된 API 클라이언트를 흉내 낸 교육용 mock
- [src/views/LoginView.vue](src/views/LoginView.vue): 로그인 입력/검증 예제
- [src/views/SecretsListView.vue](src/views/SecretsListView.vue): 목록 조회/필터 예제
- [src/views/SecretDetailView.vue](src/views/SecretDetailView.vue): 상세 조회/오류 분기 예제
- [src/views/SecretCreateView.vue](src/views/SecretCreateView.vue): 등록 저장/validation 예제

## 사용 방식

- 실제 실행 가능한 완성 앱이 아니라 강의용 구조 예제다.
- Options API에서 `data`, `computed`, `methods`, `mounted`가 어디에 배치되는지 보여주는 데 집중한다.
- API 호출은 실제 Axios 대신 mock Promise로 구성되어 있어, 화면 상태 흐름 설명에 바로 사용할 수 있다.

## 강의 연결 포인트

- 1회차: [src/views/LoginView.vue](src/views/LoginView.vue), [src/views/SecretCreateView.vue](src/views/SecretCreateView.vue)
- 2회차: [src/views/SecretsListView.vue](src/views/SecretsListView.vue), [src/views/SecretDetailView.vue](src/views/SecretDetailView.vue), [src/api/secrets-api.ts](src/api/secrets-api.ts)
