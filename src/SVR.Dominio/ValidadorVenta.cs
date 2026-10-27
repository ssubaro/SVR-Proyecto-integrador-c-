namespace SVR.Dominio;

/// <summary>
/// HU-05: valida que una venta no infrinja las restricciones de la tienda
/// (alcohol fuera de horario, producto bloqueado). Módulo 7 (pattern matching, switch expression).
/// </summary>
public static class ValidadorVenta
{
    public static void Validar(LineaVenta venta, Producto producto)
    {
        string? motivo = producto switch
        {
            { Categoria: "Alcohol" } when !EnHorarioAlcohol(venta.Hora)
                => $"alcohol fuera de horario ({venta.Hora:HH\\:mm})",
            { Bloqueado: true }
                => "producto bloqueado",
            _ => null,
        };

        if (motivo is not null)
        {
            throw new VentaRestringidaException(venta.Ean, motivo);
        }
    }

    private static bool EnHorarioAlcohol(TimeOnly hora) =>
        hora >= Reglas.InicioVentaAlcohol && hora < Reglas.FinVentaAlcohol;
}
