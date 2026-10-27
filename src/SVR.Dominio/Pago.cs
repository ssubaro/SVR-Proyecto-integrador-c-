namespace SVR.Dominio;

/// <summary>
/// HU-04: valida pagos mediante sus tipos concretos.
/// Módulo 7: record abstracto y pattern matching con property patterns.
/// </summary>
public abstract record Pago
{
    /// <summary>Selecciona la validación correspondiente al medio de pago concreto.</summary>
    public bool Validar(decimal total)
    {
        if (total < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(total), "El total no puede ser negativo.");
        }

        return this switch
        {
            // El efectivo puede superar el total; la diferencia se entrega como cambio.
            Efectivo { ImporteRecibido: > 0 } efectivo => efectivo.ImporteRecibido >= total,
            // Tarjetas deben estar autorizadas y cubrir exactamente el importe.
            Tarjeta { Importe: > 0, Autorizada: true } tarjeta => tarjeta.Importe == total,
            // Los vales deben cubrir el total completo y respetar el máximo permitido.
            Vales { Importe: > 0 and <= Reglas.MaximoEnVales } vales => vales.Importe == total,
            _ => false
        };
    }
}

/// <summary>Pago en efectivo; permite calcular el cambio si alcanza para cubrir el total.</summary>
public sealed record Efectivo(decimal ImporteRecibido) : Pago
{
    /// <summary>Devuelve la diferencia entre el efectivo recibido y el total validado.</summary>
    public decimal CalcularCambio(decimal total)
    {
        if (!Validar(total))
        {
            throw new InvalidOperationException("El efectivo recibido no cubre el total.");
        }

        return ImporteRecibido - total;
    }
}

/// <summary>Pago con tarjeta; requiere autorización y el importe exacto del total.</summary>
public sealed record Tarjeta(decimal Importe, bool Autorizada) : Pago;

/// <summary>Pago con vales; debe cubrir el total sin exceder el límite por ticket.</summary>
public sealed record Vales(decimal Importe) : Pago;
