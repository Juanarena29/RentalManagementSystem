using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Enums;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Departamentos;

public class DepartamentoAltaUseCase
{
    private readonly IRepositorioDepartamento _repositorio;
    private readonly DepartamentoValidator _validador;

    public DepartamentoAltaUseCase(IRepositorioDepartamento repositorio, DepartamentoValidator validador)
    {
        _repositorio = repositorio;
        _validador = validador;
    }

    public async Task EjecutarAsync(CrearDepartamentoDto dto)
    {
        // 1. Validaciones de formato
        _validador.Validar(dto);

        // 2. Validar nombre no duplicado
        if (await _repositorio.ExisteNombreAsync(dto.Nombre.Trim()))
            throw new ValidationException("Ya existe un departamento con ese nombre.");

        // 3. Crear entidad
        var departamento = new Departamento
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            CapacidadMaxima = dto.CapacidadMaxima,
            PrecioPorNoche = dto.PrecioPorNoche,
            Estado = EstadoDepartamento.Activo,
            Observaciones = dto.Observaciones?.Trim()
        };

        // 4. Persistir
        await _repositorio.AgregarAsync(departamento);
    }
}
