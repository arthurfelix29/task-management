<script setup lang="ts">
import Button from '@/shared/ui/Button.vue'

interface Props {
  message: string
  canRetry?: boolean
}

withDefaults(defineProps<Props>(), {
  canRetry: true,
})

const emit = defineEmits<{
  retry: []
}>()
</script>

<template>
  <div
    role="alert"
    aria-live="assertive"
    class="flex flex-col items-start gap-3 rounded-md border border-danger bg-surface-elevated px-4 py-3 text-sm text-foreground"
  >
    <p>{{ message }}</p>
    <Button v-if="canRetry" size="sm" variant="secondary" @click="emit('retry')">
      Try again
    </Button>
  </div>
</template>
