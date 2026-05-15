<script setup lang="ts">
import { cn } from '@/shared/lib/cn'
import type { TaskFilter } from '@/features/tasks/types/task'

interface Props {
  modelValue: TaskFilter
}

defineProps<Props>()

const emit = defineEmits<{
  'update:modelValue': [value: TaskFilter]
}>()

const filters: { value: TaskFilter; label: string }[] = [
  { value: 'all', label: 'All' },
  { value: 'active', label: 'Active' },
  { value: 'completed', label: 'Completed' },
]
</script>

<template>
  <div role="group" aria-label="Filter tasks" class="inline-flex rounded-md border border-border bg-surface p-1 shadow-sm">
    <button
      v-for="f in filters"
      :key="f.value"
      type="button"
      :aria-pressed="modelValue === f.value"
      :class="
        cn(
          'rounded px-3 py-1 text-sm transition focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-focus-ring',
          modelValue === f.value
            ? 'bg-primary text-primary-foreground'
            : 'text-muted-foreground hover:bg-surface-elevated hover:text-foreground',
        )
      "
      @click="emit('update:modelValue', f.value)"
    >
      {{ f.label }}
    </button>
  </div>
</template>
