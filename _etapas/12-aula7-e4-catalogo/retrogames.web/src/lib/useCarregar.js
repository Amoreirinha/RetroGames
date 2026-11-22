import { useEffect, useState } from 'react'
import { obterMensagemErro } from '../api/erros.js'

// Busca dados e devolve os três estados da tela: carregando, erro e dados.
// Refaz a busca quando algum valor de "dependencias" muda ou quando "recarregar" é chamado.
// O resultado guarda a "chave" da busca que o gerou: enquanto a chave atual for diferente,
// a tela está carregando (assim não há setState síncrono dentro do efeito).
export function useCarregar(buscar, dependencias) {
  const [tentativa, setTentativa] = useState(0)
  const [resultado, setResultado] = useState(null)
  const chave = `${JSON.stringify(dependencias)}#${tentativa}`

  useEffect(() => {
    let cancelado = false

    buscar()
      .then((dados) => {
        if (!cancelado) setResultado({ chave, dados, erro: null })
      })
      .catch((erro) => {
        if (!cancelado) setResultado({ chave, dados: null, erro: obterMensagemErro(erro) })
      })

    // Ignora respostas de buscas antigas (ex.: o usuário trocou o filtro no meio da requisição).
    return () => {
      cancelado = true
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chave])

  const atual = resultado !== null && resultado.chave === chave

  return {
    carregando: !atual,
    dados: atual ? resultado.dados : null,
    erro: atual ? resultado.erro : null,
    recarregar: () => setTentativa((n) => n + 1),
  }
}
