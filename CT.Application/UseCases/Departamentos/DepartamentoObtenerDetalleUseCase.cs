using CT.Application.DTOs;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Departamentos;

public class DepartamentoObtenerDetalleUseCase
{
    private readonly IRepositorioDepartamento _repositorio;

    public DepartamentoObtenerDetalleUseCase(IRepositorioDepartamento repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<DepartamentoDto> EjecutarAsync(int departamentoId)
    {
        var departamento = await _repositorio.ObtenerPorIdAsync(departamentoId);

        if (departamento == null)
            throw new EntityNotFoundException("Departamento no encontrado.");

        return departamento.MapToDto();
    }
}
