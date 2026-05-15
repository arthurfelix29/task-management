<script setup lang="ts">
import { useToast } from '@/composables/useToast'
import { cn } from '@/shared/lib/cn'

const { toasts, dismiss } = useToast()

function roleFor(kind: 'success' | 'error' | 'info'): 'alert' | 'status' {
  return kind === 'error' ? 'alert' : 'status'
}

function classesFor(kind: 'success' | 'error' | 'info') {
  return cn(
    'pointer-events-auto flex min-w-72 max-w-sm items-start gap-3 rounded-md border px-4 py-3 shadow-md',
    kind === 'success' && 'border-border bg-surface-overlay text-foreground',
    kind === 'error' && 'border-danger bg-surface-overlay text-foreground',
    kind === 'info' && 'border-border bg-surface-overlay text-foreground',
  )
}
</script>

<template>
  <div
    aria-live="polite"
    class="pointer-events-none fixed inset-x-0 bottom-4 z-50 flex flex-col items-center gap-2 px-4"
  >
    <div
      v-for="t in toasts"
      :key="t.id"
      :role="roleFor(t.kind)"
      :aria-live="t.kind === 'error' ? 'assertive' : 'polite'"
      :class="classesFor(t.kind)"
    >
      <span class="flex-1 text-sm">{{ t.message }}</span>
      <button
        type="button"
        :aria-label="`Dismiss notification: ${t.message}`"
        class="text-muted-foreground hover:text-foreground focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-focus-ring"
        @click="dismiss(t.id)"
      >
        ×
      </button>
    </div>
  </div>
</template>
