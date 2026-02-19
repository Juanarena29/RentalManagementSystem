using System;

namespace CT.Domain.Exceptions;

public class ReservaCanceladaException : Exception
{
    public ReservaCanceladaException(string mensaje) : base(mensaje) { }
}
