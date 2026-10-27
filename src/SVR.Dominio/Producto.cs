using System.Globalization;

namespace SVR.Dominio;

/// <summary>
/// HU-01: un renglón válido del catálogo. Módulo 2.
/// </summary>
public sealed record Producto(
    string Ean,
    string Descripcion,
    string Categoria,
    decimal Precio,
    int PuntoReorden,
    int Stock,
    bool Bloqueado)
{
    /// <summary>
    /// Intenta crear un Producto a partir de los campos crudos de catalogo.csv.
    /// Rechaza EAN vacío o precio inválido; no lanza excepciones (validación en el borde).
    /// </summary>
    public static bool TryCrear(string[] campos, out Producto? producto, out string? motivoRechazo)
    {
        producto = null;

        if (campos.Length != 7)
        {
            motivoRechazo = "formato inválido";
            return false;
        }

        if (string.IsNullOrWhiteSpace(campos[0]))
        {
            motivoRechazo = "EAN vacío";
            return false;
        }

        if (!decimal.TryParse(campos[3], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precio))
        {
            motivoRechazo = $"precio inválido '{campos[3]}'";
            return false;
        }

        int.TryParse(campos[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out int puntoReorden);
        int.TryParse(campos[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out int stock);
        bool bloqueado = string.Equals(campos[6], "SI", StringComparison.OrdinalIgnoreCase);

        producto = new Producto(campos[0], campos[1], campos[2], precio, puntoReorden, stock, bloqueado);
        motivoRechazo = null;
        return true;
    }
}
