using SVR.Aplicacion;

namespace SVR.Pruebas;

public class ServicioReabastoPruebas
{
    private sealed class CedisFalso(Dictionary<string, int> existencias, int demoraMs = 0) : ICedisServicio
    {
        public async Task<int> ExistenciaAsync(string ean, CancellationToken ct)
        {
            if (demoraMs > 0)
            {
                await Task.Delay(demoraMs, ct);
            }

            return existencias.GetValueOrDefault(ean);
        }
    }

    [Fact]
    public async Task ConsultarExistenciasAsync_ConCedisRespondiendo_UsaLaExistenciaDelCedis()
    {
        //Arrange
        var cedis = new CedisFalso(new Dictionary<string, int> { ["7501055300075"] = 1240 });
        var servicio = new ServicioReabasto(cedis, TimeSpan.FromSeconds(1));
        var stockLocal = new Dictionary<string, int> { ["7501055300075"] = 60 };

        //Act
        var resultado = await servicio.ConsultarExistenciasAsync(stockLocal);

        //Assert
        Assert.True(resultado["7501055300075"].DeCedis);
        Assert.Equal(1240, resultado["7501055300075"].Existencia);
    }

    [Fact]
    public async Task ConsultarExistenciasAsync_ConCedisLento_UsaStockLocalTrasElTimeout()
    {
        //Arrange
        var cedis = new CedisFalso(new Dictionary<string, int> { ["7501030411111"] = 55 }, demoraMs: 500);
        var servicio = new ServicioReabasto(cedis, TimeSpan.FromMilliseconds(50));
        var stockLocal = new Dictionary<string, int> { ["7501030411111"] = 10 };

        //Act
        var resultado = await servicio.ConsultarExistenciasAsync(stockLocal);

        //Assert
        Assert.False(resultado["7501030411111"].DeCedis);
        Assert.Equal(10, resultado["7501030411111"].Existencia);
    }

    [Fact]
    public async Task ConsultarExistenciasAsync_ConVariosProductos_LosConsultaATodosEnParalelo()
    {
        //Arrange
        var cedis = new CedisFalso(new Dictionary<string, int>
        {
            ["A"] = 10,
            ["B"] = 20,
        });
        var servicio = new ServicioReabasto(cedis, TimeSpan.FromSeconds(1));
        var stockLocal = new Dictionary<string, int> { ["A"] = 1, ["B"] = 2 };

        //Act
        var resultado = await servicio.ConsultarExistenciasAsync(stockLocal);

        //Assert
        Assert.Equal(2, resultado.Count);
        Assert.Equal(10, resultado["A"].Existencia);
        Assert.Equal(20, resultado["B"].Existencia);
    }
}
