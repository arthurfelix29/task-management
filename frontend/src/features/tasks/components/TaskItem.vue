<script setup lang="ts">
import { computed, ref } from 'vue'
import { Trash2 } from '@lucide/vue'
import Button from '@/shared/ui/Button.vue'
import Checkbox from '@/shared/ui/Checkbox.vue'
import ConfirmDeleteModal from '@/features/tasks/components/ConfirmDeleteModal.vue'
import type { Task } from '@/features/tasks/types/task'
import { cn } from '@/shared/lib/cn'

interface Props {
  task: Task
}

const props = defineProps<Props>()

const emit = defineEmits<{
  toggle: [id: string]
  remove: [id: string]
}>()

const canToggle = computed(() => props.task.links.some((l) => l.rel === 'toggle'))
const canDelete = computed(() => props.task.links.some((l) => l.rel === 'delete'))

const titleClasses = computed(() =>
  cn(
    'flex-1 text-sm break-words text-foreground',
    props.task.isCompleted && 'text-muted-foreground line-through',
  ),
)

const createdAtLabel = computed(() => {
  const date = new Date(props.task.createdAt)
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(date)
})

const modalOpen = ref(false)

function onDeleteClick() {
  modalOpen.value = true
}

function onConfirmDelete() {
  modalOpen.value = false
  emit('remove', props.task.id)
}

function onCancelDelete() {
  modalOpen.value = false
}
</script>

<template>
  <li class="group flex items-center gap-3 rounded-md border border-border bg-surface px-3 py-2 shadow-sm">
    <Checkbox
      :model-value="task.isCompleted"
      :label="`Mark ${task.title} as ${task.isCompleted ? 'pending' : 'completed'}`"
      :disabled="!canToggle"
      @update:model-value="emit('toggle', task.id)"
    />
    <div class="flex-1">
      <p :class="titleClasses">{{ task.title }}</p>
      <p class="text-xs text-muted-foreground">{{ createdAtLabel }}</p>
    </div>
    <Button
      variant="ghost"
      size="icon"
      :disabled="!canDelete"
      :aria-label="`Delete task: ${task.title}`"
      class="opacity-0 transition-opacity duration-150 hover:text-danger focus-visible:opacity-100 group-hover:opacity-100 [@media(hover:none)]:opacity-60"
      @click="onDeleteClick"
    >
      <Trash2 class="h-4 w-4" aria-hidden="true" />
    </Button>

    <ConfirmDeleteModal
      :open="modalOpen"
      :task-title="task.title"
      @confirm="onConfirmDelete"
      @cancel="onCancelDelete"
    />
  </li>
</template>
