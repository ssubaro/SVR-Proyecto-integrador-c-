using SVR.Dominio;
using SVR.Infraestructura;

namespace SVR.Pruebas;

public class InventarioEnMemoriaPruebas
{
    private static InventarioEnMemoria CrearInventario() => new(
    [
        new Producto("7501055300075", "Refresco Cola 600ml", "Bebidas", 18.50m, 24, 60),
    ]);

    [Fact]
    public void Descontar_ConStockSuficiente_ReduceElStock()
    {
        //Arrange
        InventarioEnMemoria inventario = CrearInventario();

        //Act
        inventario.Descontar("7501055300075", 10);

        //Assert
        Assert.Equal(50, inventario.Stock("7501055300075"));
    }

    [Fact]
    public void Descontar_ConStockInsuficiente_LanzaStockInsuficienteYNoModificaElStock()
    {
        //Arrange
        InventarioEnMemoria inventario = CrearInventario();

        //Act
        void Accion() => inventario.Descontar("7501055300075", 61);

        //Assert
        var ex = Assert.Throws<StockInsuficienteException>(Accion);
        Assert.Equal(60, ex.Disponible);
        Assert.Equal(61, ex.Solicitado);
        Assert.Equal(60, inventario.Stock("7501055300075"));
    }

    [Fact]
    public void Descontar_ConPiezasIgualesAlStock_DejaElStockEnCero()
    {
        //Arrange
        InventarioEnMemoria inventario = CrearInventario();

        //Act
        inventario.Descontar("7501055300075", 60);

        //Assert
        Assert.Equal(0, inventario.Stock("7501055300075"));
    }

    [Fact]
    public void Stock_ConEanDesconocido_RegresaCero()
    {
        //Arrange
        InventarioEnMemoria inventario = CrearInventario();

        //Act
        int stock = inventario.Stock("0000000000000");

        //Assert
        Assert.Equal(0, stock);
    }
}
