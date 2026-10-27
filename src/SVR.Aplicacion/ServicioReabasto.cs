using SVR.Dominio;

namespace SVR.Aplicacion;

/// <summary>
/// HU-07: consulta las existencias del CEDIS en paralelo con un timeout. Si una consulta
/// no responde a tiempo, cae al stock local y lo informa. Módulo 6.
/// </summary>
public sealed class ServicioReabasto
{
    private readonly ICedisServicio _cedis;
    private readonly TimeSpan _timeout;

    public ServicioReabasto(ICedisServicio cedis, TimeSpan? timeout = null)
    {
        _cedis = cedis;
        _timeout = timeout ?? Reglas.TimeoutConsultaCedis;
    }

    /// <summary>
    /// Consulta la existencia de cada EAN de stockLocal en el CEDIS, todas en paralelo.
    /// Si una consulta no responde dentro del timeout, se usa el stock local como respaldo.
    /// </summary>
    public async Task<IReadOnlyDictionary<string, (int Existencia, bool DeCedis)>> ConsultarExistenciasAsync(
        IReadOnlyDictionary<string, int> stockLocal)
    {
        using var cts = new CancellationTokenSource(_timeout);
        var consultas = stockLocal
            .Select(par => ConsultarUnoAsync(par.Key, par.Value, cts.Token))
            .ToList();

        var resultados = await Task.WhenAll(consultas);
        return resultados.ToDictionary(r => r.Ean, r => (r.Existencia, r.DeCedis));
    }

    private async Task<(string Ean, int Existencia, bool DeCedis)> ConsultarUnoAsync(
        string ean, int stockLocal, CancellationToken ct)
    {
        try
        {
            int existencia = await _cedis.ExistenciaAsync(ean, ct);
            return (ean, existencia, true);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return (ean, stockLocal, false);
        }
    }
}
