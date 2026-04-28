import client from './client'

export async function getPublicForm(shareKey) {
  const response = await client.get(`/api/public/forms/${shareKey}`)
  return response.data
}

export async function submitPublicResponse(shareKey, payload) {
  const response = await client.post(`/api/public/forms/${shareKey}/responses`, payload)
  return response.data
}
