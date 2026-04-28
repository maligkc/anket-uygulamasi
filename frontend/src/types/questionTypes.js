export const QUESTION_TYPES = {
  SHORT_TEXT: 'ShortText',
  PARAGRAPH: 'Paragraph',
  SINGLE_CHOICE: 'SingleChoice',
  MULTIPLE_CHOICE: 'MultipleChoice',
}

export const QUESTION_TYPE_OPTIONS = [
  { value: QUESTION_TYPES.SHORT_TEXT, label: 'Kısa metin' },
  { value: QUESTION_TYPES.PARAGRAPH, label: 'Paragraf' },
  { value: QUESTION_TYPES.SINGLE_CHOICE, label: 'Tek seçim' },
  { value: QUESTION_TYPES.MULTIPLE_CHOICE, label: 'Çoklu seçim' },
]

export function isChoiceType(type) {
  return (
    type === QUESTION_TYPES.SINGLE_CHOICE ||
    type === QUESTION_TYPES.MULTIPLE_CHOICE
  )
}
