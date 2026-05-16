<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/zod'
import { Plus } from '@lucide/vue'
import Button from '@/shared/ui/Button.vue'
import Input from '@/shared/ui/Input.vue'
import { createTaskSchema, type CreateTaskInput } from '@/features/tasks/schemas/task.schema'
import { TaskValidationError, useTasksStore } from '@/features/tasks/stores/tasks.store'
import { useToast } from '@/composables/useToast'

const store = useTasksStore()
const toast = useToast()
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

const onSubmit = handleSubmit(async (values) => {
  try {
    await store.create(values.title)
    resetForm()
    focusInput()
    toast.success('Task added.')
  } catch (err) {
    if (err instanceof TaskValidationError) {
      const fieldMessage = err.fieldErrors['Title']?.join(', ')
      if (fieldMessage !== undefined) {
        setErrors({ title: fieldMessage })
        return
      }
    }
    toast.error('Could not add the task. Please try again.')
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
          :maxlength="200"
          :invalid="!!errors.title"
          error-id="create-task-error"
          hide-label
          v-bind="titleAttrs"
        />
      </div>
      <Button type="submit" :disabled="isSubmitting">
        <Plus v-if="!isSubmitting" class="h-4 w-4" aria-hidden="true" />
        <span :class="{ 'ml-1.5': !isSubmitting }">{{ isSubmitting ? 'Adding…' : 'Add' }}</span>
      </Button>
    </div>
    <p
      v-if="errors.title"
      id="create-task-error"
      role="alert"
      aria-live="polite"
      class="text-sm text-danger"
    >
      {{ errors.title }}
    </p>
  </form>
</template>
