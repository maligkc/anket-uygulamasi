import { useEffect, useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getPublicForm, submitPublicResponse } from '../api/publicApi'
import { getErrorMessage } from '../utils/getErrorMessage'
import { QUESTION_TYPES } from '../types/questionTypes'

export default function PublicFormPage() {
  const { shareKey } = useParams()

  const [form, setForm] = useState(null)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')
  const [answers, setAnswers] = useState({})

  useEffect(() => {
    const loadPublicForm = async () => {
      try {
        const data = await getPublicForm(shareKey)
        setForm(data)
      } catch (loadError) {
        setError(getErrorMessage(loadError, 'Form yüklenemedi.'))
      } finally {
        setLoading(false)
      }
    }

    loadPublicForm()
  }, [shareKey])

  const sortedQuestions = useMemo(() => {
    if (!form) {
      return []
    }

    return form.questions.slice().sort((a, b) => a.order - b.order)
  }, [form])

  const handleMultipleChoiceChange = (questionId, optionId, checked) => {
    setAnswers((prev) => {
      const current = prev[questionId] ?? []

      const next = checked
        ? [...current, optionId]
        : current.filter((id) => id !== optionId)

      return {
        ...prev,
        [questionId]: next,
      }
    })
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setSuccessMessage('')

    if (!form) {
      return
    }

    try {
      setSubmitting(true)

      const payload = {
        answers: sortedQuestions.map((question) => {
          if (
            question.type === QUESTION_TYPES.SHORT_TEXT ||
            question.type === QUESTION_TYPES.PARAGRAPH
          ) {
            return {
              questionId: question.id,
              value: answers[question.id] || '',
              selectedOptionIds: [],
            }
          }

          if (question.type === QUESTION_TYPES.SINGLE_CHOICE) {
            return {
              questionId: question.id,
              value: '',
              selectedOptionIds: answers[question.id] ? [answers[question.id]] : [],
            }
          }

          return {
            questionId: question.id,
            value: '',
            selectedOptionIds: answers[question.id] || [],
          }
        }),
      }

      await submitPublicResponse(shareKey, payload)
      setSuccessMessage('Yanıtınız gönderildi. Teşekkürler!')
      setAnswers({})
    } catch (submitError) {
      setError(getErrorMessage(submitError, 'Yanıt gönderilemedi.'))
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) {
    return <div className="public-page">Form yükleniyor...</div>
  }

  if (!form) {
    return <div className="public-page error-text">{error || 'Form bulunamadı.'}</div>
  }

  return (
    <div className="public-page">
      <form className="card public-form" onSubmit={handleSubmit}>
        <h1>{form.title}</h1>
        {form.description ? <p>{form.description}</p> : null}

        {sortedQuestions.map((question) => (
          <section className="public-question" key={question.id}>
            <h3>
              {question.title} {question.isRequired ? '*' : ''}
            </h3>

            {(question.type === QUESTION_TYPES.SHORT_TEXT ||
              question.type === QUESTION_TYPES.PARAGRAPH) && (
              <textarea
                rows={question.type === QUESTION_TYPES.PARAGRAPH ? 4 : 2}
                value={answers[question.id] || ''}
                onChange={(event) =>
                  setAnswers((prev) => ({
                    ...prev,
                    [question.id]: event.target.value,
                  }))
                }
              />
            )}

            {question.type === QUESTION_TYPES.SINGLE_CHOICE && (
              <div className="radio-list">
                {question.options.map((option) => (
                  <label className="checkbox-label" key={option.id}>
                    <input
                      checked={answers[question.id] === option.id}
                      name={`single-${question.id}`}
                      onChange={() =>
                        setAnswers((prev) => ({ ...prev, [question.id]: option.id }))
                      }
                      type="radio"
                    />
                    {option.value}
                  </label>
                ))}
              </div>
            )}

            {question.type === QUESTION_TYPES.MULTIPLE_CHOICE && (
              <div className="checkbox-list">
                {question.options.map((option) => (
                  <label className="checkbox-label" key={option.id}>
                    <input
                      checked={(answers[question.id] || []).includes(option.id)}
                      onChange={(event) =>
                        handleMultipleChoiceChange(
                          question.id,
                          option.id,
                          event.target.checked,
                        )
                      }
                      type="checkbox"
                    />
                    {option.value}
                  </label>
                ))}
              </div>
            )}
          </section>
        ))}

        {error && <p className="error-text">{error}</p>}
        {successMessage && <p className="success-text">{successMessage}</p>}

        <button disabled={submitting} type="submit">
          {submitting ? 'Gönderiliyor...' : 'Yanıtı gönder'}
        </button>
      </form>
    </div>
  )
}
