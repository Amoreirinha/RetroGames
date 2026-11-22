// Anterior, números e Próxima. Não aparece quando há uma página só.
const JANELA = 5

export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) return null

  const inicio = Math.max(1, Math.min(pagina - Math.floor(JANELA / 2), totalPaginas - JANELA + 1))
  const fim = Math.min(totalPaginas, inicio + JANELA - 1)
  const numeros = []
  for (let n = inicio; n <= fim; n++) numeros.push(n)

  return (
    <nav aria-label="Paginação do catálogo" className="mt-4">
      <ul className="pagination justify-content-center flex-wrap">
        <li className={`page-item ${pagina <= 1 ? 'disabled' : ''}`}>
          <button type="button" className="page-link" disabled={pagina <= 1} onClick={() => aoMudar(pagina - 1)}>
            Anterior
          </button>
        </li>

        {numeros.map((n) => (
          <li key={n} className={`page-item ${n === pagina ? 'active' : ''}`}>
            <button
              type="button"
              className="page-link"
              aria-current={n === pagina ? 'page' : undefined}
              onClick={() => aoMudar(n)}
            >
              {n}
            </button>
          </li>
        ))}

        <li className={`page-item ${pagina >= totalPaginas ? 'disabled' : ''}`}>
          <button
            type="button"
            className="page-link"
            disabled={pagina >= totalPaginas}
            onClick={() => aoMudar(pagina + 1)}
          >
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}
