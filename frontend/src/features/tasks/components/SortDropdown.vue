<script setup lang="ts">
import { type Component, computed } from 'vue'
import { storeToRefs } from 'pinia'
import {
  Listbox,
  ListboxButton,
  ListboxOption,
  ListboxOptions,
} from '@headlessui/vue'
import {
  ArrowDownAZ,
  ArrowDownWideNarrow,
  ArrowDownZA,
  ArrowUpNarrowWide,
  Check,
  ChevronDown,
} from '@lucide/vue'
import { useTasksStore } from '@/features/tasks/stores/tasks.store'
import type { SortOption } from '@/features/tasks/types/task'
import { cn } from '@/shared/lib/cn'

type SortMeta = { value: SortOption; label: string; icon: Component }

const options: readonly SortMeta[] = [
  { value: 'newest', label: 'Newest', icon: ArrowDownWideNarrow },
  { value: 'oldest', label: 'Oldest', icon: ArrowUpNarrowWide },
  { value: 'name-asc', label: 'Name A→Z', icon: ArrowDownAZ },
  { value: 'name-desc', label: 'Name Z→A', icon: ArrowDownZA },
]

const store = useTasksStore()
const { sortBy } = storeToRefs(store)

const currentOption = computed(() => options.find((o) => o.value === sortBy.value) ?? options[0]!)

const value = computed({
  get: () => sortBy.value,
  set: (next: SortOption) => store.setSortBy(next),
})
</script>

<template>
  <Listbox v-model="value" as="div" class="relative">
    <ListboxButton
      class="inline-flex items-center gap-2 rounded-md border border-border-strong bg-transparent px-3 py-1.5 text-sm text-foreground shadow-sm transition hover:bg-surface-elevated focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-focus-ring"
      aria-label="Sort tasks"
    >
      <component :is="currentOption.icon" class="h-4 w-4 text-muted-foreground" aria-hidden="true" />
      <span>{{ currentOption.label }}</span>
      <ChevronDown class="h-4 w-4 text-muted-foreground" aria-hidden="true" />
    </ListboxButton>

    <Transition
      enter-active-class="ease-out duration-150"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="ease-in duration-150"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <ListboxOptions
        class="absolute right-0 z-20 mt-2 w-48 origin-top-right overflow-hidden rounded-lg border border-border bg-surface-overlay shadow-lg focus:outline-none"
      >
        <ListboxOption
          v-for="option in options"
          :key="option.value"
          :value="option.value"
          as="template"
          v-slot="{ active, selected }"
        >
          <li
            :class="
              cn(
                'flex cursor-pointer items-center gap-3 px-3 py-2 text-sm transition',
                active ? 'bg-surface-elevated' : 'bg-transparent',
                selected ? 'font-medium text-foreground' : 'text-foreground',
              )
            "
          >
            <component :is="option.icon" class="h-4 w-4 text-muted-foreground" aria-hidden="true" />
            <span class="flex-1">{{ option.label }}</span>
            <Check v-if="selected" class="h-4 w-4 text-primary" aria-hidden="true" />
          </li>
        </ListboxOption>
      </ListboxOptions>
    </Transition>
  </Listbox>
</template>
