namespace SVR.Aplicacion;

/// <summary>Producto y cantidad necesaria para reponer hasta el punto de reorden.</summary>
public sealed record PedidoSugerido(
    string ProductoId,
    string Descripcion,
    int StockActual,
    int PuntoReorden,
    int PiezasSugeridas);

/// <summary>Producto con mayor cantidad de piezas vendidas.</summary>
public sealed record ProductoTopVentas(
    string ProductoId,
    string Descripcion,
    int PiezasVendidas);

/// <summary>Importe vendido por producto para el corte CSV.</summary>
public sealed record ImporteCorteCaja(string ProductoId, decimal Importe);

/// <summary>Resultados del pedido sugerido, top de ventas y corte del día.</summary>
public sealed record ReporteVentas(
    IReadOnlyList<PedidoSugerido> PedidoSugerido,
    IReadOnlyList<ProductoTopVentas> TopVentas,
    IReadOnlyList<ImporteCorteCaja> CorteCaja);
