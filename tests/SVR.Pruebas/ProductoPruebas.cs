using SVR.Dominio;

namespace SVR.Pruebas;

public class ProductoPruebas
{
    [Fact]
    public void TryCrear_ConEanVacio_Rechaza()
    {
        //Arrange
        string[] campos = ["", "Detergente Liquido 1L", "Limpieza", "45.00", "5", "12", "NO"];

        //Act
        bool creado = Producto.TryCrear(campos, out Producto? producto, out string? motivo);

        //Assert
        Assert.False(creado);
        Assert.Null(producto);
        Assert.Equal("EAN vacío", motivo);
    }

    [Fact]
    public void TryCrear_ConPrecioInvalido_Rechaza()
    {
        //Arrange
        string[] campos = ["7502998001122", "Cafe Soluble 100g", "Abarrotes", "N/D", "20", "18", "NO"];

        //Act
        bool creado = Producto.TryCrear(campos, out Producto? producto, out string? motivo);

        //Assert
        Assert.False(creado);
        Assert.Null(producto);
        Assert.Equal("precio inválido 'N/D'", motivo);
    }

    [Fact]
    public void TryCrear_ConDatosValidos_CreaProducto()
    {
        //Arrange
        string[] campos = ["7501055300075", "Refresco Cola 600ml", "Bebidas", "18.50", "24", "60", "NO"];

        //Act
        bool creado = Producto.TryCrear(campos, out Producto? producto, out string? motivo);

        //Assert
        Assert.True(creado);
        Assert.Null(motivo);
        Assert.Equal(18.50m, producto!.Precio);
        Assert.Equal(24, producto.PuntoReorden);
        Assert.False(producto.Bloqueado);
    }

    [Fact]
    public void TryCrear_ConBloqueadoSI_MarcaProductoBloqueado()
    {
        //Arrange
        string[] campos = ["7501998877665", "Leche Entera 1L", "Lacteos", "24.00", "15", "40", "SI"];

        //Act
        bool creado = Producto.TryCrear(campos, out Producto? producto, out string? motivo);

        //Assert
        Assert.True(creado);
        Assert.Null(motivo);
        Assert.True(producto!.Bloqueado);
    }
}
