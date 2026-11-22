import { useSearchParams } from 'react-router-dom'
import { generoApi, jogoApi, plataformaApi } from '../../api/servicos.js'
import CardJogo from '../../components/CardJogo.jsx'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx'
import Paginacao from '../../components/Paginacao.jsx'
import { useCarregar } from '../../lib/useCarregar.js'

const TAMANHO_PAGINA = 12

export default function Catalogo() {
  // Os filtros ficam na URL (?busca=mario&idPlataforma=2&pagina=2):
  // o link é compartilhável e o botão Voltar do navegador funciona.
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const idPlataforma = parametros.get('idPlataforma') ?? ''
  const idGenero = parametros.get('idGenero') ?? ''
  const pagina = Math.max(parseInt(parametros.get('pagina') ?? '1', 10) || 1, 1)

  // As listas de apoio carregam uma vez; os jogos recarregam sempre que um filtro da URL muda.
  const plataformas = useCarregar(() => plataformaApi.listar(), [])
  const generos = useCarregar(() => generoApi.listar(), [])
  const jogos = useCarregar(
    () => jogoApi.listar({ busca, idPlataforma, idGenero, pagina, tamanhoPagina: TAMANHO_PAGINA }),
    [busca, idPlataforma, idGenero, pagina],
  )

  // Trocar um filtro volta à página 1.
  function alterarFiltro(nome, valor) {
    const novos = new URLSearchParams(parametros)
    if (valor) novos.set(nome, valor)
    else novos.delete(nome)
    novos.delete('pagina')
    setParametros(novos)
  }

  function irParaPagina(numero) {
    const novos = new URLSearchParams(parametros)
    if (numero > 1) novos.set('pagina', String(numero))
    else novos.delete('pagina')
    setParametros(novos)
  }

  function aoBuscar(evento) {
    evento.preventDefault()
    const texto = String(new FormData(evento.currentTarget).get('busca') ?? '').trim()
    alterarFiltro('busca', texto)
  }

  function limparFiltros() {
    setParametros(new URLSearchParams())
  }

  const temFiltro = busca !== '' || idPlataforma !== '' || idGenero !== ''

  return (
    <>
      <h1 className="h3 mb-3">Catálogo</h1>

      <div className="row g-2 mb-4 align-items-end">
        {/* key={busca}: se a URL mudar (botão Voltar), o campo é recriado com o valor da URL. */}
        <form key={busca} className="col-12 col-md-5" onSubmit={aoBuscar} role="search">
          <label htmlFor="busca" className="form-label small mb-1">
            Buscar por nome
          </label>
          <div className="input-group">
            <input id="busca" name="busca" type="search" className="form-control" defaultValue={busca} placeholder="Ex.: zelda" />
            <button type="submit" className="btn btn-primary">
              Buscar
            </button>
          </div>
        </form>

        <div className="col-6 col-md-3">
          <label htmlFor="plataforma" className="form-label small mb-1">
            Plataforma
          </label>
          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            onChange={(e) => alterarFiltro('idPlataforma', e.target.value)}
          >
            <option value="">Todas</option>
            {(plataformas.dados ?? []).map((p) => (
              <option key={p.id} value={p.id}>
                {p.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-6 col-md-3">
          <label htmlFor="genero" className="form-label small mb-1">
            Gênero
          </label>
          <select
            id="genero"
            className="form-select"
            value={idGenero}
            onChange={(e) => alterarFiltro('idGenero', e.target.value)}
          >
            <option value="">Todos</option>
            {(generos.dados ?? []).map((g) => (
              <option key={g.id} value={g.id}>
                {g.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-md-1">
          {temFiltro && (
            <button type="button" className="btn btn-outline-secondary w-100" onClick={limparFiltros}>
              Limpar
            </button>
          )}
        </div>
      </div>

      {jogos.carregando && <Carregando mensagem="Carregando jogos..." />}

      {!jogos.carregando && jogos.erro && (
        <MensagemErro mensagem={jogos.erro} aoTentarNovamente={jogos.recarregar} />
      )}

      {!jogos.carregando && !jogos.erro && jogos.dados.itens.length === 0 && (
        <EstadoVazio mensagem="Nenhum jogo encontrado com esses filtros." />
      )}

      {!jogos.carregando && !jogos.erro && jogos.dados.itens.length > 0 && (
        <>
          <p className="text-muted small">
            {jogos.dados.totalItens} {jogos.dados.totalItens === 1 ? 'jogo encontrado' : 'jogos encontrados'}
          </p>
          <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4 g-3">
            {jogos.dados.itens.map((jogo) => (
              <div className="col" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>
          <Paginacao pagina={jogos.dados.pagina} totalPaginas={jogos.dados.totalPaginas} aoMudar={irParaPagina} />
        </>
      )}
    </>
  )
}
