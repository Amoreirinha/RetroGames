import api from './api.js'

// Uma função por endpoint. Cada uma já devolve o "data" do envelope da API.
// Gravação e login entram na aula 8.

// Remove parâmetros vazios, para não mandar ?busca=&idPlataforma=
function limparParametros(parametros) {
  return Object.fromEntries(
    Object.entries(parametros).filter(([, valor]) => valor !== '' && valor != null),
  )
}

export const jogoApi = {
  listar: (parametros = {}) =>
    api.get('/Jogo', { params: limparParametros(parametros) }).then((resposta) => resposta.data.data),
  buscarPorId: (id) => api.get(`/Jogo/${id}`).then((resposta) => resposta.data.data),
}

export const plataformaApi = {
  listar: () => api.get('/Plataforma').then((resposta) => resposta.data.data),
}

export const generoApi = {
  listar: () => api.get('/Genero').then((resposta) => resposta.data.data),
}

export const condicaoJogoApi = {
  listar: () => api.get('/CondicaoJogo').then((resposta) => resposta.data.data),
}
