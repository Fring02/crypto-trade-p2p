namespace Domain.Exceptions;

public class SessionBlockedException : Exception
{
    
    public SessionBlockedException(string message) : base(message){}
}