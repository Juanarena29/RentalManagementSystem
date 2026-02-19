using System;

namespace CT.Domain.Exceptions;

public class ReservaSuperpuestaException : Exception
{
    public ReservaSuperpuestaException(string mensaje) : base(mensaje) { }
}
