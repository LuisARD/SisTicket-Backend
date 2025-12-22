namespace SisTicket.Core.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key) 
        : base($"La entidad \"{name}\" con id ({key}) no fue encontrada.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
