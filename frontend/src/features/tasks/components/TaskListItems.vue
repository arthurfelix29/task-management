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
  <ul class="space-y-2">
    <TaskItem
      v-for="task in tasks"
      :key="task.id"
      :task="task"
      @toggle="emit('toggle', $event)"
      @remove="emit('remove', $event)"
    />
  </ul>
</template>
