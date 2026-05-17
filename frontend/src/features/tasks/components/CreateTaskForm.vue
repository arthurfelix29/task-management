<script setup lang="ts">
import { computed, ref } from 'vue'
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import { Plus } from '@lucide/vue'
import Button from '@/shared/ui/Button.vue'
import Input from '@/shared/ui/Input.vue'
import FieldError from '@/shared/ui/FieldError.vue'
import { createTaskSchema, type CreateTaskInput } from '@/features/tasks/schemas/task.schema'
import { useTasksStore } from '@/features/tasks/stores/tasks.store'
import { cn } from '@/shared/lib/cn'

const MAX_TITLE = 200
const WARNING_THRESHOLD = 180

const store = useTasksStore()
const inputRef = ref<InstanceType<typeof Input> | null>(null)

const { defineField, handleSubmit, resetForm, errors, isSubmitting, setErrors, submitCount } =
  useForm<CreateTaskInput>({
    validationSchema: toTypedSchema(createTaskSchema),
    initialValues: { title: '' },
    validateOnMount: false,
  })

const [title, titleAttrs] = defineField('title', () => {
  const hasAttemptedSubmit = submitCount.value > 0
  return {
    validateOnBlur: hasAttemptedSubmit,
    validateOnChange: hasAttemptedSubmit,
    validateOnInput: false,
    validateOnModelUpdate: hasAttemptedSubmit,
  }
})

const titleLength = computed(() => (title.value ?? '').length)
const isApproachingLimit = computed(() => titleLength.value > WARNING_THRESHOLD)
const counterClasses = computed(() =>
  cn(
    'text-xs tabular-nums transition-colors',
    isApproachingLimit.value ? 'font-medium text-warning' : 'text-muted-foreground',
  ),
)

const onSubmit = handleSubmit(async (values) => {
  const outcome = await store.create(values.title)

  if (outcome.kind === 'ok') {
    resetForm()
    focusInput()
    return
  }

  if (outcome.kind === 'field-error') {
    setErrors({ [outcome.field]: outcome.message })
  }
})

function focusInput() {
  const input = document.getElementById('create-task-input') as HTMLInputElement | null
  input?.focus()
}

function onEscape() {
  resetForm()
}
</script>

<template>
  <form class="flex flex-col gap-2" novalidate @submit="onSubmit" @keydown.escape="onEscape">
    <div class="flex items-end gap-2">
      <div class="flex-1">
        <Input
          id="create-task-input"
          ref="inputRef"
          v-model="title"
          label="New task"
          placeholder="What needs to be done?"
          :maxlength="MAX_TITLE"
          :invalid="!!errors.title"
          error-id="create-task-error"
          hide-label
          v-bind="titleAttrs"
        >
          <template #suffix>
            <span v-if="titleLength > 0" :class="counterClasses" aria-live="off">
              {{ titleLength }}/{{ MAX_TITLE }}
            </span>
          </template>
        </Input>
      </div>
      <Button type="submit" :disabled="isSubmitting">
        <Plus v-if="!isSubmitting" class="h-4 w-4" aria-hidden="true" />
        <span :class="{ 'ml-1.5': !isSubmitting }">{{ isSubmitting ? 'Adding…' : 'Add' }}</span>
      </Button>
    </div>
    <FieldError id="create-task-error" :message="errors.title ?? ''" />
  </form>
</template>
