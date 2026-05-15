import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from '@/App.vue'
import { applyInitialTheme } from '@/composables/useTheme'
import '@/styles.css'

applyInitialTheme()

const app = createApp(App)
app.use(createPinia())

if (import.meta.env.DEV) {
  const VueAxe = await import('vue-axe')
  app.use(VueAxe.default)
}

app.mount('#app')
