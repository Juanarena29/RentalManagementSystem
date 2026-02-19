using System;

namespace CT.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string mensaje) : base(mensaje) { }
}
