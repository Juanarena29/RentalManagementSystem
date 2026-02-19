using System;

namespace CT.Domain.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string mensaje) : base(mensaje) { }
}
