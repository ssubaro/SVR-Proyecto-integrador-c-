using SVR.Dominio;

namespace SVR.Pruebas;

public class LineaVentaPruebas
{
    private static readonly IReadOnlyDictionary<string, Producto> Catalogo = new Dictionary<string, Producto>
    {
        ["7501055300075"] = new Producto("7501055300075", "Refresco Cola 600ml", "Bebidas", 18.50m, 24, 60),
    };

    [Fact]
    public void TryCrear_ConEanInexistente_LanzaProductoNoEncontrado()
    {
        //Arrange
        string[] campos = ["T457-0002", "2", "07:15", "7509999999999", "1", "TARJETA"];

        //Act
        void Accion() => LineaVenta.TryCrear(campos, Catalogo, out _, out _);

        //Assert
        var ex = Assert.Throws<ProductoNoEncontradoException>(Accion);
        Assert.Equal("7509999999999", ex.Ean);
    }

    [Fact]
    public void TryCrear_ConPiezasNoNumericas_Rechaza()
    {
        //Arrange
        string[] campos = ["T457-0003", "1", "08:40", "7501055300075", "abc", "VALES"];

        //Act
        bool creado = LineaVenta.TryCrear(campos, Catalogo, out LineaVenta? venta, out string? motivo);

        //Assert
        Assert.False(creado);
        Assert.Null(venta);
        Assert.Equal("piezas inválidas 'abc'", motivo);
    }

    [Fact]
    public void TryCrear_ConDatosValidos_CreaLineaVenta()
    {
        //Arrange
        string[] campos = ["T457-0001", "1", "07:12", "7501055300075", "2", "EFECTIVO"];

        //Act
        bool creado = LineaVenta.TryCrear(campos, Catalogo, out LineaVenta? venta, out string? motivo);

        //Assert
        Assert.True(creado);
        Assert.Null(motivo);
        Assert.Equal(2, venta!.Piezas);
        Assert.Equal(new TimeOnly(7, 12), venta.Hora);
    }
}
