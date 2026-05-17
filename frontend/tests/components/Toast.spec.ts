import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { nextTick } from 'vue'
import { render, screen } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'
import Toast from '@/shared/ui/Toast.vue'
import { useToast } from '@/composables/useToast'

function resetToasts() {
  const { toasts, progress } = useToast()
  toasts.value = []
  progress.value = {}
}

beforeEach(() => {
  resetToasts()
})

afterEach(() => {
  vi.useRealTimers()
  resetToasts()
})

describe('Toast', () => {
  it('When_KindIsSuccess_Should_HaveRoleStatus', async () => {
    render(Toast)
    useToast().success('Saved')

    await nextTick()

    const toastEl = await screen.findByRole('status')
    expect(toastEl).toHaveTextContent('Saved')
  })

  it('When_KindIsError_Should_HaveRoleAlert', async () => {
    render(Toast)
    useToast().error('Broken')

    await nextTick()

    const toastEl = await screen.findByRole('alert')
    expect(toastEl).toHaveTextContent('Broken')
  })

  it('When_HoveringToast_Should_PauseAutoDismiss', async () => {
    vi.useFakeTimers({
      toFake: ['setTimeout', 'clearTimeout', 'requestAnimationFrame', 'cancelAnimationFrame', 'performance'],
    })
    const user = userEvent.setup({ advanceTimers: vi.advanceTimersByTime })
    render(Toast)
    useToast().success('Pause me', 1000)

    await vi.advanceTimersByTimeAsync(200)
    const toastEl = await screen.findByRole('status')

    await user.hover(toastEl)
    await vi.advanceTimersByTimeAsync(3000)

    expect(screen.queryByRole('status')).toBeInTheDocument()
    expect(screen.getByText('Pause me')).toBeInTheDocument()
  })

  it('When_ClickingDismiss_Should_RemoveToastImmediately', async () => {
    const user = userEvent.setup()
    render(Toast)
    useToast().info('Dismiss me')

    await nextTick()
    const dismissButton = await screen.findByRole('button', { name: /dismiss notification/i })

    await user.click(dismissButton)

    expect(screen.queryByText('Dismiss me')).not.toBeInTheDocument()
  })

  it('When_ToastDismissedThenNewOneCreated_Should_KeepProgressAnimating', async () => {
    vi.useFakeTimers({
      toFake: ['setTimeout', 'clearTimeout', 'requestAnimationFrame', 'cancelAnimationFrame', 'performance'],
    })
    render(Toast)
    const api = useToast()

    const firstId = api.success('first', 1000)
    await vi.advanceTimersByTimeAsync(50)
    api.dismiss(firstId)

    const secondId = api.success('second', 1000)
    await vi.advanceTimersByTimeAsync(100)
    const earlyRatio = api.progress.value[secondId]
    expect(earlyRatio).toBeDefined()
    expect(earlyRatio).toBeLessThan(1)

    await vi.advanceTimersByTimeAsync(300)
    const laterRatio = api.progress.value[secondId]
    expect(laterRatio).toBeDefined()
    expect(laterRatio!).toBeLessThan(earlyRatio!)
  })
})
