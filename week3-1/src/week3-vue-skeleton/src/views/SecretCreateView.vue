<template>
  <section class="page">
    <h2>Secret 등록</h2>
    <p class="description">입력 상태와 저장 상태를 분리하는 예제다.</p>

    <label>
      제목
      <input v-model="form.title" type="text" />
    </label>
    <p v-if="validationErrors.title" class="error">{{ validationErrors.title }}</p>

    <label>
      담당 팀
      <input v-model="form.ownerName" type="text" />
    </label>
    <p v-if="validationErrors.ownerName" class="error">{{ validationErrors.ownerName }}</p>

    <label>
      Secret 값
      <input v-model="form.secretValue" type="password" />
    </label>
    <p v-if="validationErrors.secretValue" class="error">{{ validationErrors.secretValue }}</p>

    <label>
      설명
      <textarea v-model="form.description" rows="4"></textarea>
    </label>

    <button @click="submit" :disabled="isSaving">
      {{ isSaving ? '저장 중...' : '저장' }}
    </button>

    <p v-if="saveMessage" class="success">{{ saveMessage }}</p>
  </section>
</template>

<script>
import { secretsApi } from '../api/secrets-api';

function createEmptyForm() {
  return {
    title: '',
    ownerName: '',
    secretValue: '',
    description: ''
  };
}

function createValidationErrors() {
  return {
    title: '',
    ownerName: '',
    secretValue: ''
  };
}

export default {
  name: 'SecretCreateView',
  data() {
    return {
      form: createEmptyForm(),
      isSaving: false,
      saveMessage: '',
      validationErrors: createValidationErrors()
    };
  },
  methods: {
    validate() {
      this.validationErrors = createValidationErrors();

      if (!this.form.title.trim()) {
        this.validationErrors.title = '제목은 필수입니다.';
      }

      if (!this.form.ownerName.trim()) {
        this.validationErrors.ownerName = '담당 팀은 필수입니다.';
      }

      if (!this.form.secretValue.trim()) {
        this.validationErrors.secretValue = 'Secret 값은 필수입니다.';
      }

      return !this.validationErrors.title && !this.validationErrors.ownerName && !this.validationErrors.secretValue;
    },
    async submit() {
      this.saveMessage = '';

      if (!this.validate()) {
        return;
      }

      this.isSaving = true;

      try {
        const created = await secretsApi.createSecret(this.form);
        this.saveMessage = `${created.title} 항목이 저장되었습니다.`;
        this.form = createEmptyForm();
      } finally {
        this.isSaving = false;
      }
    }
  }
};
</script>

<style scoped>
.page {
  display: grid;
  gap: 12px;
  max-width: 480px;
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
