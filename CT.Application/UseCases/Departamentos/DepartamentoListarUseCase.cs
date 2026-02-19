using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Departamentos;

public class DepartamentoListarUseCase
{
    private readonly IRepositorioDepartamento _repositorio;

    public DepartamentoListarUseCase(IRepositorioDepartamento repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<DepartamentoDto>> EjecutarAsync(bool soloActivos = false)
    {
        var departamentos = soloActivos
            ? await _repositorio.ListarDisponiblesAsync()
            : await _repositorio.ListarAsync();

        return departamentos.MapToDto();
    }
}
