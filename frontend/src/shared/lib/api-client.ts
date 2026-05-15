import { type ApiError, networkError, parseProblemDetails } from '@/shared/lib/problem-details'

export type Result<T, E = ApiError> =
  | { kind: 'ok'; data: T }
  | { kind: 'error'; error: E }

const baseUrl = (import.meta.env['VITE_API_BASE_URL'] ?? '').replace(/\/$/, '')

export type RequestOptions = {
  signal?: AbortSignal
  body?: unknown
}

export async function request<T>(
  method: string,
  path: string,
  options: RequestOptions = {},
): Promise<Result<T>> {
  const headers: Record<string, string> = { Accept: 'application/json' }
  if (options.body !== undefined) {
    headers['Content-Type'] = 'application/json'
  }

  const init: RequestInit = { method, headers }
  if (options.body !== undefined) {
    init.body = JSON.stringify(options.body)
  }
  if (options.signal !== undefined) {
    init.signal = options.signal
  }

  let response: Response
  try {
    response = await fetch(`${baseUrl}${path}`, init)
  } catch (err) {
    if (err instanceof DOMException && err.name === 'AbortError') {
      throw err
    }
    return { kind: 'error', error: networkError(describeNetworkFailure(err)) }
  }

  if (!response.ok) {
    return { kind: 'error', error: await parseProblemDetails(response) }
  }

  if (response.status === 204) {
    return { kind: 'ok', data: undefined as T }
  }

  try {
    const data = (await response.json()) as T
    return { kind: 'ok', data }
  } catch {
    return { kind: 'error', error: { kind: 'contract', message: 'Response was not valid JSON.' } }
  }
}

function describeNetworkFailure(err: unknown): string {
  if (err instanceof Error) return err.message
  return 'Network request failed.'
}
