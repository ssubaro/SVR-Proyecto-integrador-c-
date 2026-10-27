using System.Globalization;

namespace SVR.Dominio;

/// <summary>
/// HU-02: un renglón válido de ventas_0457.csv. Módulo 7.
/// </summary>
public sealed record LineaVenta(
    string Folio,
    int Caja,
    TimeOnly Hora,
    string Ean,
    int Piezas,
    string MedioPago)
{
    /// <summary>
    /// Intenta crear una LineaVenta a partir de los campos crudos de ventas_0457.csv.
    /// Los errores de formato se rechazan con motivo (TryParse); el EAN inexistente en el
    /// catálogo es un error de negocio y se reporta lanzando ProductoNoEncontradoException.
    /// </summary>
    public static bool TryCrear(
        string[] campos,
        IReadOnlyDictionary<string, Producto> catalogo,
        out LineaVenta? venta,
        out string? motivoRechazo)
    {
        venta = null;

        if (campos.Length != 6)
        {
            motivoRechazo = "formato inválido";
            return false;
        }

        string folio = campos[0];
        string ean = campos[3];
        string medioPago = campos[5];

        if (!int.TryParse(campos[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int caja))
        {
            motivoRechazo = $"caja inválida '{campos[1]}'";
            return false;
        }

        if (!TimeOnly.TryParseExact(campos[2], "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly hora))
        {
            motivoRechazo = $"hora inválida '{campos[2]}'";
            return false;
        }

        if (!int.TryParse(campos[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out int piezas))
        {
            motivoRechazo = $"piezas inválidas '{campos[4]}'";
            return false;
        }

        if (!catalogo.ContainsKey(ean))
        {
            throw new ProductoNoEncontradoException(ean);
        }

        venta = new LineaVenta(folio, caja, hora, ean, piezas, medioPago);
        motivoRechazo = null;
        return true;
    }
}
