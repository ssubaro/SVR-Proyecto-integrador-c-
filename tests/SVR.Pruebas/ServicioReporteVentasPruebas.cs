using SVR.Aplicacion;
using SVR.Dominio;

namespace SVR.Pruebas;

public class ServicioReporteVentasPruebas
{
    [Fact]
    public void Generar_SugiereReponerHastaPuntoDeReordenYExcluyeBloqueados()
    {
        var catalogo = new Dictionary<string, Producto>
        {
            ["EAN-BAJO"] = new("EAN-BAJO", "Producto bajo", "Abarrotes", 10m, 8, 3, false),
            ["EAN-IGUAL"] = new("EAN-IGUAL", "Producto igual", "Abarrotes", 10m, 8, 8, false),
            ["EAN-BLOQUEADO"] = new("EAN-BLOQUEADO", "Producto bloqueado", "Abarrotes", 10m, 8, 2, true)
        };
        var servicio = new ServicioReporteVentas();

        ReporteVentas reporte = servicio.Generar(catalogo, []);

        PedidoSugerido pedido = Assert.Single(reporte.PedidoSugerido);
        Assert.Equal("EAN-BAJO", pedido.ProductoId);
        Assert.Equal(5, pedido.PiezasSugeridas);
    }

    [Fact]
    public void Generar_OrdenaTopPorPiezasYCalculaImportePorProducto()
    {
        var catalogo = new Dictionary<string, Producto>
        {
            ["EAN-A"] = new("EAN-A", "Producto A", "Abarrotes", 10m, 0, 20, false),
            ["EAN-B"] = new("EAN-B", "Producto B", "Abarrotes", 7.50m, 0, 20, false)
        };
        LineaVenta[] ventas =
        [
            new("F-1", 1, new TimeOnly(10, 0), "EAN-A", 2, "EFECTIVO"),
            new("F-2", 1, new TimeOnly(10, 5), "EAN-B", 4, "TARJETA"),
            new("F-3", 1, new TimeOnly(10, 10), "EAN-A", 1, "EFECTIVO")
        ];
        var servicio = new ServicioReporteVentas();

        ReporteVentas reporte = servicio.Generar(catalogo, ventas);

        Assert.Equal("EAN-B", reporte.TopVentas[0].ProductoId);
        Assert.Equal(4, reporte.TopVentas[0].PiezasVendidas);
        Assert.Equal(
            [new ImporteCorteCaja("EAN-A", 30m), new ImporteCorteCaja("EAN-B", 30m)],
            reporte.CorteCaja);
    }

    [Fact]
    public void Generar_ConVentaDeEanInexistente_LanzaProductoNoEncontrado()
    {
        var catalogo = new Dictionary<string, Producto>();
        LineaVenta[] ventas = [new("F-1", 1, new TimeOnly(10, 0), "EAN-X", 1, "EFECTIVO")];
        var servicio = new ServicioReporteVentas();

        Assert.Throws<ProductoNoEncontradoException>(() => servicio.Generar(catalogo, ventas));
    }
}
