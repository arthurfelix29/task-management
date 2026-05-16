export type SearchTokens = {
  dates: Date[]
  texts: string[]
}

export type DateOrder = 'DMY' | 'MDY'

const MDY_LOCALES = new Set(['en-us', 'en-ph'])

const SEPARATOR_PATTERN = /[/\-.]/
const DIGITS_ONLY = /^\d+$/

export function resolveLocaleDateOrder(locale: string): DateOrder {
  return MDY_LOCALES.has(locale.toLowerCase()) ? 'MDY' : 'DMY'
}

export function parseSearch(query: string, locale: string, now: Date): SearchTokens {
  const order = resolveLocaleDateOrder(locale)
  const rawTokens = query.trim().split(/\s+/).filter((t) => t.length > 0)
  const dates: Date[] = []
  const texts: string[] = []

  for (const token of rawTokens) {
    const parsed = tryParseDate(token, order, now)
    if (parsed !== null) {
      dates.push(parsed)
    } else {
      texts.push(token)
    }
  }

  return { dates, texts }
}

function tryParseDate(token: string, order: DateOrder, now: Date): Date | null {
  if (!SEPARATOR_PATTERN.test(token)) return null

  const parts = token.split(/[/\-.]/)
  if (parts.length !== 2 && parts.length !== 3) return null

  const first = parts[0]
  const second = parts[1]
  if (first === undefined || second === undefined) return null
  if (!DIGITS_ONLY.test(first) || !DIGITS_ONLY.test(second)) return null

  let year: number
  if (parts.length === 3) {
    const yearPart = parts[2]
    if (yearPart === undefined || yearPart.length !== 4 || !DIGITS_ONLY.test(yearPart)) return null
    year = Number(yearPart)
  } else {
    year = now.getFullYear()
  }

  const firstN = Number(first)
  const secondN = Number(second)
  const day = order === 'DMY' ? firstN : secondN
  const month = order === 'DMY' ? secondN : firstN

  if (month < 1 || month > 12) return null
  if (day < 1 || day > daysInMonth(year, month)) return null

  return new Date(year, month - 1, day)
}

function daysInMonth(year: number, month: number): number {
  return new Date(year, month, 0).getDate()
}

export function matchesTask(
  task: { title: string; createdAt: string | Date },
  tokens: SearchTokens,
  _now: Date,
): boolean {
  if (tokens.dates.length === 0 && tokens.texts.length === 0) return true

  const created = task.createdAt instanceof Date ? task.createdAt : new Date(task.createdAt)

  const dateOk =
    tokens.dates.length === 0 || tokens.dates.some((d) => sameCalendarDay(d, created))

  const titleLower = task.title.toLocaleLowerCase()
  const textOk = tokens.texts.every((t) => titleLower.includes(t.toLocaleLowerCase()))

  return dateOk && textOk
}

function sameCalendarDay(a: Date, b: Date): boolean {
  return (
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate()
  )
}
