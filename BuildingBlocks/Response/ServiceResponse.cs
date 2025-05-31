namespace BuildingBlocks.Response;

public class ServiceResponse<T>
{
    public bool Succeeded { get; set; } = true;
    public string Message { get; set; } = null!;
    public T? Data { get; set; }
}