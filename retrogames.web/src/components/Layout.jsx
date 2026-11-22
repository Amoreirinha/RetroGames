import { NavLink, Outlet } from 'react-router-dom'

export default function Layout() {
  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <NavLink className="navbar-brand fw-bold" to="/">
            RetroGames
          </NavLink>
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menu-principal"
            aria-controls="menu-principal"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="menu-principal">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <NavLink className="nav-link" to="/" end>
                  Catálogo
                </NavLink>
              </li>
            </ul>
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        <Outlet />
      </main>

      <footer className="bg-light border-top py-3 text-center text-muted small">
        RetroGames &middot; Gestão do Ciclo de Vida da Aplicação
      </footer>
    </div>
  )
}
