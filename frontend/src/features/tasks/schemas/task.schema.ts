import { z } from 'zod'

export const TASK_TITLE_MAX_LENGTH = 200

export const linkSchema = z.object({
  href: z.string(),
  rel: z.string(),
  method: z.string(),
})

export const taskSchema = z.object({
  id: z.string(),
  title: z.string(),
  isCompleted: z.boolean(),
  createdAt: z.string(),
  links: z.array(linkSchema),
})

export const taskListResponseSchema = z.object({
  data: z.array(taskSchema),
  links: z.array(linkSchema),
})

export const createTaskSchema = z.object({
  title: z
    .string()
    .trim()
    .min(1, 'Title is required')
    .max(TASK_TITLE_MAX_LENGTH, `Title must be ${TASK_TITLE_MAX_LENGTH} characters or fewer`),
})

export type CreateTaskInput = z.infer<typeof createTaskSchema>
