namespace SVR.Dominio;

/// <summary>
/// HU-01 a HU-06: base de las excepciones de negocio de la tienda. Módulo 5.
/// </summary>
public abstract class TiendaException : Exception
{
    protected TiendaException(string mensaje) : base(mensaje)
    {
    }
}

/// <summary>
/// HU-02: el EAN de una línea de venta no existe en el catálogo.
/// </summary>
public sealed class ProductoNoEncontradoException : TiendaException
{
    public string Ean { get; }

    public ProductoNoEncontradoException(string ean)
        : base($"producto no encontrado: EAN {ean}")
    {
        Ean = ean;
    }
}
