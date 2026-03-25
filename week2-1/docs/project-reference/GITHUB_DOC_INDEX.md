# ESSecrets 문서 인덱스 (GitHub 온보딩용)

처음 합류한 참여자가 **어떤 문서를 어떤 순서로** 보면 되는지 안내하는 허브 문서입니다.

## 1) 10분 빠른 진입

1. [README](../README.md)
2. [도메인/키워드 가이드](DOMAIN_KEYWORD_GUIDE.md)
3. [Usecase 반영 지시서](USECASE_IMPLEMENTATION_GUIDE.md)
4. [UML 다이어그램](UML.drawio)
5. [Class Diagram 지시서 (경량)](CLASS_DIAGRAM_IMPLEMENTATION_GUIDE.md)
6. [핵심 Sequence 시나리오](SEQUENCE_CORE_SCENARIOS.md)

## 2) 역할별 권장 동선

### PM / 기획

- [도메인/키워드 가이드](DOMAIN_KEYWORD_GUIDE.md)
- [Usecase 반영 지시서](USECASE_IMPLEMENTATION_GUIDE.md)
- [핵심 Sequence 시나리오](SEQUENCE_CORE_SCENARIOS.md)

### Backend

- [도메인/키워드 가이드](DOMAIN_KEYWORD_GUIDE.md)
- [Class Diagram 지시서 (경량)](CLASS_DIAGRAM_IMPLEMENTATION_GUIDE.md)
- [핵심 Sequence 시나리오](SEQUENCE_CORE_SCENARIOS.md)
- [Usecase 반영 지시서](USECASE_IMPLEMENTATION_GUIDE.md)

### Frontend

- [도메인/키워드 가이드](DOMAIN_KEYWORD_GUIDE.md)
- [UI 설계](UI.drawio)
- [핵심 Sequence 시나리오](SEQUENCE_CORE_SCENARIOS.md)
- [Usecase 반영 지시서](USECASE_IMPLEMENTATION_GUIDE.md)

## 3) 구현 우선순위 링크

- P1-1 로그인/잠금: [시퀀스 1](SEQUENCE_CORE_SCENARIOS.md#1-로그인--잠금-처리)
- P1-2 시크릿 목록 조회/접근기록: [시퀀스 2](SEQUENCE_CORE_SCENARIOS.md#2-정보-목록-조회-권한-필터-포함)
- P1-3 원문 조회/세션 재인증: [시퀀스 3](SEQUENCE_CORE_SCENARIOS.md#3-민감정보-원문-조회-재인증-포함)
- P1-4 추가/수정: [시퀀스 4](SEQUENCE_CORE_SCENARIOS.md#4-정보-추가수정)
- P1-5 삭제요청: [시퀀스 5](SEQUENCE_CORE_SCENARIOS.md#5-삭제-요청-생성)
- P1-6 승인/반려: [시퀀스 6](SEQUENCE_CORE_SCENARIOS.md#6-삭제-요청-승인반려)
- P1-7 실제 삭제: [시퀀스 7](SEQUENCE_CORE_SCENARIOS.md#7-실제-삭제-실행-soft-delete)
- P1-8 권한 부여/회수: [시퀀스 8](SEQUENCE_CORE_SCENARIOS.md#8-접근권한-부여회수)

## 4) 주차별 운영 계획

- [2주차 교육 세부 계획](../../training/handbook/WEEK2_BACKEND_API_TRAINING_PLAN.md)
- [Day 1 CLI 실습 시나리오](../../training/handbook/WEEK2_DAY1_ASPNETCORE_CLI_SCENARIO.md)
- [Day 2 CLI 실습 시나리오](../../training/handbook/WEEK2_DAY2_EFCORE_CLI_SCENARIO.md)
- [2주차 슬라이드 아웃라인](../../training/handbook/WEEK2_BACKEND_API_SLIDE_OUTLINE.md)
- [2주차 발표 자료 초안](../../training/handbook/WEEK2_BACKEND_API_SLIDE_DRAFT.md)
- [3주차 프론트엔드 교육 세부 계획](../../training/handbook/WEEK3_FRONTEND_VUE_TRAINING_PLAN.md)
- [3주차 프론트엔드 슬라이드 아웃라인](../../training/handbook/WEEK3_FRONTEND_VUE_SLIDE_OUTLINE.md)
- [3주차 프론트엔드 발표 자료 초안](../../training/handbook/WEEK3_FRONTEND_VUE_SLIDE_DRAFT.md)
- [3주차 Gemini PPT 작성 가이드](../../training/handbook/WEEK3_FRONTEND_VUE_GEMINI_CANVAS_PPT_GUIDE.md)
- [3주차 Vue 예제 스켈레톤](../../training/labs/week3-vue-skeleton/README.md)

## 5) 리뷰 체크포인트

- Usecase 용어와 Class 명칭이 일치하는가
- 시퀀스의 예외 분기(`401/403/404/409`)가 API 스펙에 반영됐는가
- 삭제요청/승인/실행이 분리되어 있는가
- 접근기록(AuditLog) 기록 포인트가 누락되지 않았는가
- 용어가 [도메인/키워드 가이드](DOMAIN_KEYWORD_GUIDE.md)와 일치하는가
