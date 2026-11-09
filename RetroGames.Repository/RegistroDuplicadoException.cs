using System;

namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(string message) : base(message) { }
    public RegistroDuplicadoException(string message, Exception innerException) : base(message, innerException) { }
}