using SVR.Dominio;

namespace SVR.Pruebas;

public class PagoPruebas
{
    [Fact]
    public void Efectivo_ConImporteSuficiente_ValidaYCalculaCambio()
    {
        var pago = new Efectivo(500m);

        Assert.True(pago.Validar(420m));
        Assert.Equal(80m, pago.CalcularCambio(420m));
    }

    [Fact]
    public void Efectivo_ConImporteInsuficiente_NoValida()
    {
        var pago = new Efectivo(400m);

        Assert.False(pago.Validar(420m));
        Assert.Throws<InvalidOperationException>(() => pago.CalcularCambio(420m));
    }

    [Fact]
    public void Tarjeta_RequiereAutorizacionEImporteExacto()
    {
        Assert.True(new Tarjeta(420m, true).Validar(420m));
        Assert.False(new Tarjeta(420m, false).Validar(420m));
        Assert.False(new Tarjeta(421m, true).Validar(420m));
    }

    [Fact]
    public void Vales_RequierenCubrirTotalYSinExcederLimite()
    {
        Assert.True(new Vales(1_000m).Validar(1_000m));
        Assert.False(new Vales(999m).Validar(1_000m));
        Assert.False(new Vales(1_001m).Validar(1_001m));
    }

    [Fact]
    public void Validar_ConTotalNegativo_LanzaArgumentOutOfRangeException()
    {
        var pago = new Efectivo(100m);

        Assert.Throws<ArgumentOutOfRangeException>(() => pago.Validar(-1m));
    }
}
