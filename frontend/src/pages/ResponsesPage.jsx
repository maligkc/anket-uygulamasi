import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getFormResponses } from '../api/formsApi'
import AppLayout from '../components/AppLayout'
import { getErrorMessage } from '../utils/getErrorMessage'

export default function ResponsesPage() {
  const { id } = useParams()

  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    const loadResponses = async () => {
      try {
        const responseData = await getFormResponses(id)
        setData(responseData)
      } catch (loadError) {
        setError(getErrorMessage(loadError, 'Yanıtlar yüklenemedi.'))
      } finally {
        setLoading(false)
      }
    }

    loadResponses()
  }, [id])

  return (
    <AppLayout>
      <div className="row">
        <h2>Yanıtlar</h2>
        <Link className="button-link secondary-button" to={`/forms/${id}/edit`}>
          Düzenleyiciye dön
        </Link>
      </div>

      {loading && <p className="meta-text">Yanıtlar yükleniyor...</p>}
      {error && <p className="error-text">{error}</p>}

      {!loading && data && (
        <>
          <section className="card">
            <h3>{data.formTitle}</h3>
            <p className="meta-text">Toplam yanıt: {data.responseCount}</p>
          </section>

          {data.choiceSummaries.length > 0 && (
            <section className="card">
              <h3>Seçenek özeti</h3>

              {data.choiceSummaries.map((summary) => (
                <div className="summary-block" key={summary.questionId}>
                  <h4>{summary.questionTitle}</h4>
                  <ul className="options-list">
                    {summary.options.map((option) => (
                      <li key={option.optionId}>
                        {option.optionValue}: {option.count}
                      </li>
                    ))}
                  </ul>
                </div>
              ))}
            </section>
          )}

          <section>
            <h3>Gönderiler</h3>

            {data.submissions.length === 0 ? (
              <p className="meta-text">Henüz gönderi yok.</p>
            ) : (
              data.submissions.map((submission) => (
                <article className="card" key={submission.responseId}>
                  <p className="meta-text">
                    Gönderilme zamanı: {new Date(submission.submittedAt).toLocaleString('tr-TR')}
                  </p>

                  <div className="answers-list">
                    {submission.answers.map((answer) => (
                      <div className="answer-item" key={answer.questionId}>
                        <strong>{answer.questionTitle}</strong>

                        {answer.value ? <p>{answer.value}</p> : null}

                        {answer.selectedOptions.length > 0 ? (
                          <ul className="options-list">
                            {answer.selectedOptions.map((option) => (
                              <li key={`${answer.questionId}-${option}`}>{option}</li>
                            ))}
                          </ul>
                        ) : null}

                        {!answer.value && answer.selectedOptions.length === 0 ? (
                          <p className="meta-text">Yanıt verilmedi.</p>
                        ) : null}
                      </div>
                    ))}
                  </div>
                </article>
              ))
            )}
          </section>
        </>
      )}
    </AppLayout>
  )
}
