<script setup lang="ts">
interface Props {
  modelValue: boolean
  label: string
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

function onChange(event: Event) {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.checked)
}
</script>

<template>
  <input
    type="checkbox"
    :checked="modelValue"
    :aria-label="label"
    :disabled="disabled"
    class="h-4 w-4 cursor-pointer rounded border-border bg-surface text-primary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-focus-ring disabled:cursor-not-allowed disabled:opacity-50"
    @change="onChange"
  />
</template>
