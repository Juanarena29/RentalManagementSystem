using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Departamentos;

/// <summary>
/// Cambia el estado de un departamento.
/// Valida que no se pueda poner en Mantenimiento/Inactivo si tiene reservas activas.
/// </summary>
public class DepartamentoCambiarEstadoUseCase
{
    private readonly IRepositorioDepartamento _repositorio;
    private readonly IRepositorioReserva _repositorioReserva;

    public DepartamentoCambiarEstadoUseCase(
        IRepositorioDepartamento repositorio,
        IRepositorioReserva repositorioReserva)
    {
        _repositorio = repositorio;
        _repositorioReserva = repositorioReserva;
    }

    public async Task EjecutarAsync(int departamentoId, EstadoDepartamento nuevoEstado)
    {
        var departamento = await _repositorio.ObtenerPorIdAsync(departamentoId);
        if (departamento == null)
            throw new EntityNotFoundException("Departamento no encontrado.");

        if (departamento.Estado == nuevoEstado)
            return; // Ya está en ese estado

        // Si se va a desactivar o poner en mantenimiento, verificar reservas activas
        if (nuevoEstado is EstadoDepartamento.Inactivo or EstadoDepartamento.Mantenimiento)
        {
            var reservas = await _repositorioReserva.ListarPorDepartamentoAsync(departamentoId);
            var tieneReservasActivas = reservas.Any(r =>
                r.Estado == EstadoReserva.Pendiente ||
                r.Estado == EstadoReserva.Confirmada);

            if (tieneReservasActivas)
                throw new DomainInvalidOperationException(
                    $"No se puede cambiar el estado a {nuevoEstado} porque el departamento tiene reservas activas.");
        }

        departamento.Estado = nuevoEstado;
        await _repositorio.ModificarAsync(departamento);
    }
}
