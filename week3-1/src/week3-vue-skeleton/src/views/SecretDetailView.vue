<template>
  <section class="page">
    <h2>Secret 상세</h2>

    <div class="toolbar">
      <input v-model.number="secretId" type="number" min="1" />
      <button @click="loadSecret" :disabled="isLoading">
        {{ isLoading ? '조회 중...' : '상세 조회' }}
      </button>
    </div>

    <p v-if="notFound" class="error">대상을 찾을 수 없습니다.</p>
    <p v-else-if="forbidden" class="error">원문 조회 권한이 없습니다.</p>

    <article v-else-if="item" class="card">
      <h3>{{ item.title }}</h3>
      <p>담당: {{ item.ownerName }}</p>
      <p>상태: {{ item.status }}</p>
      <p>설명: {{ item.description }}</p>
      <p>표시 값: {{ displayedValue }}</p>
      <button @click="toggleReveal" :disabled="!item.canViewRawValue">
        {{ revealRawValue ? '마스킹으로 전환' : '원문 보기' }}
      </button>
    </article>
  </section>
</template>

<script>
import { secretsApi } from '../api/secrets-api';

export default {
  name: 'SecretDetailView',
  data() {
    return {
      secretId: 1,
      item: null,
      isLoading: false,
      notFound: false,
      forbidden: false,
      revealRawValue: false
    };
  },
  computed: {
    displayedValue() {
      if (!this.item) {
        return '';
      }

      if (this.revealRawValue && this.item.canViewRawValue) {
        return this.item.rawValue ?? this.item.maskedValue;
      }

      return this.item.maskedValue;
    }
  },
  mounted() {
    this.loadSecret();
  },
  methods: {
    async loadSecret() {
      this.isLoading = true;
      this.notFound = false;
      this.forbidden = false;
      this.item = null;
      this.revealRawValue = false;

      try {
        this.item = await secretsApi.getSecret(this.secretId);
      } catch (error) {
        const code = error instanceof Error ? error.message : '';
        this.notFound = code === 'NOT_FOUND';
        this.forbidden = code === 'FORBIDDEN';
      } finally {
        this.isLoading = false;
      }
    },
    toggleReveal() {
      if (!this.item?.canViewRawValue) {
        return;
      }

      this.revealRawValue = !this.revealRawValue;
    }
  }
};
</script>

<style scoped>
.page {
  display: grid;
  gap: 16px;
}

.toolbar {
  display: flex;
  gap: 8px;
}

.card {
  border: 1px solid #d1d5db;
  padding: 16px;
}

.error {
  color: #b91c1c;
}
</style>
