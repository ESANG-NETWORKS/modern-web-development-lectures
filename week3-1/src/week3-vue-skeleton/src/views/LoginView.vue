<template>
  <section class="panel login-panel">
    <h2>로그인</h2>
    <p class="description">입력값 검증과 저장 중 상태를 가장 단순하게 보여주는 예제다.</p>

    <label>
      사용자 ID
      <input v-model="form.userId" type="text" placeholder="user01" />
    </label>
    <p v-if="validationErrors.userId" class="error">{{ validationErrors.userId }}</p>

    <label>
      비밀번호
      <input v-model="form.password" type="password" placeholder="password" />
    </label>
    <p v-if="validationErrors.password" class="error">{{ validationErrors.password }}</p>

    <button @click="submitLogin" :disabled="isSubmitting">
      {{ isSubmitting ? '로그인 중...' : '로그인' }}
    </button>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p v-if="loginMessage" class="success">{{ loginMessage }}</p>
  </section>
</template>

<script>
import { authApi } from '../api/secrets-api';

export default {
  name: 'LoginView',
  data() {
    return {
      form: {
        userId: '',
        password: ''
      },
      isSubmitting: false,
      errorMessage: '',
      loginMessage: '',
      validationErrors: {
        userId: '',
        password: ''
      }
    };
  },
  methods: {
    validateForm() {
      this.validationErrors.userId = this.form.userId.trim() ? '' : '사용자 ID를 입력하세요.';
      this.validationErrors.password = this.form.password.trim() ? '' : '비밀번호를 입력하세요.';
      return !this.validationErrors.userId && !this.validationErrors.password;
    },
    async submitLogin() {
      this.errorMessage = '';
      this.loginMessage = '';

      if (!this.validateForm()) {
        return;
      }

      this.isSubmitting = true;

      try {
        const response = await authApi.login(this.form);
        this.loginMessage = `${response.displayName}님, 환영합니다.`;
      } catch {
        this.errorMessage = '로그인에 실패했습니다.';
      } finally {
        this.isSubmitting = false;
      }
    }
  }
};
</script>

<style scoped>
.panel {
  display: grid;
  gap: 12px;
  max-width: 360px;
}

.description {
  color: #4b5563;
}

.error {
  color: #b91c1c;
}

.success {
  color: #047857;
}
</style>
