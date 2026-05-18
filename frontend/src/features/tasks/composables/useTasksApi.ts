import type { z } from 'zod'
import { request, type Result } from '@/shared/lib/api-client'
import type { ApiError } from '@/shared/lib/problem-details'
import { taskListResponseSchema, taskSchema } from '@/features/tasks/schemas/task.schema'
import type { CreateTaskRequest, Task, TaskListResponse } from '@/features/tasks/types/task'

const base = '/api/v1/tasks'

export type ApiCallOptions = {
  signal?: AbortSignal
}

export const tasksApi = {
  async list(options: ApiCallOptions = {}): Promise<Result<TaskListResponse>> {
    const response = await request<unknown>('GET', base, withSignal(options))
    if (response.kind === 'error') return response
    return parse(response.data, taskListResponseSchema)
  },

  async create(body: CreateTaskRequest, options: ApiCallOptions = {}): Promise<Result<Task>> {
    const response = await request<unknown>('POST', base, { ...withSignal(options), body })
    if (response.kind === 'error') return response
    return parse(response.data, taskSchema)
  },

  async toggle(id: string, options: ApiCallOptions = {}): Promise<Result<Task>> {
    const response = await request<unknown>('POST', `${base}/${id}/toggle`, withSignal(options))
    if (response.kind === 'error') return response
    return parse(response.data, taskSchema)
  },

  async remove(id: string, options: ApiCallOptions = {}): Promise<Result<void>> {
    return request<void>('DELETE', `${base}/${id}`, withSignal(options))
  },
}

function withSignal(options: ApiCallOptions): { signal?: AbortSignal } {
  return options.signal !== undefined ? { signal: options.signal } : {}
}

function parse<T>(value: unknown, schema: z.ZodType<T>): Result<T, ApiError> {
  const result = schema.safeParse(value)
  if (!result.success) {
    return { kind: 'error', error: { kind: 'contract', message: 'Response did not match expected shape.' } }
  }
  return { kind: 'ok', data: result.data }
}
