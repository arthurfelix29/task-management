<script setup lang="ts">
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useTasksStore } from '@/features/tasks/stores/tasks.store'
import SearchBar from '@/features/tasks/components/SearchBar.vue'
import CreateTaskForm from '@/features/tasks/components/CreateTaskForm.vue'
import TaskListItems from '@/features/tasks/components/TaskListItems.vue'
import TaskFilters from '@/features/tasks/components/TaskFilters.vue'
import TaskCountFooter from '@/features/tasks/components/TaskCountFooter.vue'
import EmptyState from '@/features/tasks/components/EmptyState.vue'
import ErrorState from '@/features/tasks/components/ErrorState.vue'
import LoadingState from '@/features/tasks/components/LoadingState.vue'

const store = useTasksStore()
const { filter, viewState, completedCount, totalCount } = storeToRefs(store)

onMounted(() => {
  void store.loadAll()
})
</script>

<template>
  <section class="space-y-8" :aria-busy="store.loading">
    <SearchBar />

    <div class="space-y-4">
      <CreateTaskForm />

      <div class="flex flex-wrap items-center justify-between gap-2">
        <TaskFilters :model-value="filter" @update:model-value="store.setFilter($event)" />
        <TaskCountFooter :completed="completedCount" :total="totalCount" />
      </div>

      <LoadingState v-if="viewState.kind === 'loading'" />
      <ErrorState
        v-else-if="viewState.kind === 'error'"
        :message="viewState.message"
        :can-retry="viewState.canRetry"
        @retry="store.loadAll()"
      />
      <EmptyState v-else-if="viewState.kind === 'empty'" :filter="viewState.filter" />
      <TaskListItems
        v-else
        :tasks="viewState.tasks"
        @toggle="store.toggle($event)"
        @remove="store.remove($event)"
      />
    </div>
  </section>
</template>
