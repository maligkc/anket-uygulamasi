import { useEffect, useState } from 'react'
import { isChoiceType, QUESTION_TYPE_OPTIONS } from '../types/questionTypes'

function toDraft(question) {
  return {
    title: question.title,
    type: question.type,
    isRequired: question.isRequired,
    order: question.order,
    options: question.options.length
      ? question.options.map((option) => option.value)
      : ['', ''],
  }
}

export default function QuestionEditorCard({ question, onSave, onDelete }) {
  const [isEditing, setIsEditing] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [draft, setDraft] = useState(() => toDraft(question))
  const [error, setError] = useState('')

  useEffect(() => {
    setDraft(toDraft(question))
    setIsEditing(false)
    setError('')
  }, [question])

  const showOptions = isChoiceType(draft.type)

  const handleSave = async () => {
    setError('')

    try {
      setIsSaving(true)

      await onSave({
        title: draft.title,
        type: draft.type,
        isRequired: draft.isRequired,
        order: Number(draft.order),
        options: showOptions ? draft.options : [],
      })

      setIsEditing(false)
    } catch (saveError) {
      setError(saveError.message)
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <section className="card question-card">
      {isEditing ? (
        <>
          <label>
            Soru
            <input
              value={draft.title}
              onChange={(event) =>
                setDraft((prev) => ({ ...prev, title: event.target.value }))
              }
            />
          </label>

          <div className="row">
            <label>
              Tür
              <select
                value={draft.type}
                onChange={(event) =>
                  setDraft((prev) => {
                    const type = event.target.value
                    const resetOptions =
                      isChoiceType(type) && prev.options.length < 2
                        ? ['', '']
                        : prev.options

                    return { ...prev, type, options: resetOptions }
                  })
                }
              >
                {QUESTION_TYPE_OPTIONS.map((item) => (
                  <option key={item.value} value={item.value}>
                    {item.label}
                  </option>
                ))}
              </select>
            </label>

            <label>
              Sıra
              <input
                min={1}
                type="number"
                value={draft.order}
                onChange={(event) =>
                  setDraft((prev) => ({ ...prev, order: event.target.value }))
                }
              />
            </label>
          </div>

          <label className="checkbox-label">
            <input
              checked={draft.isRequired}
              onChange={(event) =>
                setDraft((prev) => ({ ...prev, isRequired: event.target.checked }))
              }
              type="checkbox"
            />
            Zorunlu soru
          </label>

          {showOptions && (
            <div className="options-block">
              <h4>Seçenekler</h4>
              {draft.options.map((option, index) => (
                <div className="row" key={`${question.id}-opt-${index}`}>
                  <input
                    placeholder={`Seçenek ${index + 1}`}
                    value={option}
                    onChange={(event) => {
                      const nextOptions = [...draft.options]
                      nextOptions[index] = event.target.value
                      setDraft((prev) => ({ ...prev, options: nextOptions }))
                    }}
                  />
                  <button
                    className="danger-button"
                    disabled={draft.options.length <= 2}
                    onClick={() => {
                      setDraft((prev) => ({
                        ...prev,
                        options: prev.options.filter((_, i) => i !== index),
                      }))
                    }}
                    type="button"
                  >
                    Kaldır
                  </button>
                </div>
              ))}

              <button
                className="secondary-button"
                onClick={() =>
                  setDraft((prev) => ({ ...prev, options: [...prev.options, ''] }))
                }
                type="button"
              >
                Seçenek ekle
              </button>
            </div>
          )}

          {error && <p className="error-text">{error}</p>}

          <div className="row actions-row">
            <button disabled={isSaving} onClick={handleSave} type="button">
              {isSaving ? 'Kaydediliyor...' : 'Soruyu kaydet'}
            </button>
            <button
              className="secondary-button"
              onClick={() => {
                setDraft(toDraft(question))
                setIsEditing(false)
                setError('')
              }}
              type="button"
            >
              İptal
            </button>
          </div>
        </>
      ) : (
        <>
          <div className="question-header">
            <h3>
              {question.order}. {question.title}
            </h3>
            <div className="row">
              <button className="secondary-button" onClick={() => setIsEditing(true)} type="button">
                Düzenle
              </button>
              <button className="danger-button" onClick={onDelete} type="button">
                Sil
              </button>
            </div>
          </div>

          <p className="meta-text">
            {question.type} {question.isRequired ? '• Zorunlu' : '• İsteğe bağlı'}
          </p>

          {question.options.length > 0 && (
            <ul className="options-list">
              {question.options
                .slice()
                .sort((a, b) => a.order - b.order)
                .map((option) => (
                  <li key={option.id}>{option.value}</li>
                ))}
            </ul>
          )}
        </>
      )}
    </section>
  )
}
