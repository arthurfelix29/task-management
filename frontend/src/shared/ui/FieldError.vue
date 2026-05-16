<script setup lang="ts">
import { AlertTriangle } from '@lucide/vue'

defineProps<{ id?: string; message: string }>()
</script>

<template>
  <Transition
    enter-active-class="transition ease-out duration-150"
    enter-from-class="opacity-0 -translate-y-1"
    enter-to-class="opacity-100 translate-y-0"
    leave-active-class="transition ease-in duration-100"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <p
      v-if="message"
      :id="id"
      role="alert"
      aria-live="polite"
      class="flex items-start gap-2 rounded-md border-l-[3px] border-danger bg-danger-subtle px-3 py-2 text-sm text-foreground"
    >
      <AlertTriangle class="mt-0.5 h-4 w-4 shrink-0 text-danger" aria-hidden="true" />
      <span>{{ message }}</span>
    </p>
  </Transition>
</template>
