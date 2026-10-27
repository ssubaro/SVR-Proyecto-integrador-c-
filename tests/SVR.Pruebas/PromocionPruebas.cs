using SVR.Dominio;

namespace SVR.Pruebas;

public class PromocionPruebas
{
    // Comprueba que N×M descuente las piezas gratis solo en grupos completos.
    [Theory]
    [InlineData(2, 1, 2, 18.50, 18.50)]
    [InlineData(2, 1, 3, 18.50, 18.50)]
    [InlineData(3, 2, 6, 18.50, 37.00)]
    public void LlevaNPaga_DescuentaSoloGruposCompletos(
        int lleva,
        int paga,
        int piezas,
        decimal precio,
        decimal descuentoEsperado)
    {
        // Arrange: configura los parámetros del caso de promoción.
        var promocion = new LlevaNPaga(lleva, paga);

        // Act: calcula el descuento para el precio y cantidad indicados.
        decimal descuento = promocion.Descuento(precio, piezas);

        // Assert: compara el resultado con el descuento esperado.
        Assert.Equal(descuentoEsperado, descuento);
    }

    // Comprueba que el porcentaje se aplique al importe acumulado de todas las piezas.
    [Fact]
    public void PorcentajeDesc_AplicaElPorcentajeAlImporteTotal()
    {
        var promocion = new PorcentajeDesc(15m);

        decimal importeFinal = promocion.ImporteFinal(100m, 2);

        Assert.Equal(170m, importeFinal);
    }

    // Comprueba que, alcanzado el mínimo, todas las piezas usen el precio promocional.
    [Fact]
    public void PrecioPorVolumen_AplicaPrecioPromocionalATodasLasPiezasAlAlcanzarMinimo()
    {
        var promocion = new PrecioPorVolumen(6, 18m);

        decimal importeFinal = promocion.ImporteFinal(18.50m, 6);

        Assert.Equal(108m, importeFinal);
    }

    // Evita que una promoción con precio superior al regular aumente el importe.
    [Fact]
    public void PrecioPorVolumen_NoGeneraDescuentoSiPrecioPromocionalEsMayor()
    {
        var promocion = new PrecioPorVolumen(2, 20m);

        decimal descuento = promocion.Descuento(18m, 2);

        Assert.Equal(0m, descuento);
    }

    // Verifica que la entrada inválida se reporte como error de captura.
    [Fact]
    public void Descuento_ConPrecioNegativo_LanzaArgumentOutOfRangeException()
    {
        var promocion = new PorcentajeDesc(10m);

        Assert.Throws<ArgumentOutOfRangeException>(() => promocion.Descuento(-1m, 1));
    }
}
