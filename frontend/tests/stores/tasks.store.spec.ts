import { afterEach, beforeEach, describe, expect, it } from 'vitest'
import { HttpResponse, http } from 'msw'
import { createPinia, setActivePinia } from 'pinia'
import { useTasksStore } from '@/features/tasks/stores/tasks.store'
import { sampleListResponse, sampleTask, server } from '../setup/msw'

beforeEach(() => {
  setActivePinia(createPinia())
})

afterEach(() => {
  server.resetHandlers()
})

describe('tasks store', () => {
  it('When_LoadAllSucceeds_Should_PopulateTasksAndSetSuccessState', async () => {
    const tasks = [sampleTask({ id: 'a', title: 'Alpha' }), sampleTask({ id: 'b', title: 'Beta' })]
    server.use(http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse(tasks))))

    const store = useTasksStore()
    await store.loadAll()

    expect(store.tasks).toHaveLength(2)
    expect(store.loaded).toBe(true)
    expect(store.error).toBeNull()
  })

  it('When_LoadAllReturns500_Should_SetErrorStateWithMessage', async () => {
    server.use(
      http.get('/api/v1/tasks', () =>
        HttpResponse.json({ status: 500, title: 'Server error', detail: 'Boom' }, { status: 500 }),
      ),
    )

    const store = useTasksStore()
    await store.loadAll()

    expect(store.error).toBe('Boom')
    expect(store.tasks).toHaveLength(0)
  })

  it('When_TogglingTask_Should_OptimisticallyFlipCompletionImmediately', async () => {
    const original = sampleTask({ id: 'x', isCompleted: false })
    server.use(
      http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse([original]))),
      http.post('/api/v1/tasks/:id/toggle', async () => {
        await new Promise((resolve) => setTimeout(resolve, 20))
        return HttpResponse.json(sampleTask({ id: 'x', isCompleted: true }))
      }),
    )

    const store = useTasksStore()
    await store.loadAll()
    const togglePromise = store.toggle('x')

    expect(store.tasks[0]?.isCompleted).toBe(true)

    await togglePromise
    expect(store.tasks[0]?.isCompleted).toBe(true)
  })

  it('When_ToggleApiFails_Should_RestoreSnapshotAndExposeError', async () => {
    const original = sampleTask({ id: 'x', isCompleted: false })
    server.use(
      http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse([original]))),
      http.post('/api/v1/tasks/:id/toggle', () =>
        HttpResponse.json({ status: 500, title: 'Boom', detail: 'Server crashed' }, { status: 500 }),
      ),
    )

    const store = useTasksStore()
    await store.loadAll()
    await store.toggle('x')

    expect(store.tasks[0]?.isCompleted).toBe(false)
    expect(store.error).toBe('Server crashed')
  })

  it('When_DeletingTask_Should_OptimisticallyRemoveFromListImmediately', async () => {
    const tasks = [sampleTask({ id: 'a' }), sampleTask({ id: 'b' })]
    server.use(
      http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse(tasks))),
      http.delete('/api/v1/tasks/:id', async () => {
        await new Promise((resolve) => setTimeout(resolve, 20))
        return new HttpResponse(null, { status: 204 })
      }),
    )

    const store = useTasksStore()
    await store.loadAll()
    const removePromise = store.remove('a')

    expect(store.tasks.map((t) => t.id)).toEqual(['b'])

    await removePromise
    expect(store.tasks.map((t) => t.id)).toEqual(['b'])
  })

  it('When_DeleteApiFails_Should_RestoreSnapshotAndExposeError', async () => {
    const tasks = [sampleTask({ id: 'a' }), sampleTask({ id: 'b' })]
    server.use(
      http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse(tasks))),
      http.delete('/api/v1/tasks/:id', () =>
        HttpResponse.json({ status: 500, title: 'Boom', detail: 'Server crashed' }, { status: 500 }),
      ),
    )

    const store = useTasksStore()
    await store.loadAll()
    await store.remove('a')

    expect(store.tasks).toHaveLength(2)
    expect(store.error).toBe('Server crashed')
  })
})
