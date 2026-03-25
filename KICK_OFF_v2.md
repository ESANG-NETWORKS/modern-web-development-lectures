# 사내 Secrets 관리 시스템 개발 프로젝트 (KICK-OFF v2)

## 📋 프로젝트 개요

### 목적

1. **실용적인 도구 개발**: 계정, 암호, API KEY 등 민감 정보를 안전하게 관리하는 사내 웹 애플리케이션 구축
2. **기술 스택 전환 경험**: WebForms/jQuery 기반의 레거시 환경을 **ASP.NET Core + Vue.js** 모던 아키텍처로 전환하는 노하우 습득
3. **협업 문화 정착**: Git Flow, 코드 리뷰, 페어 프로그래밍 등 선진 개발 문화 체험

### 진행 방식의 변화 (Core Concept)

* **기획은 PM이, 구현은 팀이**: 기획과 UI 설계에 소요되는 시간을 최소화하고, 팀원들은 **'기술적 구현'**과 **'새로운 툴 적응'**에 집중합니다.
* **AI Pair Programming**: 모르는 문법이나 기술은 AI(Copilot/Cursor)를 적극 활용하여 학습 곡선을 단축합니다.

---

## 👥 팀 구성 및 역할 분담 (R&R)

### 프로젝트 리더 & PM (14년차)

* **핵심 역할**: 요구사항 정의, 화면 UI/UX 기획(Wireframe), 핵심 비즈니스 로직 설계
* **멘토링**: 기술적 난제 해결 가이드, 아키텍처 방향성 제시, 최종 산출물 품질 검수

### 개발팀 (3명)

* **윤영현 (Tech Lead)**
  * 백엔드 아키텍처(ASP.NET Core) 수립 및 DB 연결 구조 설계
  * Git 브랜치 전략 관리 및 코드 병합(Merge) 승인
  * 비동기 프로그래밍(Async/Await) 및 LINQ 가이드

* **김경은 (Frontend Core)**
  * PM의 기획을 바탕으로 Vue.js 컴포넌트 구조화 및 화면 구현
  * DB 스키마 설계 지원 및 API 명세 구체화
  * 프론트엔드 상태 관리 및 데이터 바인딩 구현

* **나성민 (AI & Testing)**
  * AI 도구를 활용한 기본 코드(Boilerplate) 생성 및 단위 기능 구현
  * Swagger/OpenAPI 연동 확인 및 API 테스트
  * Git 충돌 해결 학습 및 테스트 코드(Unit Test) 작성

---

## 🎯 학습 및 문화 목표

### 기술적 목표

* **VS Code + AI Agent** 기반의 현대적 개발 환경 적응
* **ASP.NET Core Web API** 설계 및 **OpenAPI/Swagger** 자동화 이해
* **Vue.js** 기초 및 컴포넌트 기반 개발 방식 습득
* **Git**을 활용한 안전하고 체계적인 형상 관리

### 협업 문화 목표

* **코드 리뷰의 일상화**: "검사"가 아닌 "공유"를 위한 주 1회 정기 리뷰
* **페어 프로그래밍**: 난이도가 높은 구간(초기 세팅, 인증 로직 등)은 화면을 공유하며 함께 코딩
* **실패 용인**: Git 충돌이나 버그 발생을 두려워하지 않고, 해결 과정을 통해 학습

---

## 🛠 기술 스택 & 개발 환경

* **Backend**: ASP.NET Core 10, EF Core (Database First), Swagger, JWT
* **Frontend**: Vue 3 (Options API), Vite, PrimeVue, Axios
* **Tools**:
* **IDE**: Visual Studio Code (필수 확장: C# Dev Kit, Vue - Official, GitGraph, GitHub Copilot)
* **Collaboration**: Git (GitHub/GitLab), Postman

---

## 🔄 개발 워크플로우 (Sprint Process)

PM님의 기획을 바탕으로 효율적으로 개발을 진행하는 순환 프로세스입니다.

1. **Design (PM)**: 기능 명세서, 화면 와이어프레임, UI/UX 가이드 전달
2. **Dev Spec (Tech Lead)**: 구현을 위한 API 명세 정의, DB 스키마 확정 (Swagger 초안 작성)
3. **Implement (Dev Team)**:

* AI를 활용하여 코드 초안 생성 (백엔드/프론트엔드)
* 기능 구현 및 단위 테스트 진행

1. **Code Review (전체)**:

* **1차 (Tech Lead)**: 코드 스타일, 로직 적절성, 구조적 문제 검토
* **2차 (PM)**: 요구사항 부합 여부, 사용자 경험(UX), 비즈니스 로직 검증

1. **Merge & Deploy**: 리뷰 통과 시 Main 브랜치 병합 및 배포

---

## 📅 4주 상세 일정

### 1주차: 기획 리뷰 & 개발 환경 워크숍 (Onboarding)

**목표**: PM의 기획 의도를 완벽히 이해하고, "개발 가능한" 환경과 규칙을 확정한다.

> 📘 상세 교육안: [docs/training/handbook/1주차_교육가이드.md](training/handbook/1주차_교육가이드.md)

* **기획 리뷰 미팅 (PM 주관)**
* 요구사항 명세서 및 와이어프레임 설명
* DB 스키마(ERD) 초안 리뷰

* **Git 협업 그라운드 룰 확정 (필수)**
* 브랜치 전략: `main` - `develop` - `feature/기능명`
* 커밋 메시지 규칙 정하기
* **[실습] Git 시뮬레이션**: PR 보내기, 리뷰하기, **충돌(Conflict) 해결해보기** (성민님 필수 참여)

* **Tech Spike (기술 검증)**
* **영현**: 기존 DB 연결 및 EF Core 조회 테스트
* **경은**: Vue.js 프로젝트 생성 및 '메인 화면' 레이아웃 잡아보기
* **성민**: AI 도구 설치 및 "Hello World" API 생성 테스트

### 2주차: 백엔드 API 개발 & OpenAPI 구축

**목표**: ASP.NET Core Web API 구축 및 프론트엔드 연동 준비

* **주요 활동**
* 1일차: `Program.cs`, DI, lifetime, middleware, controller 문법, OpenAPI 기반 ASP.NET Core 내부 구조 설명
* 2일차: `DbContext`, `ChangeTracker`, LINQ, 성능 구문, SQL 확인, 로깅 기반 EF Core 내부 동작 설명
* 실습은 기능 완성보다 런타임 메커니즘 이해와 디버깅 감각 확보에 집중
* 세부 운영 기준: [2주차 교육 세부 계획](../../training/handbook/WEEK2_BACKEND_API_TRAINING_PLAN.md)

* **[함께하는 시간]**
* 목요일: 백엔드 코드 리뷰 및 구조 점검

### 3주차: 프론트엔드 개발 (Vue.js)

**목표**: AI의 도움을 받아 jQuery 사고방식을 Vue 컴포넌트 방식으로 전환

* **주요 활동**
* OpenAPI Generator로 API 클라이언트 자동 생성 (TypeScript/Axios)
* 화면별 컴포넌트 구현 (로그인, 목록, 상세, 등록)
* **AI 활용 포인트**: *"jQuery로 작성된 이 로직을 Vue 3 Options API 방식으로 변환해줘."*
* 세부 운영 기준: [3주차 프론트엔드 교육 세부 계획](../../training/handbook/WEEK3_FRONTEND_VUE_TRAINING_PLAN.md)

* **[함께하는 시간]**
* 수요일: 페어 프로그래밍 (프론트엔드 데이터 바인딩 난관 해결)

### 4주차: 통합 테스트 & 배포

**목표**: 전체 기능 연동 확인 및 최종 산출물 완성

* **주요 활동**
* 프론트엔드-백엔드 연동 테스트
* 버그 수정 및 UI 폴리싱 (PM 피드백 반영)
* 배포 시나리오 점검 (IIS 또는 Docker)

* **프로젝트 회고**
* KPT (Keep, Problem, Try) 회고 진행

---

## 💡 AI 에이전트 활용 가이드 (PM's Tip)

### 추천 프롬프트 패턴

#### 1. 레거시 코드 전환 (WebForms → Vue/Core)

> "PM님이 작성하신 이 WebForms 로직(C#)을 분석해서,
>
> 1. ASP.NET Core 서비스 레이어에 맞게 리팩토링해주고,
> 2. Vue.js 컴포넌트에서 호출할 수 있는 로직으로 변환해줘."
>
>

#### 2. Vue.js 초보 탈출

> "나는 jQuery에는 익숙하지만 Vue는 처음이야.
> `$('#btnSave').click(function() { ... })` 이 코드를
> Vue 3의 methods와 v-on 디렉티브를 사용한 코드로 바꿔주고, 원리를 설명해줘."

#### 3. API 클라이언트 활용

> "OpenAPI Generator로 생성된 `secrets-api.ts` 파일을 사용해서
> 비밀번호 목록을 불러오는 Vue 메서드를 작성해줘. async/await를 사용해."

---

## 🚀 첫 번째 미션 (킥오프 전 준비)

1. **VS Code 설치**: [다운로드 링크]
2. **확장 프로그램 설치**: C# Dev Kit, Vue - Official, GitGraph, GitHub Copilot
3. **마음가짐**: "모르는 건 죄가 아니다. AI에게 물어보고, 동료에게 물어보자."

**우리는 '완벽한 제품'보다 '완벽한 팀워크'와 '성장'을 목표로 합니다.** 🚀
