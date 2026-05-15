import { ref } from 'vue'

export type ToastKind = 'success' | 'error' | 'info'

export type Toast = {
  id: number
  kind: ToastKind
  message: string
}

const toasts = ref<Toast[]>([])
let nextId = 0

const DEFAULT_DURATION_MS = 4000

function show(kind: ToastKind, message: string, durationMs = DEFAULT_DURATION_MS): number {
  const id = nextId++
  toasts.value = [...toasts.value, { id, kind, message }]
  if (durationMs > 0) {
    setTimeout(() => dismiss(id), durationMs)
  }
  return id
}

function dismiss(id: number) {
  toasts.value = toasts.value.filter((t) => t.id !== id)
}

export function useToast() {
  return {
    toasts,
    dismiss,
    success: (message: string, durationMs?: number) => show('success', message, durationMs),
    error: (message: string, durationMs?: number) => show('error', message, durationMs),
    info: (message: string, durationMs?: number) => show('info', message, durationMs),
  }
}
