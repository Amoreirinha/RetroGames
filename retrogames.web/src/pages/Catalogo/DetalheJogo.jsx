import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'
import { formatarPreco } from '../../lib/formato.js'
import { useCarregar } from '../../lib/useCarregar.js'

export default function DetalheJogo() {
  // O número vem da rota (/jogos/3) e vai para GET /api/Jogo/3.
  const { id } = useParams()
  const { carregando, dados: jogo, erro, recarregar } = useCarregar(() => jogoApi.buscarPorId(id), [id])

  return (
    <>
      <Link to="/" className="btn btn-link px-0 mb-3">
        &larr; Voltar ao catálogo
      </Link>

      {carregando && <Carregando mensagem="Carregando jogo..." />}

      {/* 404 (jogo inexistente ou inativo): mostra o errorMessage do envelope, sem quebrar. */}
      {!carregando && erro && <MensagemErro mensagem={erro} aoTentarNovamente={recarregar} />}

      {!carregando && !erro && jogo && (
        <article className="card shadow-sm">
          <div className="card-body">
            <span className="badge text-bg-secondary mb-2">{jogo.nomePlataforma}</span>
            <h1 className="h3 card-title">{jogo.nome}</h1>

            <p className="text-muted mb-2">
              {jogo.ano != null && <span>{jogo.ano} &middot; </span>}
              Condição: {jogo.descricaoCondicao}
            </p>

            {jogo.generos.length > 0 && (
              <div className="mb-3">
                {jogo.generos.map((genero) => (
                  <span key={genero.id} className="badge rounded-pill text-bg-light border me-1">
                    {genero.nome}
                  </span>
                ))}
              </div>
            )}

            {jogo.descricao ? (
              <p className="card-text">{jogo.descricao}</p>
            ) : (
              <p className="card-text text-muted fst-italic">Este jogo não tem descrição.</p>
            )}

            <hr />

            <div className="d-flex flex-wrap justify-content-between align-items-center gap-2">
              <strong className="fs-3">{formatarPreco(jogo.preco)}</strong>
              {jogo.estoque > 0 ? (
                <span className="text-success">{jogo.estoque} em estoque</span>
              ) : (
                <span className="badge text-bg-danger fs-6">Esgotado</span>
              )}
            </div>
          </div>
        </article>
      )}
    </>
  )
}
