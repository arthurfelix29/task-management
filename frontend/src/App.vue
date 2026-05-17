<script setup lang="ts">
import { Moon, Sun } from '@lucide/vue'
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
        <h1 class="text-3xl font-semibold tracking-tight text-foreground">Tasklist</h1>
        <p class="mt-1 text-sm text-muted-foreground">A small task list, done well.</p>
      </div>
      <Button
        variant="secondary"
        size="sm"
        :aria-label="theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'"
        @click="toggle"
      >
        <Sun v-if="theme === 'dark'" class="h-4 w-4" aria-hidden="true" />
        <Moon v-else class="h-4 w-4" aria-hidden="true" />
      </Button>
    </header>
    <TaskList />
  </main>
  <footer class="mx-auto max-w-2xl px-4 pb-8 pt-12 text-center text-xs text-muted-foreground">
    Built with Vue 3 + .NET 10
  </footer>
  <Toast />
</template>
