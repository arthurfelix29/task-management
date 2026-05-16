<script setup lang="ts">
import {
  Dialog,
  DialogDescription,
  DialogPanel,
  DialogTitle,
  TransitionChild,
  TransitionRoot,
} from '@headlessui/vue'
import Button from '@/shared/ui/Button.vue'

interface Props {
  open: boolean
  taskTitle: string
}

defineProps<Props>()

const emit = defineEmits<{
  confirm: []
  cancel: []
}>()
</script>

<template>
  <TransitionRoot appear :show="open" as="template">
    <Dialog as="div" class="relative z-50" @close="emit('cancel')">
      <TransitionChild
        as="template"
        enter="duration-150 ease-out"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="duration-150 ease-in"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-black/40" aria-hidden="true" />
      </TransitionChild>

      <div class="fixed inset-0 flex items-center justify-center p-4">
        <TransitionChild
          as="template"
          enter="duration-150 ease-out"
          enter-from="opacity-0 scale-95"
          enter-to="opacity-100 scale-100"
          leave="duration-150 ease-in"
          leave-from="opacity-100 scale-100"
          leave-to="opacity-0 scale-95"
        >
          <DialogPanel
            class="w-full max-w-sm rounded-lg border border-border bg-surface-overlay p-5 shadow-lg"
          >
            <DialogTitle class="text-base font-semibold text-foreground">Delete task?</DialogTitle>
            <DialogDescription class="mt-2 text-sm text-muted-foreground">
              "{{ taskTitle }}" will be permanently removed.
            </DialogDescription>
            <div class="mt-5 flex justify-end gap-2">
              <Button variant="secondary" size="sm" @click="emit('cancel')">Cancel</Button>
              <Button variant="danger" size="sm" @click="emit('confirm')">Delete</Button>
            </div>
          </DialogPanel>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>
