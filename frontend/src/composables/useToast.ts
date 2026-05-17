import { onScopeDispose, ref } from 'vue'

export type ToastKind = 'success' | 'error' | 'info'

export type Toast = {
  id: number
  kind: ToastKind
  message: string
  durationMs: number
  createdAt: number
  pausedAt: number | null
  totalPaused: number
}

const MAX_VISIBLE = 3
const DEFAULT_DURATION_MS = 4000

const toasts = ref<Toast[]>([])
const progress = ref<Record<number, number>>({})
let nextId = 0
let rafHandle: number | null = null

function now(): number {
  return performance.now()
}

function elapsedOf(toast: Toast, currentTime: number): number {
  const reference = toast.pausedAt ?? currentTime
  return reference - toast.createdAt - toast.totalPaused
}

function ratioOf(toast: Toast, currentTime: number): number {
  const elapsed = elapsedOf(toast, currentTime)
  return Math.max(0, 1 - elapsed / toast.durationMs)
}

function startLoopIfNeeded() {
  if (rafHandle !== null) return
  rafHandle = requestAnimationFrame(tick)
}

function stopLoop() {
  if (rafHandle === null) return
  cancelAnimationFrame(rafHandle)
  rafHandle = null
}

function tick() {
  rafHandle = null
  const current = toasts.value
  if (current.length === 0) return

  const t = now()
  const expired: number[] = []
  const nextProgress: Record<number, number> = {}

  for (const toast of current) {
    nextProgress[toast.id] = ratioOf(toast, t)
    if (toast.pausedAt === null && elapsedOf(toast, t) >= toast.durationMs) {
      expired.push(toast.id)
    }
  }

  progress.value = nextProgress

  if (expired.length > 0) {
    toasts.value = toasts.value.filter((toast) => !expired.includes(toast.id))
    const remaining: Record<number, number> = {}
    for (const toast of toasts.value) remaining[toast.id] = nextProgress[toast.id] ?? 1
    progress.value = remaining
  }

  if (toasts.value.length > 0) startLoopIfNeeded()
}

function show(kind: ToastKind, message: string, durationMs = DEFAULT_DURATION_MS): number {
  const id = nextId++
  const toast: Toast = {
    id,
    kind,
    message,
    durationMs,
    createdAt: now(),
    pausedAt: null,
    totalPaused: 0,
  }
  const queue = toasts.value.length >= MAX_VISIBLE ? toasts.value.slice(1) : toasts.value
  toasts.value = [...queue, toast]
  progress.value = { ...progress.value, [id]: 1 }
  startLoopIfNeeded()
  return id
}

function dismiss(id: number) {
  toasts.value = toasts.value.filter((t) => t.id !== id)
  const { [id]: _removed, ...rest } = progress.value
  progress.value = rest
  if (toasts.value.length === 0) stopLoop()
}

function pause(id: number) {
  const t = now()
  toasts.value = toasts.value.map((toast) =>
    toast.id === id && toast.pausedAt === null ? { ...toast, pausedAt: t } : toast,
  )
}

function resume(id: number) {
  const t = now()
  toasts.value = toasts.value.map((toast) => {
    if (toast.id !== id || toast.pausedAt === null) return toast
    const pausedDuration = t - toast.pausedAt
    return { ...toast, pausedAt: null, totalPaused: toast.totalPaused + pausedDuration }
  })
  startLoopIfNeeded()
}

export function useToast() {
  onScopeDispose(() => {
    if (toasts.value.length === 0) stopLoop()
  })

  return {
    toasts,
    progress,
    dismiss,
    pause,
    resume,
    success: (message: string, durationMs?: number) => show('success', message, durationMs),
    error: (message: string, durationMs?: number) => show('error', message, durationMs),
    info: (message: string, durationMs?: number) => show('info', message, durationMs),
  }
}
