import axios from 'axios'

// A única instância do Axios do projeto. Nenhum componente importa o Axios diretamente.
// withCredentials: o cookie de sessão vai junto em toda requisição.
const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

export default api
