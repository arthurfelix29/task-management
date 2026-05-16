import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { useStorage } from '@vueuse/core'
import { tasksApi } from '@/features/tasks/composables/useTasksApi'
import type { SortOption, Task, TaskFilter } from '@/features/tasks/types/task'
import { type ApiError, describeError } from '@/shared/lib/problem-details'
import { useToast } from '@/composables/useToast'

const VALID_SORTS: readonly SortOption[] = ['newest', 'oldest', 'name-asc', 'name-desc']

const sortSerializer = {
  read: (raw: string): SortOption =>
    (VALID_SORTS as readonly string[]).includes(raw) ? (raw as SortOption) : 'newest',
  write: (value: SortOption) => value,
}

const NETWORK_ERROR_MESSAGE = 'Something went wrong. Check your connection.'

export type ViewState =
  | { kind: 'loading' }
  | { kind: 'error'; message: string; canRetry: boolean }
  | { kind: 'empty'; filter: TaskFilter }
  | { kind: 'success'; tasks: Task[] }

export type CreateTaskOutcome =
  | { kind: 'ok' }
  | { kind: 'field-error'; field: 'title'; message: string }
  | { kind: 'failure' }

const DUPLICATE_TITLE_MESSAGE = 'A task with this title already exists.'
const GENERIC_VALIDATION_MESSAGE = 'The server rejected this title.'

export const useTasksStore = defineStore('tasks', () => {
  const toast = useToast()

  const tasks = ref<Task[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const filter = ref<TaskFilter>('all')
  const searchQuery = ref('')
  const sortBy = useStorage<SortOption>('tasklist:sort', 'newest', undefined, {
    serializer: sortSerializer,
  })
  const loaded = ref(false)

  const filteredTasks = computed(() => {
    const statusFiltered =
      filter.value === 'all'
        ? tasks.value
        : tasks.value.filter((t) => t.isCompleted === (filter.value === 'completed'))

    const query = searchQuery.value.trim().toLowerCase()
    const titleFiltered =
      query === '' ? statusFiltered : statusFiltered.filter((t) => t.title.toLowerCase().includes(query))

    return sortTasks(titleFiltered, sortBy.value)
  })

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

  async function create(title: string): Promise<CreateTaskOutcome> {
    const result = await tasksApi.create({ title })
    if (result.kind === 'error') {
      if (isValidationError(result.error)) {
        const fieldMessage = result.error.problem.errors?.['Title']?.join(', ')
        return { kind: 'field-error', field: 'title', message: fieldMessage ?? GENERIC_VALIDATION_MESSAGE }
      }
      if (isDuplicateConflict(result.error)) {
        return { kind: 'field-error', field: 'title', message: DUPLICATE_TITLE_MESSAGE }
      }
      error.value = describeError(result.error)
      if (result.error.kind === 'network') toast.error(NETWORK_ERROR_MESSAGE)
      return { kind: 'failure' }
    }
    tasks.value = [result.data, ...tasks.value]
    toast.success('Task created')
    return { kind: 'ok' }
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
      toast.error(
        result.error.kind === 'network' ? NETWORK_ERROR_MESSAGE : "Couldn't toggle task. Try again.",
      )
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
      toast.error(
        result.error.kind === 'network' ? NETWORK_ERROR_MESSAGE : "Couldn't delete task. Try again.",
      )
      return
    }
    toast.success('Task deleted')
  }

  function setFilter(next: TaskFilter) {
    filter.value = next
  }

  function setSearchQuery(next: string) {
    searchQuery.value = next
  }

  function setSortBy(next: SortOption) {
    sortBy.value = next
  }

  function clearError() {
    error.value = null
  }

  return {
    tasks,
    loading,
    error,
    filter,
    searchQuery,
    sortBy,
    loaded,
    filteredTasks,
    viewState,
    loadAll,
    create,
    toggle,
    remove,
    setFilter,
    setSearchQuery,
    setSortBy,
    clearError,
  }
})

function sortTasks(tasks: readonly Task[], option: SortOption): Task[] {
  const sorted = [...tasks]
  switch (option) {
    case 'newest':
      sorted.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      break
    case 'oldest':
      sorted.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime())
      break
    case 'name-asc':
      sorted.sort((a, b) => a.title.localeCompare(b.title, undefined, { sensitivity: 'accent' }))
      break
    case 'name-desc':
      sorted.sort((a, b) => b.title.localeCompare(a.title, undefined, { sensitivity: 'accent' }))
      break
  }
  return sorted
}

function isValidationError(error: ApiError): error is Extract<ApiError, { kind: 'http' }> {
  return error.kind === 'http' && error.status === 422
}

function isDuplicateConflict(error: ApiError): error is Extract<ApiError, { kind: 'http' }> {
  return error.kind === 'http' && error.status === 409
}

function isMissingResource(error: ApiError): boolean {
  return error.kind === 'http' && error.status === 404
}
