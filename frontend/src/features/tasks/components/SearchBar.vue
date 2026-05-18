<script setup lang="ts">
  import { ref, watch } from 'vue'
  import { useDebounceFn } from '@vueuse/core'
  import { Search } from '@lucide/vue'
  import { useTasksStore } from '@/features/tasks/stores/tasks.store'

  const store = useTasksStore()
  const localValue = ref(store.searchQuery)

  const commit = useDebounceFn((value: string) => {
    store.setSearchQuery(value)
  }, 150)

  watch(localValue, (next) => {
    void commit(next)
  })
</script>

<template>
  <div class="relative">
    <label for="search-input" class="sr-only">Search tasks by name</label>
    <Search class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
            aria-hidden="true" />
    <input id="search-input"
           v-model="localValue"
           type="search"
           placeholder="Search tasks by name"
           class="w-full rounded-md border border-border bg-surface px-3 py-2 pl-10 text-sm text-foreground shadow-sm transition placeholder:text-muted-foreground focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-focus-ring" />
  </div>
</template>
