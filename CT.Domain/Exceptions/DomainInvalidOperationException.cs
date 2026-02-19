using System;

namespace CT.Domain.Exceptions;

public class DomainInvalidOperationException : Exception
{
    public DomainInvalidOperationException(string mensaje) : base(mensaje) { }
}
