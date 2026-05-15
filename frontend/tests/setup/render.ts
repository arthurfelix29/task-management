import { render, type RenderOptions } from '@testing-library/vue'
import { createPinia, setActivePinia } from 'pinia'
import { createTestingPinia, type TestingOptions } from '@pinia/testing'
import type { Component } from 'vue'

export type RenderWithProvidersOptions = RenderOptions<unknown> & {
  testingPinia?: TestingOptions | false
}

export function renderWithProviders(
  component: Component,
  { testingPinia, global, ...options }: RenderWithProvidersOptions = {},
) {
  const plugins = global?.plugins ?? []
  if (testingPinia === false) {
    setActivePinia(createPinia())
  } else {
    plugins.push(createTestingPinia({ stubActions: false, ...testingPinia }))
  }

  return render(component, {
    ...options,
    global: { ...global, plugins },
  })
}
