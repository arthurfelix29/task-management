export type ProblemDetails = {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  traceId?: string
  errors?: Record<string, string[]>
}

export type ApiError =
  | { kind: 'network'; message: string; recoverable: true }
  | { kind: 'http'; status: number; problem: ProblemDetails }
  | { kind: 'contract'; message: string }

export function networkError(message: string): ApiError {
  return { kind: 'network', message, recoverable: true }
}

export function contractError(message: string): ApiError {
  return { kind: 'contract', message }
}

export async function parseProblemDetails(response: Response): Promise<ApiError> {
  const contentType = response.headers.get('content-type') ?? ''
  if (!contentType.includes('json')) {
    return {
      kind: 'http',
      status: response.status,
      problem: { status: response.status, title: response.statusText || 'Unexpected error' },
    }
  }
  const text = await response.text()
  if (text.length === 0) {
    return {
      kind: 'http',
      status: response.status,
      problem: { status: response.status, title: response.statusText || 'Unexpected error' },
    }
  }
  try {
    const body = JSON.parse(text) as ProblemDetails
    return { kind: 'http', status: response.status, problem: body }
  } catch {
    return contractError('Response body was not valid JSON.')
  }
}

export function describeError(error: ApiError): string {
  if (error.kind === 'network') return error.message
  if (error.kind === 'contract') return error.message
  return error.problem.detail ?? error.problem.title ?? `HTTP ${error.status}`
}
