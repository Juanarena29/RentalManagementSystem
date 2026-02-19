using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Reservas;

public class ConsultarDisponibilidadUseCase
{
    private readonly IRepositorioDepartamento _repositorioDepartamento;
    private readonly ReservaValidator _validador;

    public ConsultarDisponibilidadUseCase(
        IRepositorioDepartamento repositorioDepartamento,
        ReservaValidator validador)
    {
        _repositorioDepartamento = repositorioDepartamento;
        _validador = validador;
    }

    public async Task<List<DisponibilidadDto>> EjecutarAsync(DateTime fechaInicio, DateTime fechaFin, int? cantidadHuespedes = null)
    {
        // 1. Validar fechas
        _validador.ValidarRangoFechas(fechaInicio, fechaFin);

        // 2. Obtener todos los departamentos activos
        var todosDeptos = await _repositorioDepartamento.ListarDisponiblesAsync();

        // 3. Obtener los disponibles en el rango en 1 sola query (sin N+1)
        var deptosLibres = await _repositorioDepartamento.ObtenerDepartamentosDisponiblesEnRangoAsync(fechaInicio, fechaFin);
        var idsLibres = new HashSet<int>(deptosLibres.Select(d => d.DepartamentoId));

        // 4. Armar resultado con todos los deptos, indicando disponibilidad
        int cantidadNoches = (fechaFin.Date - fechaInicio.Date).Days;
        var resultado = new List<DisponibilidadDto>();

        foreach (var depto in todosDeptos)
        {
            bool disponible = idsLibres.Contains(depto.DepartamentoId);
            string? motivo = null;

            if (!disponible)
            {
                motivo = "Ocupado en ese período";
            }
            else if (cantidadHuespedes.HasValue && cantidadHuespedes.Value > depto.CapacidadMaxima)
            {
                disponible = false;
                motivo = $"Capacidad máxima: {depto.CapacidadMaxima} huéspedes";
            }

            resultado.Add(new DisponibilidadDto
            {
                DepartamentoId = depto.DepartamentoId,
                DepartamentoNombre = depto.Nombre,
                CapacidadMaxima = depto.CapacidadMaxima,
                PrecioPorNoche = depto.PrecioPorNoche,
                EstaDisponible = disponible,
                MotivoNoDisponible = motivo,
                CantidadNoches = cantidadNoches,
                MontoEstimado = cantidadNoches * depto.PrecioPorNoche
            });
        }

        return resultado;
    }
}
