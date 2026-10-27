using System.Globalization;
using SVR.Dominio;
using SVR.Infraestructura;

// Sistema de Venta y Reabastecimiento (SVR) · cierre del día de la tienda 0457.
// Proyecto base: la lectura de los CSV y el CEDIS simulado ya funcionan.
// Tu trabajo: completar HU-01 a HU-08 (ver README.md) y reemplazar cada "Pendiente(...)".

CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-MX");
Console.OutputEncoding = System.Text.Encoding.UTF8;

string datos = Path.Combine(AppContext.BaseDirectory, "datos");
var filasCatalogo = LectorCsv.Leer(Path.Combine(datos, "catalogo.csv")).ToList();
var filasPromociones = LectorCsv.Leer(Path.Combine(datos, "promociones.csv")).ToList();
var filasVentas = LectorCsv.Leer(Path.Combine(datos, "ventas_0457.csv")).ToList();
var cedis = new CedisSimulado();   // lo usarás en HU-07

Console.WriteLine("=== SVR · TIENDA 0457 ===");
Console.WriteLine($"Filas leídas: catálogo {filasCatalogo.Count} · promociones {filasPromociones.Count} · ventas {filasVentas.Count}");
Console.WriteLine($"Ejemplo: línea {filasVentas[0].Linea} -> {string.Join(" | ", filasVentas[0].Campos)}");
Console.WriteLine();

// Avance 1 (9:35–10:15)

// HU-01: cargar el catálogo y rechazar EAN vacío o precio inválido.
var catalogo = new Dictionary<string, Producto>();
int catalogoRechazados = 0;
foreach (FilaCsv fila in filasCatalogo)
{
    if (Producto.TryCrear(fila.Campos, out Producto? producto, out string? motivoRechazo))
    {
        catalogo[producto!.Ean] = producto;
    }
    else
    {
        Console.WriteLine($"[catálogo] Línea {fila.Linea} rechazada: {motivoRechazo}");
        catalogoRechazados++;
    }
}

Console.WriteLine($"HU-01 catálogo -> válidos: {catalogo.Count} | rechazados: {catalogoRechazados}");
Console.WriteLine();

// HU-02: procesar ventas_0457.csv y escribir rechazos.log (línea y motivo).
var ventasValidas = new List<LineaVenta>();
var rechazosVentas = new List<string>();
foreach (FilaCsv fila in filasVentas)
{
    try
    {
        if (LineaVenta.TryCrear(fila.Campos, catalogo, out LineaVenta? venta, out string? motivoRechazo))
        {
            ventasValidas.Add(venta!);
        }
        else
        {
            rechazosVentas.Add($"Línea {fila.Linea}: {motivoRechazo}");
        }
    }
    catch (TiendaException ex)
    {
        rechazosVentas.Add($"Línea {fila.Linea}: {ex.Message}");
    }
}

File.WriteAllLines(Path.Combine(datos, "rechazos.log"), rechazosVentas);
Console.WriteLine($"HU-02 ventas -> válidas: {ventasValidas.Count} | rechazadas: {rechazosVentas.Count} (ver rechazos.log)");
Console.WriteLine();

// Avance 2 (10:15–11:15) · TODO: HU-03 promociones, HU-04 medios de pago,
//   HU-05 reglas de venta, HU-06 inventario.

// Avance 3 (11:15–12:15) · TODO: HU-07 CEDIS en paralelo con timeout,
//   HU-08 pedido sugerido, top de ventas y corte de caja.
