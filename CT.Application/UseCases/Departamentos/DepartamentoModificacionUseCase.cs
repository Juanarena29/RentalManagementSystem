using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Departamentos;

public class DepartamentoModificacionUseCase
{
    private readonly IRepositorioDepartamento _repositorio;
    private readonly DepartamentoValidator _validador;

    public DepartamentoModificacionUseCase(IRepositorioDepartamento repositorio, DepartamentoValidator validador)
    {
        _repositorio = repositorio;
        _validador = validador;
    }

    public async Task EjecutarAsync(ModificarDepartamentoDto dto)
    {
        // 1. Validaciones de formato
        _validador.Validar(dto);

        // 2. Obtener departamento existente
        var departamento = await _repositorio.ObtenerPorIdAsync(dto.DepartamentoId);
        if (departamento == null)
            throw new EntityNotFoundException("Departamento no encontrado.");

        // 3. Validar nombre no duplicado (excluyendo el actual)
        if (await _repositorio.ExisteNombreAsync(dto.Nombre.Trim(), dto.DepartamentoId))
            throw new ValidationException("Ya existe otro departamento con ese nombre.");

        // 4. Mapear solo los campos editables (preserva Estado original)
        departamento.Nombre = dto.Nombre.Trim();
        departamento.Descripcion = dto.Descripcion?.Trim();
        departamento.CapacidadMaxima = dto.CapacidadMaxima;
        departamento.PrecioPorNoche = dto.PrecioPorNoche;
        departamento.Observaciones = dto.Observaciones?.Trim();

        // 5. Persistir cambios
        await _repositorio.ModificarAsync(departamento);
    }
}
