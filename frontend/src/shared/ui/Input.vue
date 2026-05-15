<script setup lang="ts">
import { useId } from 'vue'
import { cn } from '@/shared/lib/cn'

interface Props {
  modelValue: string
  label: string
  placeholder?: string
  disabled?: boolean
  maxlength?: number
  invalid?: boolean
  errorId?: string
  hideLabel?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '',
  disabled: false,
  invalid: false,
  hideLabel: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const inputId = useId()

function onInput(event: Event) {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>

<template>
  <div class="w-full">
    <label
      :for="inputId"
      :class="cn('mb-1 block text-sm font-medium text-foreground', hideLabel && 'sr-only')"
    >
      {{ label }}
    </label>
    <input
      :id="inputId"
      type="text"
      :value="modelValue"
      :placeholder="placeholder"
      :aria-invalid="invalid || undefined"
      :aria-describedby="errorId"
      :disabled="disabled"
      :maxlength="maxlength"
      :class="
        cn(
          'w-full rounded-md border bg-surface px-3 py-2 text-sm text-foreground shadow-sm transition placeholder:text-muted-foreground focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-focus-ring disabled:opacity-60',
          invalid ? 'border-danger' : 'border-border',
        )
      "
      @input="onInput"
    />
  </div>
</template>
