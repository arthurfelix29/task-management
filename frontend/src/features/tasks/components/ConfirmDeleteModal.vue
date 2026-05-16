<script setup lang="ts">
import {
  Dialog,
  DialogDescription,
  DialogPanel,
  DialogTitle,
  TransitionChild,
  TransitionRoot,
} from '@headlessui/vue'
import { AlertTriangle } from '@lucide/vue'
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
        enter="ease-out duration-150"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="ease-in duration-150"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div
          class="fixed inset-0 bg-black/50 backdrop-blur-sm dark:bg-black/70"
          aria-hidden="true"
        />
      </TransitionChild>

      <div class="fixed inset-0 flex items-center justify-center p-4">
        <TransitionChild
          as="template"
          enter="ease-out duration-150"
          enter-from="opacity-0 scale-95"
          enter-to="opacity-100 scale-100"
          leave="ease-in duration-150"
          leave-from="opacity-100 scale-100"
          leave-to="opacity-0 scale-95"
        >
          <DialogPanel
            class="w-full max-w-md rounded-xl border border-border bg-surface-overlay p-6 shadow-2xl"
          >
            <div class="flex justify-center">
              <div
                class="flex h-12 w-12 items-center justify-center rounded-full bg-danger-subtle"
              >
                <AlertTriangle class="h-6 w-6 text-danger" aria-hidden="true" />
              </div>
            </div>

            <DialogTitle class="mt-4 text-center text-lg font-semibold text-foreground">
              Delete this task?
            </DialogTitle>
            <DialogDescription class="mt-2 text-center text-sm text-muted-foreground">
              "{{ taskTitle }}" will be permanently removed.
            </DialogDescription>
            <p class="mt-2 text-center text-xs text-muted-foreground">
              This action cannot be undone.
            </p>

            <div class="mt-6 flex justify-end gap-3 border-t border-border pt-4">
              <Button variant="secondary" size="md" @click="emit('cancel')">Cancel</Button>
              <Button variant="destructive" size="md" @click="emit('confirm')">Delete</Button>
            </div>
          </DialogPanel>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>
