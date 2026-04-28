import { useCallback, useEffect, useMemo, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  addQuestion,
  deleteQuestion,
  getFormById,
  publishForm,
  unpublishForm,
  updateForm,
  updateQuestion,
} from '../api/formsApi'
import AppLayout from '../components/AppLayout'
import QuestionEditorCard from '../components/QuestionEditorCard'
import { getErrorMessage } from '../utils/getErrorMessage'
import { isChoiceType, QUESTION_TYPE_OPTIONS, QUESTION_TYPES } from '../types/questionTypes'

const INITIAL_QUESTION = {
  title: '',
  type: QUESTION_TYPES.SHORT_TEXT,
  isRequired: false,
  order: '',
  options: ['', ''],
}

export default function FormEditorPage() {
  const { id } = useParams()

  const [form, setForm] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [statusMessage, setStatusMessage] = useState('')

  const [formDraft, setFormDraft] = useState({ title: '', description: '' })
  const [savingForm, setSavingForm] = useState(false)

  const [newQuestion, setNewQuestion] = useState(INITIAL_QUESTION)
  const [addingQuestion, setAddingQuestion] = useState(false)

  const publicUrl = useMemo(() => {
    if (!form?.shareKey) {
      return ''
    }

    return `${window.location.origin}/public/forms/${form.shareKey}`
  }, [form?.shareKey])

  const loadForm = useCallback(async () => {
    setError('')

    try {
      const data = await getFormById(id)
      setForm(data)
      setFormDraft({
        title: data.title,
        description: data.description || '',
      })
    } catch (loadError) {
      setError(getErrorMessage(loadError, 'Form yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [id])

  useEffect(() => {
    loadForm()
  }, [loadForm])

  const handleSaveForm = async () => {
    setStatusMessage('')
    setError('')

    try {
      setSavingForm(true)
      const updated = await updateForm(id, formDraft)
      setForm(updated)
      setStatusMessage('Form kaydedildi.')
    } catch (saveError) {
      setError(getErrorMessage(saveError, 'Form kaydedilemedi.'))
    } finally {
      setSavingForm(false)
    }
  }

  const handlePublishToggle = async () => {
    setStatusMessage('')
    setError('')

    try {
      const updated = form.isPublished
        ? await unpublishForm(id)
        : await publishForm(id)

      setForm(updated)
      setStatusMessage(updated.isPublished ? 'Form yayınlandı.' : 'Form yayından kaldırıldı.')
    } catch (toggleError) {
      setError(getErrorMessage(toggleError, 'Yayın durumu güncellenemedi.'))
    }
  }

  const handleAddQuestion = async (event) => {
    event.preventDefault()
    setStatusMessage('')
    setError('')

    try {
      setAddingQuestion(true)

      await addQuestion(id, {
        title: newQuestion.title,
        type: newQuestion.type,
        isRequired: newQuestion.isRequired,
        order: newQuestion.order ? Number(newQuestion.order) : undefined,
        options: isChoiceType(newQuestion.type) ? newQuestion.options : [],
      })

      setNewQuestion(INITIAL_QUESTION)
      await loadForm()
      setStatusMessage('Soru eklendi.')
    } catch (addError) {
      setError(getErrorMessage(addError, 'Soru eklenemedi.'))
    } finally {
      setAddingQuestion(false)
    }
  }

  const handleUpdateQuestion = async (questionId, payload) => {
    try {
      await updateQuestion(id, questionId, payload)
      await loadForm()
      setStatusMessage('Soru güncellendi.')
    } catch (updateError) {
      throw new Error(getErrorMessage(updateError, 'Soru güncellenemedi.'))
    }
  }

  const handleDeleteQuestion = async (questionId) => {
    const confirmed = window.confirm('Bu soru silinecek. Emin misiniz?')

    if (!confirmed) {
      return
    }

    setError('')

    try {
      await deleteQuestion(id, questionId)
      await loadForm()
      setStatusMessage('Soru silindi.')
    } catch (deleteError) {
      setError(getErrorMessage(deleteError, 'Soru silinemedi.'))
    }
  }

  if (loading) {
    return (
      <AppLayout>
        <p className="meta-text">Form yükleniyor...</p>
      </AppLayout>
    )
  }

  if (!form) {
    return (
      <AppLayout>
        <p className="error-text">{error || 'Form bulunamadı.'}</p>
      </AppLayout>
    )
  }

  const showOptionEditor = isChoiceType(newQuestion.type)

  return (
    <AppLayout>
      <div className="row">
        <h2>Form düzenleyici</h2>
        <Link className="button-link secondary-button" to={`/forms/${id}/responses`}>
          Yanıtları gör
        </Link>
      </div>

      {error && <p className="error-text">{error}</p>}
      {statusMessage && <p className="success-text">{statusMessage}</p>}

      <section className="card">
        <label>
          Form başlığı
          <input
            value={formDraft.title}
            onChange={(event) =>
              setFormDraft((prev) => ({ ...prev, title: event.target.value }))
            }
          />
        </label>

        <label>
          Açıklama
          <textarea
            rows={3}
            value={formDraft.description}
            onChange={(event) =>
              setFormDraft((prev) => ({ ...prev, description: event.target.value }))
            }
          />
        </label>

        <div className="row actions-row">
          <button disabled={savingForm} onClick={handleSaveForm} type="button">
            {savingForm ? 'Kaydediliyor...' : 'Kaydet'}
          </button>

          <button className="secondary-button" onClick={handlePublishToggle} type="button">
            {form.isPublished ? 'Yayından kaldır' : 'Yayınla'}
          </button>
        </div>

        {form.isPublished && form.shareKey && (
          <div className="share-box">
            <p className="meta-text">Herkese açık bağlantı</p>
            <a href={publicUrl} rel="noreferrer" target="_blank">
              {publicUrl}
            </a>
          </div>
        )}
      </section>

      <section>
        <h3>Sorular</h3>
        {form.questions.length === 0 ? (
          <p className="meta-text">Henüz soru yok.</p>
        ) : (
          form.questions
            .slice()
            .sort((a, b) => a.order - b.order)
            .map((question) => (
              <QuestionEditorCard
                key={question.id}
                question={question}
                onDelete={() => handleDeleteQuestion(question.id)}
                onSave={(payload) => handleUpdateQuestion(question.id, payload)}
              />
            ))
        )}
      </section>

      <section className="card">
        <h3>Soru ekle</h3>

        <form onSubmit={handleAddQuestion}>
          <label>
            Soru
            <input
              required
              value={newQuestion.title}
              onChange={(event) =>
                setNewQuestion((prev) => ({ ...prev, title: event.target.value }))
              }
            />
          </label>

          <div className="row">
            <label>
              Tür
              <select
                value={newQuestion.type}
                onChange={(event) =>
                  setNewQuestion((prev) => ({
                    ...prev,
                    type: event.target.value,
                    options:
                      isChoiceType(event.target.value) && prev.options.length < 2
                        ? ['', '']
                        : prev.options,
                  }))
                }
              >
                {QUESTION_TYPE_OPTIONS.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </label>

            <label>
              Sıra (isteğe bağlı)
              <input
                min={1}
                type="number"
                value={newQuestion.order}
                onChange={(event) =>
                  setNewQuestion((prev) => ({ ...prev, order: event.target.value }))
                }
              />
            </label>
          </div>

          <label className="checkbox-label">
            <input
              checked={newQuestion.isRequired}
              onChange={(event) =>
                setNewQuestion((prev) => ({
                  ...prev,
                  isRequired: event.target.checked,
                }))
              }
              type="checkbox"
            />
            Zorunlu
          </label>

          {showOptionEditor && (
            <div className="options-block">
              <h4>Seçenekler</h4>
              {newQuestion.options.map((option, index) => (
                <div className="row" key={`new-opt-${index}`}>
                  <input
                    placeholder={`Seçenek ${index + 1}`}
                    value={option}
                    onChange={(event) => {
                      const nextOptions = [...newQuestion.options]
                      nextOptions[index] = event.target.value
                      setNewQuestion((prev) => ({ ...prev, options: nextOptions }))
                    }}
                  />
                  <button
                    className="danger-button"
                    disabled={newQuestion.options.length <= 2}
                    onClick={() =>
                      setNewQuestion((prev) => ({
                        ...prev,
                        options: prev.options.filter((_, i) => i !== index),
                      }))
                    }
                    type="button"
                  >
                    Kaldır
                  </button>
                </div>
              ))}

              <button
                className="secondary-button"
                onClick={() =>
                  setNewQuestion((prev) => ({
                    ...prev,
                    options: [...prev.options, ''],
                  }))
                }
                type="button"
              >
                Seçenek ekle
              </button>
            </div>
          )}

          <button disabled={addingQuestion} type="submit">
            {addingQuestion ? 'Ekleniyor...' : 'Soru ekle'}
          </button>
        </form>
      </section>
    </AppLayout>
  )
}
