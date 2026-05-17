import { describe, expect, it } from 'vitest'
import { nextTick } from 'vue'
import { render, screen } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'
import ConfirmDeleteModal from '@/features/tasks/components/ConfirmDeleteModal.vue'

async function renderOpenModal() {
  const result = render(ConfirmDeleteModal, { props: { open: true, taskTitle: 'Buy milk' } })
  await nextTick()
  await nextTick()
  return result
}

describe('ConfirmDeleteModal', () => {
  it('When_OpenPropTrue_Should_RenderDialogWithTaskTitle', async () => {
    await renderOpenModal()

    expect(await screen.findByRole('dialog')).toBeInTheDocument()
    expect(screen.getByText(/buy milk/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /^delete$/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /cancel/i })).toBeInTheDocument()
  })

  it('When_ClickingDelete_Should_EmitConfirm', async () => {
    const user = userEvent.setup()
    const { emitted } = await renderOpenModal()

    await user.click(await screen.findByRole('button', { name: /^delete$/i }))

    expect(emitted()['confirm']).toBeDefined()
    expect(emitted()['cancel']).toBeUndefined()
  })

  it('When_ClickingCancel_Should_EmitCancelWithoutConfirm', async () => {
    const user = userEvent.setup()
    const { emitted } = await renderOpenModal()

    await user.click(await screen.findByRole('button', { name: /cancel/i }))

    expect(emitted()['cancel']).toBeDefined()
    expect(emitted()['confirm']).toBeUndefined()
  })

  it('When_PressingEscape_Should_EmitCancelWithoutConfirm', async () => {
    const user = userEvent.setup()
    const { emitted } = await renderOpenModal()

    await user.keyboard('{Escape}')

    expect(emitted()['cancel']).toBeDefined()
    expect(emitted()['confirm']).toBeUndefined()
  })
})
