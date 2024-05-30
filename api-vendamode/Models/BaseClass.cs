namespace api_vendamode.Models;

public class BaseClass<T>
{
    public T? Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastUpdated { get; set; }
}
