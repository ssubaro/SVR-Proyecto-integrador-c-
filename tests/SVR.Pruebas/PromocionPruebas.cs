using SVR.Dominio;

namespace SVR.Pruebas;

public class PromocionPruebas
{
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
        var promocion = new LlevaNPaga(lleva, paga);

        decimal descuento = promocion.Descuento(precio, piezas);

        Assert.Equal(descuentoEsperado, descuento);
    }

    [Fact]
    public void PorcentajeDesc_AplicaElPorcentajeAlImporteTotal()
    {
        var promocion = new PorcentajeDesc(15m);

        decimal importeFinal = promocion.ImporteFinal(100m, 2);

        Assert.Equal(170m, importeFinal);
    }

    [Fact]
    public void PrecioPorVolumen_AplicaPrecioPromocionalATodasLasPiezasAlAlcanzarMinimo()
    {
        var promocion = new PrecioPorVolumen(6, 18m);

        decimal importeFinal = promocion.ImporteFinal(18.50m, 6);

        Assert.Equal(108m, importeFinal);
    }

    [Fact]
    public void PrecioPorVolumen_NoGeneraDescuentoSiPrecioPromocionalEsMayor()
    {
        var promocion = new PrecioPorVolumen(2, 20m);

        decimal descuento = promocion.Descuento(18m, 2);

        Assert.Equal(0m, descuento);
    }

    [Fact]
    public void Descuento_ConPrecioNegativo_LanzaArgumentOutOfRangeException()
    {
        var promocion = new PorcentajeDesc(10m);

        Assert.Throws<ArgumentOutOfRangeException>(() => promocion.Descuento(-1m, 1));
    }
}
