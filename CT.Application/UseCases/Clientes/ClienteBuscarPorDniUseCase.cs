using CT.Application.DTOs;
using CT.Domain.Interfaces;

namespace CT.Application.UseCases.Clientes;

/// <summary>
/// Busca un cliente por DNI y retorna sus datos, o null si no existe.
/// 
/// Diseñado para el flujo de carga de reservas:
/// 1. La UI llama a este UseCase con el DNI ingresado.
/// 2. Si retorna un ClienteDto → el cliente ya existe, se usa su Id para la reserva.
/// 3. Si retorna null → la UI muestra el formulario de alta y llama a ClienteAltaUseCase.
/// </summary>
public class ClienteBuscarPorDniUseCase
{
    private readonly IRepositorioCliente _repositorio;

    public ClienteBuscarPorDniUseCase(IRepositorioCliente repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ClienteDto?> EjecutarAsync(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            return null;

        var cliente = await _repositorio.ObtenerPorDniAsync(dni.Trim());

        return cliente?.MapToDto();
    }
}
