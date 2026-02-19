namespace CT.Application.DTOs;

/// <summary>
/// DTO de SALIDA para ConsultarDisponibilidadUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// Cuando el usuario consulta "¿qué departamentos están libres del 15 al 20 de marzo?",
/// la respuesta no es simplemente una lista de departamentos. Necesita datos CALCULADOS
/// que no existen en ninguna entidad:
///   - EstaDisponible: resultado del cruce Departamento × Reservas
///   - CantidadNoches: calculado con las fechas del usuario
///   - MontoTotal: noches × precio (no existe en ninguna tabla)
///   - MotivoNoDisponible: texto explicativo
/// 
/// Ninguna entidad tiene esta estructura. Es un resultado compuesto.
/// </summary>
public class DisponibilidadDto
{
    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public bool EstaDisponible { get; set; }
    public string? MotivoNoDisponible { get; set; }

    // Datos calculados según las fechas consultadas
    public int CantidadNoches { get; set; }
    public decimal MontoEstimado { get; set; }
}
