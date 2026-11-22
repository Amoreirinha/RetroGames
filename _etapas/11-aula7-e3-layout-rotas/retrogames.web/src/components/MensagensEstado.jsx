// Toda tela que busca dados trata três estados: carregando, erro e vazio.

export function Carregando({ mensagem = 'Carregando...' }) {
  return (
    <div className="text-center py-5" role="status">
      <div className="spinner-border text-secondary" aria-hidden="true"></div>
      <p className="mt-2 mb-0 text-muted">{mensagem}</p>
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger d-flex flex-wrap align-items-center justify-content-between gap-2" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-outline-danger btn-sm" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({ mensagem = 'Nada por aqui.' }) {
  return (
    <div className="text-center text-muted py-5">
      <p className="mb-0">{mensagem}</p>
    </div>
  )
}
