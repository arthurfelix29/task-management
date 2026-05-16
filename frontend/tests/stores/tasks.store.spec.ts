import { afterEach, beforeEach, describe, expect, it } from 'vitest'
import { HttpResponse, http } from 'msw'
import { createPinia, setActivePinia } from 'pinia'
import { useTasksStore } from '@/features/tasks/stores/tasks.store'
import { sampleListResponse, sampleTask, server } from '../setup/msw'

beforeEach(() => {
  setActivePinia(createPinia())
  window.localStorage.removeItem('tasklist:sort')
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

  it('When_CreateApiReturns409_Should_ReturnFieldErrorWithDuplicateMessage', async () => {
    server.use(
      http.post('/api/v1/tasks', () =>
        HttpResponse.json(
          { status: 409, title: 'Conflict', detail: "A task with the title 'X' already exists." },
          { status: 409 },
        ),
      ),
    )

    const store = useTasksStore()
    const outcome = await store.create('X')

    expect(outcome).toEqual({
      kind: 'field-error',
      field: 'title',
      message: 'A task with this title already exists.',
    })
    expect(store.tasks).toHaveLength(0)
  })

  it('When_SearchQueryMatchesTitle_Should_ReturnOnlyMatchingTasks', async () => {
    const tasks = [
      sampleTask({ id: 'a', title: 'Buy milk' }),
      sampleTask({ id: 'b', title: 'Sell stocks' }),
      sampleTask({ id: 'c', title: 'MILK delivery' }),
    ]
    server.use(http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse(tasks))))

    const store = useTasksStore()
    await store.loadAll()
    store.setSearchQuery('milk')

    expect(store.filteredTasks.map((t) => t.id).sort()).toEqual(['a', 'c'])
  })

  describe('sort options', () => {
    const sortFixtures = [
      sampleTask({ id: 'old', title: 'apple', createdAt: '2026-01-01T00:00:00Z' }),
      sampleTask({ id: 'new', title: 'Banana', createdAt: '2026-05-15T00:00:00Z' }),
      sampleTask({ id: 'mid', title: 'cherry', createdAt: '2026-03-01T00:00:00Z' }),
    ]

    async function bootStoreWithFixtures() {
      server.use(http.get('/api/v1/tasks', () => HttpResponse.json(sampleListResponse(sortFixtures))))
      const store = useTasksStore()
      await store.loadAll()
      return store
    }

    it('When_SortByNewest_Should_OrderByCreatedAtDescending', async () => {
      const store = await bootStoreWithFixtures()
      store.setSortBy('newest')

      expect(store.filteredTasks.map((t) => t.id)).toEqual(['new', 'mid', 'old'])
    })

    it('When_SortByOldest_Should_OrderByCreatedAtAscending', async () => {
      const store = await bootStoreWithFixtures()
      store.setSortBy('oldest')

      expect(store.filteredTasks.map((t) => t.id)).toEqual(['old', 'mid', 'new'])
    })

    it('When_SortByNameAsc_Should_OrderAlphabeticallyCaseInsensitive', async () => {
      const store = await bootStoreWithFixtures()
      store.setSortBy('name-asc')

      expect(store.filteredTasks.map((t) => t.title)).toEqual(['apple', 'Banana', 'cherry'])
    })

    it('When_SortByNameDesc_Should_OrderReverseAlphabeticallyCaseInsensitive', async () => {
      const store = await bootStoreWithFixtures()
      store.setSortBy('name-desc')

      expect(store.filteredTasks.map((t) => t.title)).toEqual(['cherry', 'Banana', 'apple'])
    })
  })
})
