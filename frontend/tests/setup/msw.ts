import { setupServer } from 'msw/node'
import { HttpResponse, http } from 'msw'
import type { Task, TaskListResponse } from '@/features/tasks/types/task'

export const sampleTask = (overrides: Partial<Task> = {}): Task => ({
  id: '0192f000-1111-7000-8000-000000000001',
  title: 'Sample task',
  isCompleted: false,
  createdAt: '2026-05-15T10:00:00Z',
  links: [
    { rel: 'self', href: '/api/v1/tasks/0192f000-1111-7000-8000-000000000001', method: 'GET' },
    { rel: 'toggle', href: '/api/v1/tasks/0192f000-1111-7000-8000-000000000001/toggle', method: 'POST' },
    { rel: 'delete', href: '/api/v1/tasks/0192f000-1111-7000-8000-000000000001', method: 'DELETE' },
  ],
  ...overrides,
})

export const sampleListResponse = (data: Task[]): TaskListResponse => ({
  data,
  count: data.length,
  links: [
    { rel: 'self', href: '/api/v1/tasks', method: 'GET' },
    { rel: 'create', href: '/api/v1/tasks', method: 'POST' },
  ],
})

export const handlers = [
  http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse([sampleTask()]))),
  http.post('/api/v1/tasks', async ({ request }) => {
    const body = (await request.json()) as { title?: string }
    return HttpResponse.json(sampleTask({ id: 'created-id', title: body.title ?? '' }), { status: 201 })
  }),
  http.post('/api/v1/tasks/:id/toggle', ({ params }) => {
    const id = params['id'] as string
    return HttpResponse.json(sampleTask({ id, isCompleted: true }))
  }),
  http.delete('/api/v1/tasks/:id', () => new HttpResponse(null, { status: 204 })),
]

export const server = setupServer(...handlers)
