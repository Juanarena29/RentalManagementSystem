using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Clientes;

public class ClienteListarUseCase
{
    private readonly IRepositorioCliente _repositorio;

    public ClienteListarUseCase(IRepositorioCliente repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<ClienteDto>> EjecutarAsync()
    {
        var clientes = await _repositorio.ListarAsync();

        return clientes.MapToDto();
    }
}
