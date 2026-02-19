using CT.Application.DTOs;
using CT.Application.Validations;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Clientes;

public class ClienteModificarUseCase
{
    private readonly IRepositorioCliente _repositorio;
    private readonly ClienteValidator _validador;

    public ClienteModificarUseCase(IRepositorioCliente repositorio, ClienteValidator validador)
    {
        _repositorio = repositorio;
        _validador = validador;
    }

    public async Task EjecutarAsync(ModificarClienteDto dto)
    {
        // 1. Validaciones de formato
        _validador.Validar(dto);

        // 2. Obtener cliente existente
        var cliente = await _repositorio.ObtenerPorIdAsync(dto.ClienteId);
        if (cliente == null)
            throw new EntityNotFoundException("Cliente no encontrado.");

        // 3. Validar DNI no duplicado (excluyendo el cliente actual)
        if (await _repositorio.ExisteDniAsync(dto.DNI, dto.ClienteId))
            throw new ValidationException("Ya existe otro cliente con ese DNI.");

        // 4. Mapear solo los campos editables (preserva FechaCreacion original)
        cliente.Nombre = dto.Nombre.Trim();
        cliente.Apellido = dto.Apellido.Trim();
        cliente.DNI = dto.DNI.Trim();
        cliente.Telefono = dto.Telefono?.Trim();
        cliente.Email = dto.Email?.Trim().ToLowerInvariant();
        cliente.Ciudad = dto.Ciudad?.Trim();
        cliente.Provincia = dto.Provincia?.Trim();
        cliente.Pais = dto.Pais?.Trim();
        cliente.Observaciones = dto.Observaciones?.Trim();

        // 5. Persistir cambios
        await _repositorio.ModificarAsync(cliente);
    }
}
