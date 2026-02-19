using System;

namespace CT.Domain.Exceptions;

public class DepartamentoNoDisponibleException : Exception
{
    public DepartamentoNoDisponibleException(string mensaje) : base(mensaje) { }
}
