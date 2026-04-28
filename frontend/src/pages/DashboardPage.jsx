import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { createForm, deleteForm, getMyForms } from '../api/formsApi'
import AppLayout from '../components/AppLayout'
import { getErrorMessage } from '../utils/getErrorMessage'

export default function DashboardPage() {
  const [forms, setForms] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [createLoading, setCreateLoading] = useState(false)
  const [formData, setFormData] = useState({ title: '', description: '' })

  const publicBaseUrl = useMemo(() => window.location.origin, [])

  const loadForms = async () => {
    setError('')

    try {
      const data = await getMyForms()
      setForms(data)
    } catch (loadError) {
      setError(getErrorMessage(loadError, 'Formlar yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadForms()
  }, [])

  const handleCreateForm = async (event) => {
    event.preventDefault()
    setError('')

    try {
      setCreateLoading(true)
      await createForm(formData)
      setFormData({ title: '', description: '' })
      await loadForms()
    } catch (createError) {
      setError(getErrorMessage(createError, 'Form oluşturulamadı.'))
    } finally {
      setCreateLoading(false)
    }
  }

  const handleDeleteForm = async (formId) => {
    const confirmed = window.confirm('Bu form kalıcı olarak silinecek. Emin misiniz?')

    if (!confirmed) {
      return
    }

    try {
      await deleteForm(formId)
      await loadForms()
    } catch (deleteError) {
      setError(getErrorMessage(deleteError, 'Form silinemedi.'))
    }
  }

  return (
    <AppLayout>
      <section className="dashboard-grid">
        <form className="card" onSubmit={handleCreateForm}>
          <h2>Yeni form oluştur</h2>

          <label>
            Başlık
            <input
              required
              value={formData.title}
              onChange={(event) =>
                setFormData((prev) => ({ ...prev, title: event.target.value }))
              }
            />
          </label>

          <label>
            Açıklama
            <textarea
              rows={3}
              value={formData.description}
              onChange={(event) =>
                setFormData((prev) => ({ ...prev, description: event.target.value }))
              }
            />
          </label>

          <button disabled={createLoading} type="submit">
            {createLoading ? 'Oluşturuluyor...' : 'Form oluştur'}
          </button>
        </form>

        <div>
          <h2>Formlarım</h2>

          {error && <p className="error-text">{error}</p>}

          {loading ? <p className="meta-text">Formlar yükleniyor...</p> : null}

          {!loading && forms.length === 0 ? (
            <p className="meta-text">Henüz form yok. İlk formunuzu oluşturun.</p>
          ) : null}

          <div className="forms-grid">
            {forms.map((form) => (
              <article className="card form-card" key={form.id}>
                <h3>{form.title}</h3>
                <p>{form.description || 'Açıklama yok'}</p>
                <p className="meta-text">
                  {form.isPublished ? 'Yayında' : 'Taslak'} • {form.responseCount} yanıt
                </p>

                {form.isPublished && form.shareKey ? (
                  <a
                    className="inline-link"
                    href={`${publicBaseUrl}/public/forms/${form.shareKey}`}
                    rel="noreferrer"
                    target="_blank"
                  >
                    Formu aç
                  </a>
                ) : null}

                <div className="row actions-row">
                  <Link className="button-link" to={`/forms/${form.id}/edit`}>
                    Düzenle
                  </Link>
                  <Link className="button-link secondary-button" to={`/forms/${form.id}/responses`}>
                    Yanıtlar
                  </Link>
                  <button className="danger-button" onClick={() => handleDeleteForm(form.id)} type="button">
                    Sil
                  </button>
                </div>
              </article>
            ))}
          </div>
        </div>
      </section>
    </AppLayout>
  )
}
