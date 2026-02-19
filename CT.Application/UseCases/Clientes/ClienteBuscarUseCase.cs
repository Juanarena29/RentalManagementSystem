using CT.Application.DTOs;
using CT.Domain.Exceptions;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Clientes;

public class ClienteBuscarUseCase
{
    private readonly IRepositorioCliente _repositorio;

    public ClienteBuscarUseCase(IRepositorioCliente repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<ClienteDto>> EjecutarAsync(string textoBusqueda)
    {
        if (string.IsNullOrWhiteSpace(textoBusqueda))
            throw new ValidationException("Debe ingresar un texto de búsqueda.");

        var clientes = await _repositorio.BuscarAsync(textoBusqueda.Trim());

        return clientes.MapToDto();
    }
}
