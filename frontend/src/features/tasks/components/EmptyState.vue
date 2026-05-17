<script setup lang="ts">
import { computed } from 'vue'
import type { EmptyReason } from '@/features/tasks/stores/tasks.store'
import type { TaskFilter } from '@/features/tasks/types/task'

interface Props {
  filter: TaskFilter
  reason: EmptyReason
}

const props = defineProps<Props>()

const heading = computed(() => {
  if (props.reason === 'search') return 'No tasks match your search'
  if (props.filter === 'active') return 'No active tasks'
  if (props.filter === 'completed') return 'No completed tasks'
  return 'No tasks yet'
})

const message = computed(() => {
  if (props.reason === 'search') return 'Try a different search or clear the search box.'
  if (props.filter === 'active') return 'Everything is done — enjoy the moment.'
  if (props.filter === 'completed') return 'Complete a task to see it here.'
  return 'Add your first task using the form above.'
})
</script>

<template>
  <section
    aria-label="Empty list"
    class="rounded-md border border-dashed border-border bg-surface-elevated px-6 py-10 text-center"
  >
    <h2 class="text-base font-semibold text-foreground">{{ heading }}</h2>
    <p class="mt-1 text-sm text-muted-foreground">{{ message }}</p>
  </section>
</template>
