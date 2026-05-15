<script setup lang="ts">
import TaskList from '@/features/tasks/components/TaskList.vue'
import Toast from '@/shared/ui/Toast.vue'
import Button from '@/shared/ui/Button.vue'
import { useTheme } from '@/composables/useTheme'

const { theme, toggle } = useTheme()
</script>

<template>
  <a href="#main" class="sr-only focus:not-sr-only focus:fixed focus:left-2 focus:top-2 focus:z-50 focus:rounded focus:bg-primary focus:px-3 focus:py-1 focus:text-primary-foreground">
    Skip to main content
  </a>
  <main id="main" class="mx-auto max-w-2xl px-4 py-10">
    <header class="mb-8 flex items-start justify-between gap-4">
      <div>
        <h1 class="text-3xl font-semibold tracking-tight text-foreground">Tasks</h1>
        <p class="mt-1 text-sm text-muted-foreground">A small task list. Add, toggle, delete.</p>
      </div>
      <Button
        variant="secondary"
        size="sm"
        :aria-label="theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'"
        @click="toggle"
      >
        {{ theme === 'dark' ? 'Light' : 'Dark' }}
      </Button>
    </header>
    <TaskList />
  </main>
  <Toast />
</template>
