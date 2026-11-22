const formatadorMoeda = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

// 199.9 -> "R$ 199,90"
export function formatarPreco(valor) {
  return formatadorMoeda.format(valor ?? 0)
}
