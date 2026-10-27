using System.Globalization;
using System.Text;
using SVR.Aplicacion;

namespace SVR.Infraestructura;

/// <summary>
/// HU-08: exporta el importe vendido por producto a CSV.
/// Módulos 7 y 8: StreamWriter con IDisposable y persistencia CSV.
/// </summary>
public sealed class CorteCajaExportador : IDisposable
{
    private readonly StreamWriter _escritor;
    private bool _disposed;

    public CorteCajaExportador(string ruta)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruta);

        string? directorio = Path.GetDirectoryName(Path.GetFullPath(ruta));
        if (directorio is not null)
        {
            Directory.CreateDirectory(directorio);
        }

        _escritor = new StreamWriter(ruta, false, new UTF8Encoding(false));
        _escritor.WriteLine("ProductoId,Importe");
    }

    /// <summary>Escribe el identificador de producto y el importe con formato decimal invariable.</summary>
    public void Escribir(IEnumerable<ImporteCorteCaja> importes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(importes);

        foreach (ImporteCorteCaja importe in importes)
        {
            _escritor.Write(importe.ProductoId);
            _escritor.Write(',');
            _escritor.WriteLine(importe.Importe.ToString("0.00", CultureInfo.InvariantCulture));
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _escritor.Dispose();
        _disposed = true;
    }
}
