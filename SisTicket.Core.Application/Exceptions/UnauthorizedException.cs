namespace SisTicket.Core.Application.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException() 
        : base("No tiene permisos para realizar esta acción.")
    {
    }
}
