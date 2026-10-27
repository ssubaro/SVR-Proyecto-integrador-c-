using SVR.Dominio;

namespace SVR.Pruebas;

public class ValidadorVentaPruebas
{
    private static readonly Producto Cerveza =
        new("7501234500011", "Cerveza Clara 355ml", "Alcohol", 22.00m, 36, 96, Bloqueado: false);

    private static readonly Producto LecheBloqueada =
        new("7501998877665", "Leche Entera 1L", "Lacteos", 24.00m, 15, 40, Bloqueado: true);

    private static LineaVenta Venta(string ean, TimeOnly hora) =>
        new("T457-0001", 1, hora, ean, 1, "EFECTIVO");

    [Fact]
    public void Validar_ConAlcoholFueraDeHorario_LanzaVentaRestringida()
    {
        //Arrange
        LineaVenta venta = Venta(Cerveza.Ean, new TimeOnly(23, 30));

        //Act
        void Accion() => ValidadorVenta.Validar(venta, Cerveza);

        //Assert
        var ex = Assert.Throws<VentaRestringidaException>(Accion);
        Assert.Equal(Cerveza.Ean, ex.Ean);
        Assert.Contains("alcohol fuera de horario", ex.Motivo);
    }

    [Fact]
    public void Validar_ConAlcoholEnHorario_NoLanza()
    {
        //Arrange
        LineaVenta venta = Venta(Cerveza.Ean, new TimeOnly(12, 0));

        //Act
        var excepcion = Record.Exception(() => ValidadorVenta.Validar(venta, Cerveza));

        //Assert
        Assert.Null(excepcion);
    }

    [Fact]
    public void Validar_ConProductoBloqueado_LanzaVentaRestringida()
    {
        //Arrange
        LineaVenta venta = Venta(LecheBloqueada.Ean, new TimeOnly(10, 0));

        //Act
        void Accion() => ValidadorVenta.Validar(venta, LecheBloqueada);

        //Assert
        var ex = Assert.Throws<VentaRestringidaException>(Accion);
        Assert.Equal("producto bloqueado", ex.Motivo);
    }
}
