import client from './client'

export async function getMyForms() {
  const response = await client.get('/api/forms')
  return response.data
}

export async function createForm(payload) {
  const response = await client.post('/api/forms', payload)
  return response.data
}

export async function getFormById(formId) {
  const response = await client.get(`/api/forms/${formId}`)
  return response.data
}

export async function updateForm(formId, payload) {
  const response = await client.put(`/api/forms/${formId}`, payload)
  return response.data
}

export async function deleteForm(formId) {
  await client.delete(`/api/forms/${formId}`)
}

export async function publishForm(formId) {
  const response = await client.post(`/api/forms/${formId}/publish`)
  return response.data
}

export async function unpublishForm(formId) {
  const response = await client.post(`/api/forms/${formId}/unpublish`)
  return response.data
}

export async function addQuestion(formId, payload) {
  const response = await client.post(`/api/forms/${formId}/questions`, payload)
  return response.data
}

export async function updateQuestion(formId, questionId, payload) {
  const response = await client.put(
    `/api/forms/${formId}/questions/${questionId}`,
    payload,
  )
  return response.data
}

export async function deleteQuestion(formId, questionId) {
  await client.delete(`/api/forms/${formId}/questions/${questionId}`)
}

export async function getFormResponses(formId) {
  const response = await client.get(`/api/forms/${formId}/responses`)
  return response.data
}
