// Transforma uma falha de requisição em mensagem para o usuário.
export function obterMensagemErro(erro) {
  // Sem resposta: a API está fora do ar ou a rede caiu.
  if (!erro || !erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  // Com resposta: usa a mensagem do envelope (errorMessage).
  return erro.response.data?.errorMessage ?? 'Ocorreu um erro inesperado. Tente novamente.'
}
