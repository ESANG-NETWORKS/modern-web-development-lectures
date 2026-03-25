# 사내 Secrets 관리 시스템 개발 프로젝트

## 📋 프로젝트 개요

### 목적

1. **실용적인 도구 개발**: 계정, 암호, API KEY 등 민감 정보를 안전하게 관리하는 사내 웹 애플리케이션
2. **최신 개발 방법론 학습**: AI 에이전트를 활용한 현대적 개발 방식 체험
3. **기술 스택 업그레이드**: ASP.NET WebForms → ASP.NET Core + Vue.js 전환 경험

### 왜 이 프로젝트인가?

- 실제로 필요한 도구를 만들면서 배운다
- 회사 내부 데이터를 안전하게 다루는 방법 습득
- 레거시 프로젝트 모던화에 대한 인사이트 확보

---

## 🎯 학습 목표

### 기술적 목표

- **VSCode + AI Agent** 기반 개발 환경 구축
- **ASP.NET Core Web API** RESTful 서비스 개발
- **OpenAPI/Swagger** 자동 API 문서화 및 클라이언트 코드 생성
- **Vue.js** 프론트엔드 프레임워크 기초
- **Git** 브랜치 전략 및 협업 워크플로우
- **EF Core** ORM 활용

### AI 활용 목표

- 자연어로 요구사항 전달 → 코드 생성
- 기존 코드 리팩토링 및 개선
- 테스트 코드 자동 생성
- 에러 해결 및 디버깅 지원

---

## 🛠 기술 스택

### 백엔드

- **ASP.NET Core 8** (Web API)
- **Entity Framework Core** (Database First)
- **SQL Server** (기존 SSMS 활용)
- **Swashbuckle (Swagger)** (API 문서화)
- **JWT** 인증

### 프론트엔드

- **Vue 3** (Options API, JavaScript)
- **Vite** (빌드 도구)
- **PrimeVue** (UI 컴포넌트 라이브러리)
- **Axios** (HTTP 클라이언트)
- **OpenAPI Generator** (타입 안전한 API 클라이언트 자동 생성)

### 개발 도구

- **Visual Studio Code**
- **GitHub Copilot** 또는 **Cursor**
- **Git** (버전 관리)
- **Postman** (API 테스트)
- **OpenAPI Generator CLI** (코드 생성)

---

## 📅 4주 개발 일정

### 1주차: 기획 & 환경 설정

**목표**: 프로젝트 이해 및 개발 환경 구축

#### 주요 활동

- 요구사항 명세서 작성
  - 어떤 정보를 저장할 것인가?
  - 누가 접근할 수 있는가?
  - 어떤 보안 요구사항이 있는가?

- DB 스키마 설계
  - SSMS를 활용한 테이블 설계
  - AI에게 스키마 리뷰 요청

- 개발 환경 세팅 워크샵
  - VSCode 설치 및 필수 확장 프로그램
  - .NET 8 SDK 설치
  - Node.js 및 npm 설치
  - Git 기본 설정
  - OpenAPI Generator CLI 설치

#### AI 활용 실습

```
프롬프트 예시:
"비밀번호, API 키, 서비스 계정 정보를 저장하는 
Secrets 관리 시스템의 DB 스키마를 설계해줘.
사용자별 접근 권한과 감사 로그도 포함해야 해."
```

#### 산출물

- 요구사항 명세서
- DB ERD
- 개발 환경 체크리스트

---

### 2주차: 백엔드 API 개발

**목표**: ASP.NET Core Web API 구축 및 OpenAPI 통합

#### WebForms vs ASP.NET Core 비교

| 개념 | WebForms | ASP.NET Core API |
|------|----------|------------------|
| 아키텍처 | Stateful (ViewState) | Stateless (RESTful) |
| 데이터 전달 | Postback | JSON HTTP 요청/응답 |
| UI 로직 | Code-behind | 프론트엔드 분리 |
| 데이터 접근 | ADO.NET, DataSet | EF Core, LINQ |
| API 문서화 | 수동 워드/엑셀 | Swagger 자동 생성 |

#### 주요 개발 내용

**1. 프로젝트 구조 생성**

```bash
dotnet new webapi -n SecretsManager
cd SecretsManager
```

**2. Entity 클래스 작성**

- Secret, User, AuditLog 모델
- AI로 Entity 클래스 초안 생성

**3. EF Core 설정**

- Database First 접근 (기존 DB 활용)
- DbContext 구성

```bash
# EF Core 도구 설치
dotnet tool install --global dotnet-ef

# 기존 DB에서 모델 생성
dotnet ef dbcontext scaffold "Server=localhost;Database=SecretsDB;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models
```

**4. CRUD API 구현**

- `GET /api/secrets` - 목록 조회
- `GET /api/secrets/{id}` - 상세 조회
- `POST /api/secrets` - 생성
- `PUT /api/secrets/{id}` - 수정
- `DELETE /api/secrets/{id}` - 삭제

**5. OpenAPI (Swagger) 설정** ⭐

**Program.cs 설정:**

```csharp
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger 서비스 등록
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Secrets Manager API",
        Version = "v1",
        Description = "사내 비밀정보 관리 시스템 API",
        Contact = new OpenApiContact
        {
            Name = "개발팀",
            Email = "dev@company.com"
        }
    });

    // XML 주석 파일 경로 설정 (API 설명 자동화)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // JWT 인증 추가
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. 예: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 개발 환경에서 Swagger 활성화
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Secrets Manager API v1");
        options.RoutePrefix = string.Empty; // 루트 경로에서 Swagger UI 표시
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

**프로젝트 파일(.csproj)에 XML 문서 생성 활성화:**

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

**API 컨트롤러에 문서화 주석 추가:**

```csharp
/// <summary>
/// 비밀정보 관리 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SecretsController : ControllerBase
{
    /// <summary>
    /// 모든 비밀정보 목록 조회
    /// </summary>
    /// <returns>비밀정보 목록</returns>
    /// <response code="200">조회 성공</response>
    /// <response code="401">인증 실패</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SecretDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<SecretDto>>> GetSecrets()
    {
        // 구현
    }

    /// <summary>
    /// 새로운 비밀정보 생성
    /// </summary>
    /// <param name="createDto">생성할 비밀정보 데이터</param>
    /// <returns>생성된 비밀정보</returns>
    /// <remarks>
    /// 샘플 요청:
    ///
    ///     POST /api/secrets
    ///     {
    ///        "name": "Production DB",
    ///        "username": "admin",
    ///        "password": "P@ssw0rd123",
    ///        "url": "https://db.company.com",
    ///        "description": "운영 데이터베이스 접속 정보"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">생성 성공</response>
    /// <response code="400">잘못된 입력 데이터</response>
    [HttpPost]
    [ProducesResponseType(typeof(SecretDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SecretDto>> CreateSecret(CreateSecretDto createDto)
    {
        // 구현
    }
}
```

**DTO 클래스 예시:**

```csharp
/// <summary>
/// 비밀정보 생성 요청 DTO
/// </summary>
public class CreateSecretDto
{
    /// <summary>
    /// 비밀정보 이름 (예: "운영 DB", "AWS API Key")
    /// </summary>
    /// <example>Production Database</example>
    [Required(ErrorMessage = "이름은 필수입니다")]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }

    /// <summary>
    /// 사용자명 또는 계정 ID
    /// </summary>
    /// <example>admin</example>
    [StringLength(100)]
    public string? Username { get; set; }

    /// <summary>
    /// 비밀번호 또는 API 키 (저장 시 자동 암호화됨)
    /// </summary>
    /// <example>P@ssw0rd123</example>
    [Required(ErrorMessage = "비밀번호는 필수입니다")]
    [StringLength(500)]
    public string Password { get; set; }

    /// <summary>
    /// 관련 URL
    /// </summary>
    /// <example>https://db.company.com</example>
    [Url]
    [StringLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// 비밀정보에 대한 설명
    /// </summary>
    /// <example>운영 데이터베이스 마스터 계정</example>
    [StringLength(1000)]
    public string? Description { get; set; }
}
```

**6. 보안 기능**

- 비밀번호 AES 암호화
- JWT 인증
- 접근 권한 검증

**7. OpenAPI JSON 스펙 생성**

```bash
# 프로젝트 실행
dotnet run

# 브라우저에서 접속
# https://localhost:5001 (Swagger UI)
# https://localhost:5001/swagger/v1/swagger.json (OpenAPI 스펙)
```

#### AI 활용 패턴

```
패턴 1: 코드 생성
"Secret 엔티티에 대한 CRUD API 컨트롤러를 만들어줘.
EF Core를 사용하고, async/await 패턴을 적용해줘."

패턴 2: Swagger 문서화
"이 컨트롤러 메서드에 Swagger XML 주석을 추가해줘.
- 메서드 설명
- 파라미터 설명
- 응답 코드별 설명 (200, 400, 404)
- 요청 예제 샘플
포함해서 작성해줘."

패턴 3: 기능 추가
"이 컨트롤러에 JWT 인증을 추가하고,
사용자가 자신의 Secret만 볼 수 있도록 필터링해줘."
```

#### 실습: Swagger UI 사용하기

1. **Swagger UI 열기**: `https://localhost:5001`
2. **API 엔드포인트 확인**: 자동 생성된 모든 API 목록 확인
3. **Try it out**: 브라우저에서 바로 API 테스트
4. **JWT 인증 테스트**:
   - POST `/api/auth/login`으로 로그인
   - 응답에서 토큰 복사
   - "Authorize" 버튼 클릭
   - `Bearer {토큰}` 입력
   - 인증이 필요한 API 호출

#### 산출물

- 동작하는 Web API
- Swagger UI 문서
- OpenAPI JSON 스펙 (`swagger.json`)
- 단위 테스트 코드

---

### 3주차: 프론트엔드 개발 + OpenAPI 클라이언트 자동 생성

**목표**: Vue.js 웹 애플리케이션 구축 및 타입 안전한 API 통신

#### 백엔드-프론트엔드 연계 전략 ⭐

**기존 방식 (수동)**

```javascript
// API 호출 코드를 매번 수동으로 작성
async function getSecrets() {
  try {
    const response = await axios.get('/api/secrets');
    return response.data;
  } catch (error) {
    console.error(error);
  }
}

async function createSecret(data) {
  try {
    const response = await axios.post('/api/secrets', data);
    return response.data;
  } catch (error) {
    console.error(error);
  }
}
// 모든 API마다 반복...
```

**새로운 방식 (OpenAPI Generator 활용)** ✨

```javascript
// 1. OpenAPI 스펙에서 자동으로 클라이언트 코드 생성
// 2. 타입 안전한 함수 자동 생성
// 3. API 변경 시 재생성만 하면 끝!

import { SecretsApi } from '@/api/generated';

const api = new SecretsApi();

// 자동 완성과 타입 체크 지원!
const secrets = await api.getSecrets();
const newSecret = await api.createSecret({
  name: 'Production DB',
  username: 'admin',
  password: 'P@ssw0rd123'
});
```

#### OpenAPI Generator CLI 설정

**1. OpenAPI Generator 설치**

```bash
# npm 전역 설치
npm install -g @openapitools/openapi-generator-cli

# 또는 프로젝트별 설치
npm install --save-dev @openapitools/openapi-generator-cli
```

**2. Vue 프로젝트에 설정 파일 추가**

**openapitools.json** (프로젝트 루트)

```json
{
  "$schema": "node_modules/@openapitools/openapi-generator-cli/config.schema.json",
  "spaces": 2,
  "generator-cli": {
    "version": "7.2.0"
  }
}
```

**package.json에 스크립트 추가**

```json
{
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "generate-api": "openapi-generator-cli generate -i http://localhost:5001/swagger/v1/swagger.json -g typescript-axios -o src/api/generated --additional-properties=withSeparateModelsAndApi=true,apiPackage=api,modelPackage=models",
    "generate-api:file": "openapi-generator-cli generate -i ./swagger.json -g typescript-axios -o src/api/generated --additional-properties=withSeparateModelsAndApi=true,apiPackage=api,modelPackage=models"
  }
}
```

**옵션 설명:**

- `-i`: OpenAPI 스펙 파일 경로 (URL 또는 로컬 파일)
- `-g`: 생성기 타입 (`typescript-axios` - TypeScript + Axios 클라이언트)
- `-o`: 출력 디렉토리
- `withSeparateModelsAndApi=true`: API와 모델을 별도 파일로 분리
- `apiPackage=api`: API 클래스 패키지명
- `modelPackage=models`: 모델 클래스 패키지명

**3. API 클라이언트 생성 워크플로우**

```bash
# 백엔드 API 실행
cd SecretsManager
dotnet run

# 새 터미널에서 프론트엔드로 이동
cd secrets-ui

# OpenAPI 스펙에서 클라이언트 코드 생성
npm run generate-api

# 생성된 파일 확인
# src/api/generated/
#   ├── api/
#   │   ├── secrets-api.ts
#   │   └── auth-api.ts
#   ├── models/
#   │   ├── secret-dto.ts
#   │   ├── create-secret-dto.ts
#   │   └── ...
#   ├── base.ts
#   ├── common.ts
#   ├── configuration.ts
#   └── index.ts
```

**4. 생성된 API 클라이언트 사용**

**API 설정 파일 생성** (`src/api/config.ts`)

```javascript
import { Configuration } from './generated';

// API 기본 설정
export const apiConfig = new Configuration({
  basePath: 'https://localhost:5001',
  baseOptions: {
    headers: {
      'Content-Type': 'application/json',
    },
  },
});

// JWT 토큰 추가 함수
export function setAuthToken(token) {
  apiConfig.accessToken = token;
}
```

**Vue 컴포넌트에서 사용**

```javascript
<template>
  <div>
    <h2>비밀정보 목록</h2>
    <ul>
      <li v-for="secret in secrets" :key="secret.id">
        {{ secret.name }} - {{ secret.username }}
        <button @click="copyPassword(secret.id)">복사</button>
      </li>
    </ul>
  </div>
</template>

<script>
import { SecretsApi } from '@/api/generated';
import { apiConfig } from '@/api/config';

export default {
  name: 'SecretList',
  data() {
    return {
      secrets: [],
      secretsApi: new SecretsApi(apiConfig)
    };
  },
  async mounted() {
    await this.loadSecrets();
  },
  methods: {
    async loadSecrets() {
      try {
        // 타입 안전한 API 호출!
        const response = await this.secretsApi.getSecrets();
        this.secrets = response.data;
      } catch (error) {
        console.error('비밀정보 로드 실패:', error);
      }
    },
    async copyPassword(secretId) {
      try {
        const response = await this.secretsApi.getSecret(secretId);
        await navigator.clipboard.writeText(response.data.password);
        alert('비밀번호가 복사되었습니다!');
      } catch (error) {
        console.error('복사 실패:', error);
      }
    }
  }
};
</script>
```

**비밀정보 생성 폼**

```javascript
<template>
  <form @submit.prevent="handleSubmit">
    <div>
      <label>이름:</label>
      <input v-model="form.name" required />
    </div>
    <div>
      <label>사용자명:</label>
      <input v-model="form.username" />
    </div>
    <div>
      <label>비밀번호:</label>
      <input v-model="form.password" type="password" required />
    </div>
    <div>
      <label>URL:</label>
      <input v-model="form.url" type="url" />
    </div>
    <div>
      <label>설명:</label>
      <textarea v-model="form.description"></textarea>
    </div>
    <button type="submit">저장</button>
  </form>
</template>

<script>
import { SecretsApi } from '@/api/generated';
import { apiConfig } from '@/api/config';

export default {
  name: 'SecretForm',
  data() {
    return {
      form: {
        name: '',
        username: '',
        password: '',
        url: '',
        description: ''
      },
      secretsApi: new SecretsApi(apiConfig)
    };
  },
  methods: {
    async handleSubmit() {
      try {
        // 생성된 타입을 그대로 사용!
        await this.secretsApi.createSecret(this.form);
        alert('저장되었습니다!');
        this.$router.push('/secrets');
      } catch (error) {
        console.error('저장 실패:', error);
        alert('저장에 실패했습니다.');
      }
    }
  }
};
</script>
```

#### jQuery vs Vue.js 비교

**기존 방식 (jQuery)**

```javascript
$('#btnLoad').click(function() {
  $.ajax({
    url: '/api/secrets',
    success: function(data) {
      var html = '';
      data.forEach(function(item) {
        html += '<tr><td>' + item.name + '</td></tr>';
      });
      $('#secretList').html(html);
    }
  });
});
```

**새로운 방식 (Vue.js + 생성된 API 클라이언트)**

```javascript
export default {
  data() {
    return {
      secrets: [],
      api: new SecretsApi(apiConfig)
    }
  },
  methods: {
    async loadSecrets() {
      const response = await this.api.getSecrets();
      this.secrets = response.data;
    }
  },
  mounted() {
    this.loadSecrets();
  }
}
```

```html
<template>
  <tr v-for="secret in secrets" :key="secret.id">
    <td>{{ secret.name }}</td>
  </tr>
</template>
```

#### 개발 워크플로우 ⭐

**1. 백엔드 개발자 작업**

```bash
# 1. API 엔드포인트 추가/수정
# 2. XML 주석으로 문서화
# 3. 백엔드 실행
dotnet run
# 4. Swagger에서 확인
# https://localhost:5001
```

**2. 프론트엔드 개발자 작업**

```bash
# 1. API 클라이언트 재생성
npm run generate-api

# 2. 새로운 API 함수 자동 완성으로 확인
# 3. 타입 체크 받으면서 개발
# 4. 컴파일 에러로 API 변경사항 즉시 파악
```

**백엔드 API 변경 시 프론트엔드 자동 동기화!**

- API 엔드포인트 추가 → `npm run generate-api` → 새 함수 자동 생성
- 요청/응답 모델 변경 → 재생성 → TypeScript 타입 오류로 즉시 파악
- API 삭제 → 재생성 → 사용 중인 코드에서 컴파일 에러 발생

#### AI 활용 실습

```
프롬프트 1: Vue 컴포넌트 생성
"Vue 3 컴포넌트를 만들어줘.
생성된 SecretsApi를 사용해서 비밀정보 목록을 테이블로 표시해.
각 행에 복사 버튼도 추가해줘."

프롬프트 2: 에러 처리 추가
"이 API 호출 코드에 에러 핸들링을 추가해줘.
네트워크 에러, 401 인증 에러, 404 에러를 구분해서 처리해야 해."

프롬프트 3: 폼 validation
"이 Secret 생성 폼에 validation을 추가해줘.
- 이름: 필수, 1-100자
- 비밀번호: 필수, 강도 체크
- URL: 유효한 URL 형식"
```

#### 주요 개발 내용

1. **Vue 프로젝트 초기화**

```bash
npm create vite@latest secrets-ui -- --template vue
cd secrets-ui
npm install
npm install axios
npm install primevue primeicons
```

1. **주요 컴포넌트 개발**
   - 로그인 폼 (JWT 토큰 획득)
   - Secret 목록 (PrimeVue DataTable)
   - Secret 추가/수정 폼
   - Secret 상세보기 (복사 기능)

2. **생성된 API 클라이언트 활용**
   - 타입 안전한 API 호출
   - 자동 완성 지원
   - API 변경사항 자동 반영

#### 단계별 학습

1. **Vue 기초**: 템플릿 문법, 데이터 바인딩
2. **이벤트 처리**: v-on, methods
3. **조건부 렌더링**: v-if, v-show
4. **리스트 렌더링**: v-for
5. **폼 입력**: v-model
6. **HTTP 통신**: 생성된 API 클라이언트 사용
7. **OpenAPI 클라이언트 재생성**: API 변경 대응

#### 산출물

- 동작하는 웹 UI
- 반응형 디자인
- 자동 생성된 API 클라이언트 코드
- 타입 안전한 API 통신

---

### 4주차: 통합 & 배포

**목표**: 프로덕션 준비 및 배포

#### 주요 활동

**1. 보안 강화**

- HTTPS 설정
- CORS 정책 설정

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:5173") // Vite 기본 포트
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

- Rate Limiting
- 입력 데이터 검증

**2. 배포 준비**

- 환경별 설정 (Development, Production)
- 연결 문자열 암호화
- 로깅 설정

**3. 프론트엔드 빌드**

```bash
# 프로덕션 빌드
npm run build

# dist 폴더에 정적 파일 생성
# 백엔드에서 정적 파일 서빙 또는 별도 웹서버 사용
```

**4. 배포 옵션**

- **옵션 A**: Azure App Service
- **옵션 B**: 사내 IIS 서버
  - 백엔드: IIS에 ASP.NET Core 호스팅
  - 프론트엔드: IIS에 정적 파일 배포 또는 백엔드에서 서빙
- **옵션 C**: Docker 컨테이너

**5. CI/CD 파이프라인** (선택)

```yaml
# .github/workflows/deploy.yml
name: Deploy

on:
  push:
    branches: [ main ]

jobs:
  build-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Build
        run: dotnet build
      - name: Test
        run: dotnet test

  build-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      - name: Install dependencies
        run: npm ci
        working-directory: ./secrets-ui
      - name: Generate API Client
        run: npm run generate-api
        working-directory: ./secrets-ui
      - name: Build
        run: npm run build
        working-directory: ./secrets-ui
```

#### AI 활용

```
"이 ASP.NET Core 앱을 Docker 컨테이너로 만들어줘.
Dockerfile과 docker-compose.yml을 작성해줘.
프론트엔드 정적 파일도 함께 서빙해야 해."

"GitHub Actions 워크플로우를 만들어줘.
백엔드 빌드 → 프론트엔드 API 클라이언트 재생성 → 프론트엔드 빌드
순서로 실행되어야 해."

"이 코드에서 보안 취약점을 찾아줘."
```

#### 문서화

- API 문서 (Swagger 활용)
- 사용자 가이드
- 운영 매뉴얼
- **OpenAPI 스펙 파일 버전 관리**

#### 산출물

- 배포된 애플리케이션
- 완성된 문서
- 회고 및 개선점

---

## 💡 AI 에이전트 활용 전략

### 효과적인 프롬프트 작성법

#### 1. 컨텍스트 제공

```
❌ 나쁜 예:
"API 만들어줘"

✅ 좋은 예:
"ASP.NET Core 8 Web API 프로젝트야.
SQL Server를 사용하고 EF Core로 연결돼 있어.
Secret 엔티티에 대한 CRUD API 컨트롤러를 만들어줘.
JWT 인증이 필요하고, Swagger 문서화도 포함해야 해.
사용자는 자신의 Secret만 볼 수 있어야 해."
```

#### 2. 점진적 개선

```
1단계: "기본 기능 먼저"
→ "Secret API 컨트롤러를 만들어줘"

2단계: "문서화 추가"
→ "여기에 Swagger XML 주석을 추가해줘"

3단계: "보안 추가"
→ "여기에 JWT 인증을 추가해줘"

4단계: "테스트 작성"
→ "이 컨트롤러에 대한 단위 테스트를 작성해줘"
```

#### 3. 에러 해결

```
"이 에러가 발생했어:
[에러 메시지 붙여넣기]

현재 코드:
[관련 코드 붙여넣기]

OpenAPI Generator로 클라이언트를 생성했는데 이 에러가 나. 어떻게 해결할 수 있을까?"
```

### AI 도구 활용 시나리오

| 작업 | AI 활용 방법 | 기대 효과 |
|------|-------------|----------|
| 새 기능 개발 | 요구사항을 자연어로 설명 → 코드 생성 | 70% 시간 단축 |
| Swagger 문서화 | AI에게 XML 주석 생성 요청 | 문서 작성 자동화 |
| API 클라이언트 생성 | OpenAPI 스펙 → 자동 코드 생성 | 100% 자동화 |
| 코드 리뷰 | AI에게 코드 품질 검토 요청 | 버그 조기 발견 |
| 테스트 작성 | 함수 선택 → 테스트 코드 자동 생성 | 테스트 커버리지 향상 |
| 디버깅 | 에러 메시지 입력 → 해결 방법 제시 | 문제 해결 속도 3배 향상 |

---

## 🔄 OpenAPI 기반 개발 워크플로우 정리

### 전체 흐름

```
1. 백엔드 개발
   ├─ API 엔드포인트 작성
   ├─ XML 주석으로 문서화
   └─ Swagger에서 확인
      ↓
2. OpenAPI 스펙 생성
   └─ https://localhost:5001/swagger/v1/swagger.json
      ↓
3. 프론트엔드 API 클라이언트 생성
   ├─ npm run generate-api
   └─ 타입 안전한 함수 자동 생성
      ↓
4. 프론트엔드 개발
   ├─ 생성된 API 사용
   ├─ 자동 완성 지원
   └─ 타입 체크
      ↓
5. API 변경 시
   ├─ 백엔드 수정
   ├─ npm run generate-api (재생성)
   └─ TypeScript 에러로 변경사항 파악
```

### 장점 정리

✅ **타입 안전성**: TypeScript로 컴파일 타임에 에러 감지  
✅ **자동 동기화**: API 변경 시 재생성만 하면 끝  
✅ **개발 속도**: 자동 완성으로 빠른 개발  
✅ **문서화**: Swagger UI로 실시간 API 문서  
✅ **협업**: 백엔드-프론트엔드 인터페이스 명확화  
✅ **유지보수**: API 스펙이 단일 진실 공급원(Single Source of Truth)

---

## 🎓 학습 자료

### 공식 문서

- [ASP.NET Core 문서](https://learn.microsoft.com/aspnet/core)
- [Vue.js 공식 가이드](https://vuejs.org/guide/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [OpenAPI Specification](https://swagger.io/specification/)
- [OpenAPI Generator](https://openapi-generator.tech/)

### 추천 학습 경로

1. **ASP.NET Core 기초** (Microsoft Learn)
2. **Swagger/OpenAPI** (Swagger 공식 문서)
3. **Vue.js 입문 가이드** (공식 튜토리얼)
4. **OpenAPI Generator 사용법** (공식 문서)
5. **AI 페어 프로그래밍** (GitHub Copilot 문서)

### 참고 프로젝트

- [eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb) - ASP.NET Core 샘플
- [Vue 3 Examples](https://vuejs.org/examples/) - Vue 공식 예제
- [OpenAPI Generator Samples](https://github.com/OpenAPITools/openapi-generator) - 코드 생성 예제

---

## 🚀 첫 번째 세션 준비사항

### 사전 설치 필요

- [ ] Visual Studio Code
- [ ] .NET 8 SDK
- [ ] Node.js (LTS 버전, 18.x 이상)
- [ ] Git
- [ ] SQL Server Management Studio (이미 설치됨)
- [ ] OpenAPI Generator CLI (`npm install -g @openapitools/openapi-generator-cli`)

### 계정 준비

- [ ] GitHub 계정 (Git 협업용)
- [ ] GitHub Copilot 또는 Cursor 라이선스 (회사 제공)

### 마음가짐

- ✅ 새로운 방식에 대한 열린 자세
- ✅ AI는 도구일 뿐, 개발자의 판단이 중요
- ✅ 자동 생성된 코드도 리뷰 필요
- ✅ 실수는 학습의 일부
- ✅ 서로 질문하고 공유하는 문화

---

## 📊 성공 지표

### 프로젝트 성공

- [ ] 실제 사용 가능한 Secrets 관리 시스템 완성
- [ ] 팀원 전원이 개발에 기여
- [ ] OpenAPI 기반 백엔드-프론트엔드 자동 연계 구현
- [ ] 코드 리뷰 문화 정착

### 학습 성공

- [ ] VSCode + AI 도구로 개발 가능
- [ ] Git 브랜치 전략 이해 및 적용
- [ ] Swagger로 API 문서화 가능
- [ ] OpenAPI Generator로 클라이언트 코드 자동 생성 가능
- [ ] 30분 내 간단한 CRUD 기능 추가 가능
- [ ] API 변경 시 프론트엔드 자동 동기화 구현 가능
- [ ] WebForms 프로젝트를 모던 스택으로 전환할 자신감 확보

### 장기 목표

- [ ] 레거시 프로젝트 모던화 계획 수립
- [ ] AI 도구를 일상 개발에 통합
- [ ] OpenAPI 기반 개발을 팀 표준으로 채택
- [ ] 팀 전체 생산성 30% 향상

---

## ❓ FAQ

### Q: AI가 모든 걸 다 해주나요?

**A**: 아니요. AI는 코드 초안을 빠르게 생성하는 도구입니다. 개발자는 생성된 코드를 리뷰하고, 비즈니스 로직을 검증하고, 보안을 확인해야 합니다.

### Q: OpenAPI Generator가 생성한 코드를 수정해도 되나요?

**A**: 생성된 코드는 수정하지 않는 것이 원칙입니다. API가 변경되면 재생성하기 때문에 수정 내용이 사라집니다. 대신 Wrapper 클래스를 만들어 확장하세요.

### Q: TypeScript를 꼭 배워야 하나요?

**A**: 이번 프로젝트에서는 JavaScript만 사용하지만, OpenAPI Generator는 TypeScript 클라이언트를 생성합니다. TypeScript의 기본 개념(타입, 인터페이스)만 이해하면 충분합니다.

### Q: WebForms 프로젝트는 어떻게 되나요?

**A**: 당장 교체하지 않습니다. 이 프로젝트는 향후 마이그레이션을 위한 학습과 실험입니다.

### Q: 4주 안에 완성할 수 있을까요?

**A**: 핵심 기능 위주로 MVP(Minimum Viable Product)를 만듭니다. 완벽함보다는 학습과 경험이 목표입니다.

### Q: API가 변경되면 프론트엔드는 어떻게 하나요?

**A**: `npm run generate-api`로 클라이언트 코드를 재생성하면 됩니다. TypeScript 컴파일 에러로 변경사항을 즉시 파악할 수 있습니다.

### Q: Swagger UI와 Postman 중 뭘 쓰나요?

**A**: 둘 다 유용합니다. Swagger UI는 빠른 확인과 문서화에, Postman은 복잡한 시나리오 테스트에 사용합니다.

---

## 📞 연락 및 지원

- **프로젝트 리드**: [담당자 이름]
- **주간 미팅**: 매주 [요일] [시간]
- **질문/논의**: [Slack 채널 or Teams 채널]
- **코드 저장소**: [GitHub Repository URL]

---

## 🎯 다음 단계

1. **개발 환경 설치** (각자 진행)
   - [ ] VSCode, .NET 8, Node.js, Git 설치
   - [ ] OpenAPI Generator CLI 설치
   - [ ] GitHub Copilot/Cursor 설정

2. **사전 설문 작성** (마감: [날짜])
   - 현재 스킬 레벨 파악
   - 학습 목표 확인
   - 일정 조율

3. **킥오프 미팅** (1주차 월요일)
   - 요구사항 브레인스토밍
   - 팀 역할 분담
   - Git 저장소 설정

4. **첫 번째 실습** (1주차 수요일)
   - AI로 "Hello World" API 만들기
   - Swagger UI 확인
   - OpenAPI Generator로 클라이언트 생성해보기
   - Vue로 간단한 페이지 만들기

---

**함께 만들어가는 프로젝트입니다. 질문과 의견은 언제든 환영합니다!** 🚀
