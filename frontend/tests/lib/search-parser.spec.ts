import { describe, expect, it } from 'vitest'
import { matchesTask, parseSearch, resolveLocaleDateOrder } from '@/features/tasks/lib/search-parser'

const today = new Date(2026, 4, 15)
const may14 = new Date(2026, 4, 14)
const may15 = new Date(2026, 4, 15)

describe('parseSearch — empty input returns no tokens', () => {
  it.each([
    { query: '', scenario: 'emptyString' },
    { query: '   ', scenario: 'whitespaceOnly' },
    { query: '\t\n  ', scenario: 'tabsAndNewlines' },
  ])('When_QueryIs_$scenario_Should_ReturnEmptyTokens', ({ query }) => {
    expect(parseSearch(query, 'en-US', today)).toEqual({ dates: [], texts: [] })
  })
})

describe('parseSearch — text-only tokens', () => {
  it.each([
    { query: 'hello', expected: ['hello'], scenario: 'lowercaseAscii' },
    { query: 'HELLO', expected: ['HELLO'], scenario: 'uppercasePreservedAsText' },
    { query: 'filha', expected: ['filha'], scenario: 'unicodePortuguese' },
    { query: '  hello  ', expected: ['hello'], scenario: 'leadingTrailingWhitespaceTrimmed' },
  ])('When_QueryIs_$scenario_Should_ReturnSingleTextToken', ({ query, expected }) => {
    const result = parseSearch(query, 'en-US', today)
    expect(result.dates).toEqual([])
    expect(result.texts).toEqual(expected)
  })
})

describe('parseSearch — DMY locale date parsing', () => {
  it.each([
    { query: '14/05', scenario: 'slashSeparator', expected: may14 },
    { query: '14-05', scenario: 'dashSeparator', expected: may14 },
    { query: '14.05', scenario: 'dotSeparator', expected: may14 },
    { query: '14/05/2026', scenario: 'fullYearExplicit', expected: may14 },
    { query: '1/1', scenario: 'singleDigitDayMonth', expected: new Date(2026, 0, 1) },
  ])('When_QueryIs_$scenario_Should_ParseAsDate', ({ query, expected }) => {
    const result = parseSearch(query, 'pt-BR', today)
    expect(result.texts).toEqual([])
    expect(result.dates).toHaveLength(1)
    expect(result.dates[0]?.getTime()).toBe(expected.getTime())
  })
})

describe('parseSearch — MDY locale date parsing', () => {
  it.each([
    { query: '5/14', locale: 'en-US', scenario: 'enUsSlash', expected: may14 },
    { query: '5/14', locale: 'en-PH', scenario: 'enPhSlash', expected: may14 },
    { query: '5-14', locale: 'en-US', scenario: 'enUsDash', expected: may14 },
    { query: '5/14/2026', locale: 'en-US', scenario: 'enUsFullYear', expected: may14 },
  ])('When_QueryIs_$scenario_Should_ParseAsDate', ({ query, locale, expected }) => {
    const result = parseSearch(query, locale, today)
    expect(result.texts).toEqual([])
    expect(result.dates).toHaveLength(1)
    expect(result.dates[0]?.getTime()).toBe(expected.getTime())
  })
})

describe('parseSearch — invalid date falls to text fallback', () => {
  it.each([
    { query: '32/13', scenario: 'dayAndMonthOutOfRange' },
    { query: '30/02', scenario: 'feb30NotALeapDay' },
    { query: '99/99/9999', scenario: 'allComponentsOutOfRange' },
    { query: '0/0', scenario: 'zerosForDayAndMonth' },
  ])('When_QueryIs_$scenario_Should_FallToText', ({ query }) => {
    const result = parseSearch(query, 'pt-BR', today)
    expect(result.dates).toEqual([])
    expect(result.texts).toEqual([query])
  })
})

describe('parseSearch — ambiguous tokens stay as text', () => {
  it.each([
    { query: '5', scenario: 'bareSingleDigit' },
    { query: '2026', scenario: 'bareFourDigitYearWithoutSeparator' },
    { query: 'may', scenario: 'monthNameWord' },
    { query: '14/05/26', scenario: 'twoDigitYearIsNotAccepted' },
  ])('When_QueryIs_$scenario_Should_FallToText', ({ query }) => {
    const result = parseSearch(query, 'pt-BR', today)
    expect(result.dates).toEqual([])
    expect(result.texts).toEqual([query])
  })
})

describe('parseSearch — locale fallback to DMY for unknown or empty locale', () => {
  it.each([
    { locale: '', scenario: 'emptyLocale' },
    { locale: 'xx-XX', scenario: 'unknownLocale' },
  ])('When_LocaleIs_$scenario_Should_TreatAsDMY', ({ locale }) => {
    expect(resolveLocaleDateOrder(locale)).toBe('DMY')
    const result = parseSearch('14/05', locale, today)
    expect(result.dates).toHaveLength(1)
    expect(result.dates[0]?.getTime()).toBe(may14.getTime())
  })
})

describe('matchesTask — date matching uses local calendar day', () => {
  const tokens = { dates: [may15], texts: [] }

  it('When_TaskCreatedAtStartOfDay_Should_MatchSameDayDateToken', () => {
    const task = { title: 'Anything', createdAt: new Date(2026, 4, 15, 0, 0, 0) }
    expect(matchesTask(task, tokens, today)).toBe(true)
  })

  it('When_TaskCreatedAtEndOfDay_Should_MatchSameDayDateToken', () => {
    const task = { title: 'Anything', createdAt: new Date(2026, 4, 15, 23, 59, 59) }
    expect(matchesTask(task, tokens, today)).toBe(true)
  })

  it('When_TaskCreatedOneDayLater_ShouldNot_MatchDateToken', () => {
    const task = { title: 'Anything', createdAt: new Date(2026, 4, 16, 0, 0, 1) }
    expect(matchesTask(task, tokens, today)).toBe(false)
  })
})

describe('matchesTask — date and text tokens combine with AND', () => {
  const tokens = parseSearch('15/05 buy', 'pt-BR', today)

  it('When_DateAndTextBothMatch_Should_ReturnTrue', () => {
    const task = { title: 'Buy groceries', createdAt: new Date(2026, 4, 15, 10, 0) }
    expect(matchesTask(task, tokens, today)).toBe(true)
  })

  it('When_DateMatchesButTextDoesNot_ShouldNot_Match', () => {
    const task = { title: 'Sell stocks', createdAt: new Date(2026, 4, 15, 10, 0) }
    expect(matchesTask(task, tokens, today)).toBe(false)
  })

  it('When_TextMatchesButDateDoesNot_ShouldNot_Match', () => {
    const task = { title: 'Buy groceries', createdAt: new Date(2026, 4, 14, 10, 0) }
    expect(matchesTask(task, tokens, today)).toBe(false)
  })
})

describe('matchesTask — multiple tokens and case-insensitive text', () => {
  it('When_MultipleTextsAllAppearInTitle_Should_Match', () => {
    const tokens = parseSearch('buy milk', 'en-US', today)
    const task = { title: 'Buy fresh milk', createdAt: new Date(2026, 4, 15) }
    expect(matchesTask(task, tokens, today)).toBe(true)
  })

  it('When_OneOfMultipleTextsIsMissing_ShouldNot_Match', () => {
    const tokens = parseSearch('buy milk', 'en-US', today)
    const task = { title: 'Buy bread', createdAt: new Date(2026, 4, 15) }
    expect(matchesTask(task, tokens, today)).toBe(false)
  })

  it('When_MultipleDatesAndTaskMatchesAtLeastOne_Should_Match', () => {
    const tokens = parseSearch('14/05 15/05', 'pt-BR', today)
    const task = { title: 'Anything', createdAt: new Date(2026, 4, 15, 10, 0) }
    expect(matchesTask(task, tokens, today)).toBe(true)
  })

  it('When_TextIsUpperCaseAndTitleIsLowerCase_Should_MatchCaseInsensitively', () => {
    const tokens = parseSearch('GROCERIES', 'en-US', today)
    const task = { title: 'Buy groceries today', createdAt: new Date(2026, 4, 15) }
    expect(matchesTask(task, tokens, today)).toBe(true)
  })
})
