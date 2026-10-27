namespace SVR.Dominio;

// HU-01 y HU-02: Producto y LineaVenta -> ver Producto.cs y LineaVenta.cs.
// HU-01 y HU-02: TiendaException y ProductoNoEncontradoException -> ver Excepciones.cs.
// HU-05 y HU-06 · TODO: StockInsuficienteException, VentaRestringidaException
//   (mismas base TiendaException, con propiedades de contexto). Módulo 5.

// HU-03 · TODO: Promocion (abstract) con Descuento(precio, piezas)
//   -> LlevaNPagaM (NXM 2;1), PorcentajeDesc (PORCENTAJE 15), PrecioPorVolumen (VOLUMEN 6;18.00).
//   Una promoción nueva = una clase nueva; la caja no cambia. Módulo 8.

// HU-04 · TODO: Pago (abstract record) -> Efectivo, Tarjeta, Vales
//   Valida cada pago con switch expression y property patterns. Módulo 7.
