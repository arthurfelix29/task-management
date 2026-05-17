<script setup lang="ts">
import TaskItem from '@/features/tasks/components/TaskItem.vue'
import type { Task } from '@/features/tasks/types/task'

interface Props {
  tasks: Task[]
}

defineProps<Props>()

const emit = defineEmits<{
  toggle: [id: string]
  remove: [id: string]
}>()
</script>

<template>
  <ul class="relative space-y-2">
    <TransitionGroup
      enter-active-class="motion-safe:transition motion-safe:ease-out motion-safe:duration-200"
      enter-from-class="motion-safe:opacity-0 motion-safe:-translate-y-1"
      enter-to-class="motion-safe:opacity-100 motion-safe:translate-y-0"
      leave-active-class="motion-safe:transition motion-safe:ease-in motion-safe:duration-150 motion-safe:absolute motion-safe:inset-x-0"
      leave-from-class="motion-safe:opacity-100"
      leave-to-class="motion-safe:opacity-0 motion-safe:-translate-x-2"
      move-class="motion-safe:transition motion-safe:duration-200"
    >
      <TaskItem
        v-for="task in tasks"
        :key="task.id"
        :task="task"
        @toggle="emit('toggle', $event)"
        @remove="emit('remove', $event)"
      />
    </TransitionGroup>
  </ul>
</template>
