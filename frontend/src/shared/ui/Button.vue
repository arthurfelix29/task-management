<script setup lang="ts">
import { computed } from 'vue'
import { cva, type VariantProps } from 'class-variance-authority'
import { cn } from '@/shared/lib/cn'

const button = cva(
  [
    'inline-flex items-center justify-center rounded-md font-medium transition',
    'focus-visible:outline-2 focus-visible:outline-offset-2',
    'disabled:cursor-not-allowed disabled:opacity-60',
  ],
  {
    variants: {
      variant: {
        primary: 'bg-primary text-primary-foreground hover:bg-primary-hover focus-visible:outline-focus-ring',
        secondary: 'border border-border-strong bg-transparent text-foreground hover:bg-surface-elevated focus-visible:outline-focus-ring',
        destructive: 'bg-danger text-danger-foreground hover:bg-danger-hover focus-visible:outline-danger',
        ghost: 'bg-transparent text-muted-foreground hover:bg-surface-elevated hover:text-foreground focus-visible:outline-focus-ring',
      },
      size: {
        sm: 'px-3 py-1.5 text-sm',
        md: 'px-4 py-2 text-sm',
        icon: 'h-9 w-9 p-0',
      },
    },
    defaultVariants: {
      variant: 'primary',
      size: 'md',
    },
  },
)

export type ButtonVariants = VariantProps<typeof button>

interface Props {
  variant?: ButtonVariants['variant']
  size?: ButtonVariants['size']
  type?: 'button' | 'submit'
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  type: 'button',
  disabled: false,
})

const classes = computed(() => cn(button({ variant: props.variant, size: props.size })))
</script>

<template>
  <button :type="type" :disabled="disabled" :class="classes">
    <slot />
  </button>
</template>
