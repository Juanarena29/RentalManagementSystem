
using CT.Domain.Enums;

namespace CT.Domain.Entities;

public class Departamento
{
    public int DepartamentoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CapacidadMaxima { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public EstadoDepartamento Estado { get; set; }
    public string? Observaciones { get; set; }

    public Departamento() { }

}
