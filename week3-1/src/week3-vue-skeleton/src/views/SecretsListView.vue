<template>
  <section class="page">
    <header>
      <h2>Secrets 목록</h2>
      <p class="description">검색 조건과 목록 결과를 분리해서 다루는 예제다.</p>
    </header>

    <div class="filters">
      <input v-model="filters.keyword" type="text" placeholder="제목 또는 담당 팀 검색" />

      <select v-model="filters.status">
        <option value="all">전체</option>
        <option value="active">active</option>
        <option value="expired">expired</option>
        <option value="draft">draft</option>
      </select>

      <button @click="loadItems" :disabled="isLoading">
        {{ isLoading ? '조회 중...' : '조회' }}
      </button>
    </div>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p v-if="!isLoading && !errorMessage" class="count">총 {{ filteredItems.length }}건</p>
    <p v-if="!isLoading && filteredItems.length === 0" class="empty">검색 결과가 없습니다.</p>

    <table v-if="filteredItems.length > 0" class="table">
      <thead>
        <tr>
          <th>ID</th>
          <th>제목</th>
          <th>담당</th>
          <th>상태</th>
          <th>마스킹 값</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in filteredItems" :key="item.secretId">
          <td>{{ item.secretId }}</td>
          <td>{{ item.title }}</td>
          <td>{{ item.ownerName }}</td>
          <td>{{ item.status }}</td>
          <td>{{ item.maskedValue }}</td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script>
import { secretsApi } from '../api/secrets-api';

export default {
  name: 'SecretsListView',
  data() {
    return {
      items: [],
      isLoading: false,
      errorMessage: '',
      filters: {
        keyword: '',
        status: 'all'
      }
    };
  },
  computed: {
    filteredItems() {
      return this.items.filter(item => {
        const keyword = this.filters.keyword.trim().toLowerCase();
        const matchesKeyword = !keyword || item.title.toLowerCase().includes(keyword) || item.ownerName.toLowerCase().includes(keyword);
        const matchesStatus = this.filters.status === 'all' || item.status === this.filters.status;
        return matchesKeyword && matchesStatus;
      });
    }
  },
  mounted() {
    this.loadItems();
  },
  methods: {
    async loadItems() {
      this.isLoading = true;
      this.errorMessage = '';

      try {
        const response = await secretsApi.searchSecrets({
          keyword: this.filters.keyword,
          status: this.filters.status === 'all' ? undefined : this.filters.status
        });
        this.items = response.items;
      } catch {
        this.errorMessage = '목록을 불러오지 못했습니다.';
      } finally {
        this.isLoading = false;
      }
    }
  }
};
</script>

<style scoped>
.page {
  display: grid;
  gap: 16px;
}

.filters {
  display: flex;
  gap: 8px;
}

.table {
  border-collapse: collapse;
  width: 100%;
}

.table th,
.table td {
  border: 1px solid #d1d5db;
  padding: 8px;
  text-align: left;
}

.description,
.count,
.empty {
  color: #4b5563;
}

.error {
  color: #b91c1c;
}
</style>
