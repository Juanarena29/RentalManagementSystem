namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para ObtenerEstadisticasOcupacionUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// El dashboard de estadísticas necesita mostrar indicadores calculados que NO existen
/// en ninguna entidad:
///   - Tasa de ocupación: noches ocupadas / noches disponibles × 100
///   - Totales y promedios calculados cruzando múltiples entidades
///   - Distribución de reservas por estado
/// 
/// Es un resultado compuesto de cálculos sobre Reservas, Pagos y Departamentos.
/// </summary>
public class EstadisticasOcupacionDto
{
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }

    // Ocupación
    public decimal TasaOcupacion { get; set; }
    public int NochesOcupadas { get; set; }
    public int NochesTotalesPosibles { get; set; }

    // Reservas
    public int TotalReservas { get; set; }
    public int ReservasPendientes { get; set; }
    public int ReservasConfirmadas { get; set; }
    public int ReservasFinalizadas { get; set; }
    public int ReservasCanceladas { get; set; }

    // Ingresos
    public decimal IngresosTotales { get; set; }
    public decimal IngresoPromedioPorReserva { get; set; }
}
