<script setup>
import { ref } from 'vue'

const newTodoItem = ref('')
const updateItemTargets = ref([])

const todoList = ref([
  {
    text: '할 일 1',
    completed: false,
  },
  {
    text: '할 일 2',
    completed: true,
  },
])

function addTodoList() {
  todoList.value.push({
    text: newTodoItem.value,
    completed: false,
  })

  // 입력창 초기화
  newTodoItem.value = ''
}

function removeTodoList(index) {
  todoList.value.splice(index, 1)
}

function modifyTodoItem(index) {
  if (!updateItemTargets.value.includes(index)) {
    updateItemTargets.value.push(index)
  }
}

function confirmModifyTodo(index) {
  updateItemTargets.value.splice(updateItemTargets.value.indexOf(index), 1)
}

function completeTodoClass(item) {
  return {
    'line-through': item.completed,
  }
}
</script>

<template>
  <h1 class="text-3xl font-bold underline">Todo List</h1>

  <div class="w-96">
    <InputGroup>
      <InputText
        class="w-full"
        size="small"
        placeholder="Username"
        v-model="newTodoItem"
        @keyup.enter="addTodoList"
      />
      <Button class="grow-0 whitespace-nowrap" size="small" @click="addTodoList">등록</Button>
    </InputGroup>

    <div class="pt-3">
      <ul class="list-none space-y-1">
        <li class="flex flex-col" v-for="(item, i) in todoList" :key="i">
          <div class="flex justify-between">
            <div class="flex items-center">
              <ToggleSwitch class="mr-2" size="small" v-model="item.completed" />
              <span v-if="!updateItemTargets.includes(i)" :class="completeTodoClass(item)">{{
                item.text
              }}</span>
              <InputGroup v-else>
                <InputText size="small" v-model="item.text" @keyup.enter="confirmModifyTodo(i)" />
                <Button icon="pi pi-save" size="small" @click="confirmModifyTodo(i)" />
              </InputGroup>
            </div>

            <div class="flex items-center gap-1">
              <Button
                icon="pi pi-pen-to-square"
                size="small"
                @click="modifyTodoItem(i)"
                :disabled="updateItemTargets.includes(i)"
              />
              <Button
                icon="pi pi-trash"
                size="small"
                @click="removeTodoList(i)"
                :disabled="updateItemTargets.includes(i)"
              />
            </div>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<style scoped></style>
