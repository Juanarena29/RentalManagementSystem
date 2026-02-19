using System;

namespace CT.Domain.Exceptions;

public class CapacidadExcedidaException : Exception
{
    public CapacidadExcedidaException(string mensaje) : base(mensaje) { }
}
