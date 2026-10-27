using SVR.Dominio;

namespace SVR.Pruebas;

public class PagoPruebas
{
    // Comprueba que el efectivo suficiente se acepte y que se calcule correctamente el cambio.
    [Fact]
    public void Efectivo_ConImporteSuficiente_ValidaYCalculaCambio()
    {
        var pago = new Efectivo(500m);

        Assert.True(pago.Validar(420m));
        Assert.Equal(80m, pago.CalcularCambio(420m));
    }

    // Comprueba que un pago insuficiente se rechace y no permita calcular cambio.
    [Fact]
    public void Efectivo_ConImporteInsuficiente_NoValida()
    {
        var pago = new Efectivo(400m);

        Assert.False(pago.Validar(420m));
        Assert.Throws<InvalidOperationException>(() => pago.CalcularCambio(420m));
    }

    // Comprueba que la tarjeta requiera autorización y el importe exacto.
    [Fact]
    public void Tarjeta_RequiereAutorizacionEImporteExacto()
    {
        Assert.True(new Tarjeta(420m, true).Validar(420m));
        Assert.False(new Tarjeta(420m, false).Validar(420m));
        Assert.False(new Tarjeta(421m, true).Validar(420m));
    }

    // Comprueba que los vales cubran el total sin rebasar el límite por ticket.
    [Fact]
    public void Vales_RequierenCubrirTotalYSinExcederLimite()
    {
        Assert.True(new Vales(1_000m).Validar(1_000m));
        Assert.False(new Vales(999m).Validar(1_000m));
        Assert.False(new Vales(1_001m).Validar(1_001m));
    }

    // Comprueba que un total negativo se trate como dato inválido.
    [Fact]
    public void Validar_ConTotalNegativo_LanzaArgumentOutOfRangeException()
    {
        var pago = new Efectivo(100m);

        Assert.Throws<ArgumentOutOfRangeException>(() => pago.Validar(-1m));
    }
}
