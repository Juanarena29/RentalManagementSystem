using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Entities;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Clientes;

public class ClienteAltaUseCase
{
    private readonly IRepositorioCliente _repositorio;
    private readonly ClienteValidator _validador;

    public ClienteAltaUseCase(IRepositorioCliente repositorio, ClienteValidator validador)
    {
        _repositorio = repositorio;
        _validador = validador;
    }

    public async Task EjecutarAsync(CrearClienteDto dto)
    {
        // 1. Validaciones de formato
        _validador.Validar(dto);

        // 2. Validaciones de negocio
        if (await _repositorio.ExisteDniAsync(dto.DNI))
            throw new ValidationException("Ya existe un cliente con ese DNI.");

        // 3. Crear entidad
        var cliente = new Cliente
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            DNI = dto.DNI.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Email = dto.Email?.Trim().ToLowerInvariant(),
            Ciudad = dto.Ciudad?.Trim(),
            Provincia = dto.Provincia?.Trim(),
            Pais = dto.Pais?.Trim(),
            Observaciones = dto.Observaciones?.Trim(),
            FechaCreacion = DateTime.UtcNow
        };

        // 4. Persistir
        await _repositorio.AgregarAsync(cliente);
    }
}
