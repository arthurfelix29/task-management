import { describe, expect, it } from 'vitest'
import { render, screen } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'
import TaskItem from '@/features/tasks/components/TaskItem.vue'
import type { Task } from '@/features/tasks/types/task'

const baseTask: Task = {
  id: 'task-1',
  title: 'Buy groceries',
  isCompleted: false,
  createdAt: '2026-05-15T10:00:00Z',
  links: [
    { rel: 'self', href: '/api/v1/tasks/task-1', method: 'GET' },
    { rel: 'toggle', href: '/api/v1/tasks/task-1/toggle', method: 'POST' },
    { rel: 'delete', href: '/api/v1/tasks/task-1', method: 'DELETE' },
  ],
}

describe('TaskItem', () => {
  it('When_ClickingToggleCheckbox_Should_EmitToggleWithTaskId', async () => {
    const user = userEvent.setup()
    const { emitted } = render(TaskItem, { props: { task: baseTask } })

    await user.click(screen.getByRole('checkbox'))

    expect(emitted('toggle')).toEqual([['task-1']])
  })

  it('When_ClickingDeleteButton_Should_EmitRemoveWithTaskId', async () => {
    const user = userEvent.setup()
    const { emitted } = render(TaskItem, { props: { task: baseTask } })

    await user.click(screen.getByRole('button', { name: /delete task: buy groceries/i }))

    expect(emitted('remove')).toEqual([['task-1']])
  })

  it('When_TaskHasNoToggleLink_Should_DisableToggleCheckbox', () => {
    const taskWithoutToggle: Task = { ...baseTask, links: baseTask.links.filter((l) => l.rel !== 'toggle') }
    render(TaskItem, { props: { task: taskWithoutToggle } })

    expect(screen.getByRole('checkbox')).toBeDisabled()
  })
})
