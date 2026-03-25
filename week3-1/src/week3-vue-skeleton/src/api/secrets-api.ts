export type SecretStatus = 'active' | 'expired' | 'draft';

export interface SecretSummaryDto {
  secretId: number;
  title: string;
  ownerName: string;
  status: SecretStatus;
  maskedValue: string;
  lastModifiedAt: string;
}

export interface SecretDetailDto extends SecretSummaryDto {
  description: string;
  canViewRawValue: boolean;
  rawValue?: string;
}

export interface SearchSecretsRequest {
  keyword?: string;
  status?: SecretStatus;
}

export interface SearchSecretsResponse {
  items: SecretSummaryDto[];
  totalCount: number;
}

export interface CreateSecretRequest {
  title: string;
  ownerName: string;
  secretValue: string;
  description: string;
}

export interface LoginRequest {
  userId: string;
  password: string;
}

export interface LoginResponse {
  displayName: string;
  accessToken: string;
}

const mockSecrets: SecretDetailDto[] = [
  {
    secretId: 1,
    title: 'ERP 운영 계정',
    ownerName: '플랫폼팀',
    status: 'active',
    maskedValue: 'erp-****-prod',
    rawValue: 'erp-prod-token-001',
    lastModifiedAt: '2026-03-20 10:15',
    description: 'ERP 배치 작업에 사용하는 운영 계정',
    canViewRawValue: true
  },
  {
    secretId: 2,
    title: '메일 발송 API Key',
    ownerName: '서비스팀',
    status: 'draft',
    maskedValue: 'mail-****-draft',
    rawValue: 'mail-draft-key-002',
    lastModifiedAt: '2026-03-18 09:00',
    description: '신규 메일 발송 연동 테스트용 키',
    canViewRawValue: false
  },
  {
    secretId: 3,
    title: '정산 DB 읽기 전용 계정',
    ownerName: '재무시스템팀',
    status: 'expired',
    maskedValue: 'settle-****-ro',
    rawValue: 'settle-readonly-003',
    lastModifiedAt: '2026-03-10 14:30',
    description: '정산 리포트 조회용 읽기 전용 계정',
    canViewRawValue: true
  }
];

function delay(milliseconds = 300): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, milliseconds));
}

export class SecretsApi {
  async searchSecrets(request: SearchSecretsRequest): Promise<SearchSecretsResponse> {
    await delay();

    const keyword = request.keyword?.trim().toLowerCase();

    const items = mockSecrets.filter(secret => {
      const matchesKeyword = !keyword || secret.title.toLowerCase().includes(keyword) || secret.ownerName.toLowerCase().includes(keyword);
      const matchesStatus = !request.status || secret.status === request.status;
      return matchesKeyword && matchesStatus;
    });

    return {
      items,
      totalCount: items.length
    };
  }

  async getSecret(secretId: number): Promise<SecretDetailDto> {
    await delay();

    const secret = mockSecrets.find(item => item.secretId === secretId);

    if (!secret) {
      throw new Error('NOT_FOUND');
    }

    if (secretId === 2) {
      throw new Error('FORBIDDEN');
    }

    return secret;
  }

  async createSecret(request: CreateSecretRequest): Promise<SecretDetailDto> {
    await delay();

    const created: SecretDetailDto = {
      secretId: mockSecrets.length + 1,
      title: request.title,
      ownerName: request.ownerName,
      status: 'draft',
      maskedValue: `${request.title.slice(0, 4)}-****`,
      rawValue: request.secretValue,
      lastModifiedAt: '2026-03-23 09:00',
      description: request.description,
      canViewRawValue: false
    };

    mockSecrets.unshift(created);
    return created;
  }
}

export class AuthApi {
  async login(request: LoginRequest): Promise<LoginResponse> {
    await delay(200);

    if (!request.userId.trim() || !request.password.trim()) {
      throw new Error('INVALID_CREDENTIALS');
    }

    return {
      displayName: '교육생',
      accessToken: 'training-access-token'
    };
  }
}

export const secretsApi = new SecretsApi();
export const authApi = new AuthApi();
