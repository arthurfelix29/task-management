<script setup lang="ts">
interface Props {
  completed: number
  total: number
}

defineProps<Props>()
</script>

<template>
  <p class="text-sm text-muted-foreground">{{ completed }} of {{ total }} completed</p>
</template>
