namespace OrderService.Application.Common.Exceptions;

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message) : base(message) { }
}
