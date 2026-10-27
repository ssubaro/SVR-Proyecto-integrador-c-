using SVR.Aplicacion;
using SVR.Dominio;

namespace SVR.Infraestructura;

/// <summary>
/// HU-06: inventario de la tienda en memoria, inicializado con el stock del catálogo.
/// Módulos 3 (Dictionary), 5 (StockInsuficienteException) y 8 (IInventarioRepo, DIP).
/// </summary>
public sealed class InventarioEnMemoria : IInventarioRepo
{
    private readonly Dictionary<string, int> _stock;

    public InventarioEnMemoria(IEnumerable<Producto> catalogo)
    {
        _stock = catalogo.ToDictionary(p => p.Ean, p => p.Stock);
    }

    public int Stock(string ean) => _stock.GetValueOrDefault(ean);

    public void Descontar(string ean, int piezas)
    {
        int disponible = Stock(ean);
        if (piezas > disponible)
        {
            throw new StockInsuficienteException(ean, disponible, piezas);
        }

        _stock[ean] = disponible - piezas;
    }
}
