namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para DepartamentoAltaUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// El usuario solo ingresa nombre, descripción, capacidad y precio.
/// El sistema asigna DepartamentoId (BD) y Estado (arranca en Activo por defecto).
/// </summary>
public class CrearDepartamentoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public string? Observaciones { get; set; }

    // NO incluye: DepartamentoId (BD), Estado (default Activo)
}
