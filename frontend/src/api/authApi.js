import client from './client'

export async function register(payload) {
  const response = await client.post('/api/auth/register', payload)
  return response.data
}

export async function login(payload) {
  const response = await client.post('/api/auth/login', payload)
  return response.data
}
