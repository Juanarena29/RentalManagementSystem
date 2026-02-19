using CT.Application.DTOs;
using CT.Domain.Exceptions;

namespace CT.Application.Validations;

/// <summary>
/// Validaciones de formato para Pago.
/// Se usa en PagoRegistrarUseCase.
/// Las validaciones de negocio (reserva existe, no cancelada, saldo pendiente) van en el UseCase.
/// </summary>
public class PagoValidator
{
    /// <summary>
    /// Valida los datos para registrar un pago
    /// </summary>
    public void Validar(RegistrarPagoDto dto)
    {
        ValidarMonto(dto.Monto);
    }

    private void ValidarMonto(decimal monto)
    {
        if (monto <= 0)
            throw new ValidationException("El monto del pago debe ser mayor a 0.");
    }
}
