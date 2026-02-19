using CT.Application.DTOs;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Clientes;

public class ClienteObtenerDetalleUseCase
{
    private readonly IRepositorioCliente _repositorio;

    public ClienteObtenerDetalleUseCase(IRepositorioCliente repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ClienteDto> EjecutarAsync(int clienteId)
    {
        var cliente = await _repositorio.ObtenerPorIdAsync(clienteId);

        if (cliente == null)
            throw new EntityNotFoundException("Cliente no encontrado.");

        return cliente.MapToDto();
    }
}
