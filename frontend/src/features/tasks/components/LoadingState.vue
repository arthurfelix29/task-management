<script setup lang="ts">
const SKELETON_ROWS = 4
</script>

<template>
  <ul aria-busy="true" aria-label="Loading tasks" class="space-y-2">
    <li
      v-for="n in SKELETON_ROWS"
      :key="n"
      class="flex h-12 animate-pulse items-center gap-3 rounded-md border border-border bg-surface-elevated px-3"
    >
      <div class="h-4 w-4 rounded bg-border-strong" />
      <div class="h-3 w-2/3 rounded bg-border-strong" />
    </li>
  </ul>
</template>
