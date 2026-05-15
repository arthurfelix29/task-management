import { describe, expect, it } from 'vitest'
import { screen } from '@testing-library/vue'
import TaskList from '@/features/tasks/components/TaskList.vue'
import type { Task } from '@/features/tasks/types/task'
import { renderWithProviders } from '../setup/render'
import { axe } from '../setup/axe'

const sampleTask = (overrides: Partial<Task> = {}): Task => ({
  id: 'task-1',
  title: 'Sample task',
  isCompleted: false,
  createdAt: '2026-05-15T10:00:00Z',
  links: [
    { rel: 'self', href: '/api/v1/tasks/task-1', method: 'GET' },
    { rel: 'toggle', href: '/api/v1/tasks/task-1/toggle', method: 'POST' },
    { rel: 'delete', href: '/api/v1/tasks/task-1', method: 'DELETE' },
  ],
  ...overrides,
})

type StoreState = {
  tasks: Task[]
  loading: boolean
  error: string | null
  filter: 'all' | 'active' | 'completed'
  loaded: boolean
}

function renderTaskListWith(state: StoreState) {
  return renderWithProviders(TaskList, {
    testingPinia: { stubActions: true, initialState: { tasks: state } },
  })
}

describe('TaskList', () => {
  it('When_StoreIsLoading_Should_RenderLoadingSkeletons', () => {
    renderTaskListWith({ tasks: [], loading: true, error: null, filter: 'all', loaded: false })

    expect(screen.getByLabelText(/loading tasks/i)).toBeInTheDocument()
  })

  it('When_StoreHasError_Should_RenderErrorStateWithRetry', () => {
    renderTaskListWith({ tasks: [], loading: false, error: 'Network unreachable', filter: 'all', loaded: true })

    const alert = screen.getByRole('alert')
    expect(alert).toHaveTextContent(/network unreachable/i)
    expect(screen.getByRole('button', { name: /try again/i })).toBeInTheDocument()
  })

  it('When_StoreHasNoTasks_Should_RenderEmptyState', () => {
    renderTaskListWith({ tasks: [], loading: false, error: null, filter: 'all', loaded: true })

    expect(screen.getByRole('heading', { name: /no tasks yet/i })).toBeInTheDocument()
  })

  it('When_StoreHasTasks_Should_RenderTaskListItems', () => {
    renderTaskListWith({
      tasks: [sampleTask({ id: 'a', title: 'Alpha' }), sampleTask({ id: 'b', title: 'Beta' })],
      loading: false,
      error: null,
      filter: 'all',
      loaded: true,
    })

    expect(screen.getByText('Alpha')).toBeInTheDocument()
    expect(screen.getByText('Beta')).toBeInTheDocument()
    expect(screen.queryByRole('heading', { name: /no tasks/i })).not.toBeInTheDocument()
  })

  it('When_Rendered_Should_HaveNoAxeViolations', async () => {
    const { container } = renderTaskListWith({
      tasks: [sampleTask({ id: 'a', title: 'Alpha' })],
      loading: false,
      error: null,
      filter: 'all',
      loaded: true,
    })

    expect(await axe(container)).toHaveNoViolations()
  })
})
