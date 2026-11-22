import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato.js'

// Componente de apresentação: só recebe dados por props.
export default function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body d-flex flex-column">
        <span className="badge text-bg-secondary align-self-start mb-2">{jogo.nomePlataforma}</span>
        <h2 className="h5 card-title">
          <Link className="stretched-link text-decoration-none" to={`/jogos/${jogo.id}`}>
            {jogo.nome}
          </Link>
        </h2>

        <p className="card-text text-muted small mb-2">
          {jogo.ano != null && <span>{jogo.ano} &middot; </span>}
          {jogo.descricaoCondicao}
        </p>

        {jogo.nomesGeneros.length > 0 && (
          <div className="mb-2">
            {jogo.nomesGeneros.map((genero) => (
              <span key={genero} className="badge rounded-pill text-bg-light border me-1">
                {genero}
              </span>
            ))}
          </div>
        )}

        <div className="mt-auto d-flex justify-content-between align-items-center">
          <strong className="fs-5">{formatarPreco(jogo.preco)}</strong>
          {esgotado && <span className="badge text-bg-danger">Esgotado</span>}
        </div>
      </div>
    </div>
  )
}
