using System;

namespace CT.Domain.Exceptions;

public class WrongCredentialsException : Exception
{
    public WrongCredentialsException(string mensaje) : base(mensaje) { }
}
