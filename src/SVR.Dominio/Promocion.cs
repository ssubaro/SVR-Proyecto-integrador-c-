namespace SVR.Dominio;

/// <summary>
/// HU-03: calcula el descuento de una promoción sin modificar la lógica del ticket al agregar nuevos tipos.
/// Módulo 8: principio abierto/cerrado.
/// </summary>
public abstract class Promocion
{
    /// <summary>Valida los datos comunes y delega el cálculo al tipo de promoción concreto.</summary>
    public decimal Descuento(decimal precio, int piezas)
    {
        if (precio < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precio), "El precio no puede ser negativo.");
        }

        if (piezas < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(piezas), "Las piezas no pueden ser negativas.");
        }

        return CalcularDescuento(precio, piezas);
    }

    /// <summary>Resta el descuento al precio unitario multiplicado por las piezas.</summary>
    public decimal ImporteFinal(decimal precio, int piezas) =>
        precio * piezas - Descuento(precio, piezas);

    // Cada promoción define su propia fórmula sin cambiar la clase base.
    protected abstract decimal CalcularDescuento(decimal precio, int piezas);
}

/// <summary>Aplica el descuento de las piezas gratis por cada grupo completo N×M.</summary>
public sealed class LlevaNPaga : Promocion
{
    public int Lleva { get; }
    public int Paga { get; }

    /// <summary>Configura un grupo promocional y comprueba que las cantidades sean coherentes.</summary>
    public LlevaNPaga(int lleva, int paga)
    {
        if (lleva <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lleva), "La cantidad que se lleva debe ser positiva.");
        }

        if (paga < 0 || paga >= lleva)
        {
            throw new ArgumentOutOfRangeException(nameof(paga), "La cantidad que se paga debe ser menor a la que se lleva.");
        }

        Lleva = lleva;
        Paga = paga;
    }

    // Solo los grupos completos generan unidades gratis.
    protected override decimal CalcularDescuento(decimal precio, int piezas) =>
        piezas / Lleva * (Lleva - Paga) * precio;
}

/// <summary>Aplica un porcentaje de descuento al importe de todas las piezas.</summary>
public sealed class PorcentajeDesc : Promocion
{
    public decimal Porcentaje { get; }

    /// <summary>Configura el descuento porcentual dentro del rango de 0 a 100.</summary>
    public PorcentajeDesc(decimal porcentaje)
    {
        if (porcentaje < 0 || porcentaje > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(porcentaje), "El porcentaje debe estar entre 0 y 100.");
        }

        Porcentaje = porcentaje;
    }

    // El porcentaje se calcula sobre el importe total antes de la promoción.
    protected override decimal CalcularDescuento(decimal precio, int piezas) =>
        precio * piezas * Porcentaje / 100m;
}

/// <summary>Aplica un precio unitario promocional al alcanzar la cantidad mínima.</summary>
public sealed class PrecioPorVolumen : Promocion
{
    public int PiezasMinimas { get; }
    public decimal PrecioPromocional { get; }

    /// <summary>Configura el mínimo de piezas y el precio unitario de volumen.</summary>
    public PrecioPorVolumen(int piezasMinimas, decimal precioPromocional)
    {
        if (piezasMinimas <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(piezasMinimas), "La cantidad mínima debe ser positiva.");
        }

        if (precioPromocional < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precioPromocional), "El precio promocional no puede ser negativo.");
        }

        PiezasMinimas = piezasMinimas;
        PrecioPromocional = precioPromocional;
    }

    // Si se alcanza el mínimo, descuenta la diferencia de precio en todas las piezas.
    protected override decimal CalcularDescuento(decimal precio, int piezas) =>
        piezas >= PiezasMinimas
            ? Math.Max(0m, precio - PrecioPromocional) * piezas
            : 0m;
}
