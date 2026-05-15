import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { tasksApi } from '@/features/tasks/composables/useTasksApi'
import type { Task, TaskFilter } from '@/features/tasks/types/task'
import { type ApiError, describeError } from '@/shared/lib/problem-details'

export type ViewState =
  | { kind: 'loading' }
  | { kind: 'error'; message: string; canRetry: boolean }
  | { kind: 'empty'; filter: TaskFilter }
  | { kind: 'success'; tasks: Task[] }

export class TaskValidationError extends Error {
  constructor(public readonly fieldErrors: Record<string, string[]>) {
    super('Validation failed')
    this.name = 'TaskValidationError'
  }
}

export const useTasksStore = defineStore('tasks', () => {
  const tasks = ref<Task[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const filter = ref<TaskFilter>('all')
  const loaded = ref(false)

  const filteredTasks = computed(() => {
    if (filter.value === 'all') return tasks.value
    const targetCompleted = filter.value === 'completed'
    return tasks.value.filter((t) => t.isCompleted === targetCompleted)
  })

  const completedCount = computed(() => tasks.value.filter((t) => t.isCompleted).length)
  const totalCount = computed(() => tasks.value.length)

  const viewState = computed<ViewState>(() => {
    if (loading.value && !loaded.value) return { kind: 'loading' }
    if (error.value !== null) return { kind: 'error', message: error.value, canRetry: true }
    if (filteredTasks.value.length === 0) return { kind: 'empty', filter: filter.value }
    return { kind: 'success', tasks: filteredTasks.value }
  })

  async function loadAll() {
    loading.value = true
    error.value = null
    const result = await tasksApi.list()
    loading.value = false
    if (result.kind === 'error') {
      error.value = describeError(result.error)
      return
    }
    tasks.value = result.data.data
    loaded.value = true
  }

  async function create(title: string) {
    const result = await tasksApi.create({ title })
    if (result.kind === 'error') {
      if (isValidationError(result.error)) {
        throw new TaskValidationError(result.error.problem.errors ?? {})
      }
      error.value = describeError(result.error)
      return
    }
    tasks.value = [result.data, ...tasks.value]
  }

  async function toggle(id: string) {
    const snapshot = tasks.value
    const index = snapshot.findIndex((t) => t.id === id)
    if (index === -1) return
    const original = snapshot[index]
    if (original === undefined) return

    tasks.value = snapshot.map((t, i) => (i === index ? { ...t, isCompleted: !t.isCompleted } : t))

    const result = await tasksApi.toggle(id)
    if (result.kind === 'error') {
      tasks.value = snapshot
      error.value = describeError(result.error)
      return
    }
    tasks.value = tasks.value.map((t) => (t.id === id ? result.data : t))
  }

  async function remove(id: string) {
    const snapshot = tasks.value
    if (!snapshot.some((t) => t.id === id)) return

    tasks.value = snapshot.filter((t) => t.id !== id)

    const result = await tasksApi.remove(id)
    if (result.kind === 'error' && !isMissingResource(result.error)) {
      tasks.value = snapshot
      error.value = describeError(result.error)
    }
  }

  function setFilter(next: TaskFilter) {
    filter.value = next
  }

  function clearError() {
    error.value = null
  }

  return {
    tasks,
    loading,
    error,
    filter,
    loaded,
    filteredTasks,
    completedCount,
    totalCount,
    viewState,
    loadAll,
    create,
    toggle,
    remove,
    setFilter,
    clearError,
  }
})

function isValidationError(error: ApiError): error is Extract<ApiError, { kind: 'http' }> {
  return error.kind === 'http' && error.status === 422
}

function isMissingResource(error: ApiError): boolean {
  return error.kind === 'http' && error.status === 404
}
