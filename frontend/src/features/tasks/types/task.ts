export type Link = {
  href: string
  rel: string
  method: string
}

export type Task = {
  id: string
  title: string
  isCompleted: boolean
  createdAt: string
  links: Link[]
}

export type TaskListResponse = {
  data: Task[]
  count: number
  links: Link[]
}

export type CreateTaskRequest = {
  title: string
}

export type TaskFilter = 'all' | 'active' | 'completed'
