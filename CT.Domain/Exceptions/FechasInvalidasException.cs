using System;

namespace CT.Domain.Exceptions;

public class FechasInvalidasException : Exception
{
    public FechasInvalidasException(string mensaje) : base(mensaje) { }
}
