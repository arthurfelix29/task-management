<script setup lang="ts">
import { type Component, computed } from 'vue'
import { CheckCircle2, Info, X, XCircle } from '@lucide/vue'
import { useToast, type ToastKind } from '@/composables/useToast'
import { cn } from '@/shared/lib/cn'

const { toasts, progress, dismiss, pause, resume } = useToast()

const ICONS: Record<ToastKind, Component> = {
  success: CheckCircle2,
  error: XCircle,
  info: Info,
}

const VARIANTS: Record<ToastKind, { container: string; icon: string; bar: string }> = {
  success: {
    container: 'border-l-4 border-success bg-success-subtle text-foreground',
    icon: 'text-success',
    bar: 'bg-success',
  },
  error: {
    container: 'border-l-4 border-danger bg-danger-subtle text-foreground',
    icon: 'text-danger',
    bar: 'bg-danger',
  },
  info: {
    container: 'border-l-4 border-info bg-info-subtle text-foreground',
    icon: 'text-info',
    bar: 'bg-info',
  },
}

const sorted = computed(() => toasts.value)

function widthFor(id: number): string {
  const ratio = progress.value[id] ?? 1
  return `${ratio * 100}%`
}
</script>

<template>
  <div
    aria-live="polite"
    class="pointer-events-none fixed bottom-4 right-4 z-50 flex w-full max-w-sm flex-col gap-2"
  >
    <TransitionGroup
      enter-active-class="transition ease-out duration-150"
      enter-from-class="opacity-0 translate-x-4"
      enter-to-class="opacity-100 translate-x-0"
      leave-active-class="transition ease-in duration-150 absolute right-0"
      leave-from-class="opacity-100 translate-x-0"
      leave-to-class="opacity-0 translate-x-4"
    >
      <div
        v-for="t in sorted"
        :key="t.id"
        :role="t.kind === 'error' ? 'alert' : 'status'"
        :aria-live="t.kind === 'error' ? 'assertive' : 'polite'"
        :class="
          cn(
            'pointer-events-auto relative w-full overflow-hidden rounded-md shadow-md',
            VARIANTS[t.kind].container,
          )
        "
        @mouseenter="pause(t.id)"
        @mouseleave="resume(t.id)"
        @focusin="pause(t.id)"
        @focusout="resume(t.id)"
      >
        <div class="flex items-start gap-3 px-4 py-3 pr-10">
          <component
            :is="ICONS[t.kind]"
            :class="cn('mt-0.5 h-5 w-5 shrink-0', VARIANTS[t.kind].icon)"
            aria-hidden="true"
          />
          <p class="flex-1 text-sm leading-tight">{{ t.message }}</p>
          <button
            type="button"
            :aria-label="`Dismiss notification: ${t.message}`"
            class="absolute right-2 top-2 inline-flex h-7 w-7 items-center justify-center rounded text-muted-foreground transition hover:bg-surface-elevated hover:text-foreground focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-focus-ring"
            @click="dismiss(t.id)"
          >
            <X class="h-4 w-4" aria-hidden="true" />
          </button>
        </div>
        <div
          :class="cn('absolute bottom-0 left-0 h-1 transition-[width] duration-100 ease-linear', VARIANTS[t.kind].bar)"
          :style="{ width: widthFor(t.id) }"
          aria-hidden="true"
        />
      </div>
    </TransitionGroup>
  </div>
</template>
