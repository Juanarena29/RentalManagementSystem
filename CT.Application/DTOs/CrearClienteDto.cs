namespace CT.Application.DTOs;

/// <summary>
/// DTO de entrada para ClienteAltaUseCase.
/// 
/// ¿POR QUÉ EXISTE?
/// La entidad Cliente tiene ClienteId (lo genera la BD) y FechaCreacion (lo genera el sistema).
/// Si pasaras la entidad directa, desde Blazor podrías setear esos campos manualmente, 
/// lo cual es un error. Este DTO solo expone lo que el usuario completa en el formulario.
/// </summary>
public class CrearClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? Pais { get; set; }
    public string? Observaciones { get; set; }

}
