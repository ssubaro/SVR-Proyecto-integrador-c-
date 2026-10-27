using SVR.Aplicacion;
using SVR.Infraestructura;

namespace SVR.Pruebas;

public class CorteCajaExportadorPruebas
{
    [Fact]
    public void Escribir_GeneraCsvConEncabezadoEImporteInvariantCulture()
    {
        string ruta = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");

        try
        {
            using (var exportador = new CorteCajaExportador(ruta))
            {
                exportador.Escribir([new ImporteCorteCaja("7501055300075", 1234.5m)]);
            }

            string[] lineas = File.ReadAllLines(ruta);
            Assert.Equal("ProductoId,Importe", lineas[0]);
            Assert.Equal("7501055300075,1234.50", lineas[1]);
        }
        finally
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
        }
    }

    [Fact]
    public void Escribir_DespuesDeDispose_LanzaObjectDisposedException()
    {
        string ruta = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");

        try
        {
            var exportador = new CorteCajaExportador(ruta);
            exportador.Dispose();

            Assert.Throws<ObjectDisposedException>(
                () => exportador.Escribir([new ImporteCorteCaja("EAN-A", 1m)]));
        }
        finally
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
        }
    }
}
