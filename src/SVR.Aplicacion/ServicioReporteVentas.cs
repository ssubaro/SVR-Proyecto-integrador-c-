using SVR.Dominio;

namespace SVR.Aplicacion;

/// <summary>
/// HU-08: genera el pedido sugerido, el top 5 de ventas y los importes del corte.
/// Módulo 4: aplica agrupación, proyección, ordenamiento, suma y límite con LINQ.
/// </summary>
public sealed class ServicioReporteVentas
{
    public ReporteVentas Generar(
        IReadOnlyDictionary<string, Producto> catalogo,
        IEnumerable<LineaVenta> ventasValidas)
    {
        ArgumentNullException.ThrowIfNull(catalogo);
        ArgumentNullException.ThrowIfNull(ventasValidas);

        // Sugiere reponer productos no bloqueados hasta su punto de reorden.
        PedidoSugerido[] pedidoSugerido = catalogo.Values
            .Where(producto => !producto.Bloqueado && producto.Stock < producto.PuntoReorden)
            .Select(producto => new PedidoSugerido(
                producto.Ean,
                producto.Descripcion,
                producto.Stock,
                producto.PuntoReorden,
                producto.PuntoReorden - producto.Stock))
            .OrderBy(producto => producto.ProductoId, StringComparer.Ordinal)
            .ToArray();

        // Agrupa las ventas aceptadas y calcula piezas e importe por producto.
        var resumenVentas = ventasValidas
            .GroupBy(venta => venta.Ean)
            .Select(grupo =>
            {
                if (!catalogo.TryGetValue(grupo.Key, out Producto? producto))
                {
                    throw new ProductoNoEncontradoException(grupo.Key);
                }

                int piezasVendidas = grupo.Sum(venta => venta.Piezas);
                if (piezasVendidas <= 0 || grupo.Any(venta => venta.Piezas <= 0))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(ventasValidas),
                        $"La venta del producto {grupo.Key} debe tener piezas positivas.");
                }

                return new
                {
                    Producto = producto,
                    PiezasVendidas = piezasVendidas,
                    Importe = producto.Precio * piezasVendidas
                };
            })
            .ToArray();

        // Ordena por unidades vendidas y usa el EAN para mantener resultados deterministas.
        ProductoTopVentas[] topVentas = resumenVentas
            .OrderByDescending(resumen => resumen.PiezasVendidas)
            .ThenBy(resumen => resumen.Producto.Ean, StringComparer.Ordinal)
            .Take(5)
            .Select(resumen => new ProductoTopVentas(
                resumen.Producto.Ean,
                resumen.Producto.Descripcion,
                resumen.PiezasVendidas))
            .ToArray();

        // Prepara el importe estimado por producto usando el precio de catálogo sin promociones.
        ImporteCorteCaja[] corteCaja = resumenVentas
            .OrderBy(resumen => resumen.Producto.Ean, StringComparer.Ordinal)
            .Select(resumen => new ImporteCorteCaja(resumen.Producto.Ean, resumen.Importe))
            .ToArray();

        return new ReporteVentas(pedidoSugerido, topVentas, corteCaja);
    }
}
