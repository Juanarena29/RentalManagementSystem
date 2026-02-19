using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Departamentos;

public class DepartamentoBajaUseCase
{
    private readonly IRepositorioDepartamento _repositorio;
    private readonly IRepositorioReserva _repositorioReserva;

    public DepartamentoBajaUseCase(IRepositorioDepartamento repositorio, IRepositorioReserva repositorioReserva)
    {
        _repositorio = repositorio;
        _repositorioReserva = repositorioReserva;
    }

    public async Task EjecutarAsync(int departamentoId)
    {
        // 1. Validar que existe
        var departamento = await _repositorio.ObtenerPorIdAsync(departamentoId);
        if (departamento == null)
            throw new EntityNotFoundException("Departamento no encontrado.");

        // 2. Validar que no tiene reservas activas (Pendiente, Confirmada o CheckIn)
        var reservas = await _repositorioReserva.ListarPorDepartamentoAsync(departamentoId);
        var tieneReservasActivas = reservas.Any(r =>
            r.Estado == EstadoReserva.Pendiente ||
            r.Estado == EstadoReserva.Confirmada);

        if (tieneReservasActivas)
            throw new DomainInvalidOperationException(
                "No se puede dar de baja el departamento porque tiene reservas activas.");

        // 3. Baja lógica: cambiar estado a Inactivo
        departamento.Estado = EstadoDepartamento.Inactivo;
        await _repositorio.ModificarAsync(departamento);
    }
}
