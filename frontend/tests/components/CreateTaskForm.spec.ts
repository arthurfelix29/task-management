import { afterEach, describe, expect, it } from 'vitest'
import { screen, waitFor } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'
import { HttpResponse, http } from 'msw'
import CreateTaskForm from '@/features/tasks/components/CreateTaskForm.vue'
import { useTasksStore } from '@/features/tasks/stores/tasks.store'
import { server } from '../setup/msw'
import { renderWithProviders } from '../setup/render'

afterEach(() => {
  server.resetHandlers()
})

describe('CreateTaskForm', () => {
  it('When_SubmittingValidTitle_Should_CallStoreCreateAndResetForm', async () => {
    const user = userEvent.setup()
    renderWithProviders(CreateTaskForm)
    const store = useTasksStore()

    const input = screen.getByLabelText(/new task/i)
    await user.type(input, 'Buy milk')
    await user.click(screen.getByRole('button', { name: /add/i }))

    await waitFor(() => expect(store.create).toHaveBeenCalledWith('Buy milk'))
    expect(input).toHaveValue('')
  })

  it('When_SubmittingEmptyTitle_Should_DisplayRequiredError', async () => {
    const user = userEvent.setup()
    renderWithProviders(CreateTaskForm)

    await user.click(screen.getByRole('button', { name: /add/i }))

    expect(await screen.findByRole('alert')).toHaveTextContent(/title is required/i)
  })

  it('When_SubmittingTitleOver200Chars_Should_DisplayMaxLengthError', async () => {
    const user = userEvent.setup()
    renderWithProviders(CreateTaskForm)
    const input = screen.getByLabelText(/new task/i) as HTMLInputElement
    input.removeAttribute('maxlength')

    await user.type(input, 'x'.repeat(201))
    await user.click(screen.getByRole('button', { name: /add/i }))

    expect(await screen.findByRole('alert')).toHaveTextContent(/200 characters/i)
  })

  it('When_ApiReturns422_Should_DisplayServerErrorOnField', async () => {
    server.use(
      http.post('/api/v1/tasks', () =>
        HttpResponse.json(
          { status: 422, title: 'Validation failed', errors: { Title: ['Title is reserved'] } },
          { status: 422 },
        ),
      ),
    )

    const user = userEvent.setup()
    renderWithProviders(CreateTaskForm, { testingPinia: false })

    const input = screen.getByLabelText(/new task/i)
    await user.type(input, 'admin')
    await user.click(screen.getByRole('button', { name: /add/i }))

    expect(await screen.findByRole('alert')).toHaveTextContent(/title is reserved/i)
  })
})
