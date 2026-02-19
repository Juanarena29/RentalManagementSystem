using System;

namespace CT.Domain.Exceptions;

public class PagoInsuficienteException : Exception
{
    public PagoInsuficienteException(string mensaje) : base(mensaje) { }
}
