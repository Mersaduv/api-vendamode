using Microsoft.EntityFrameworkCore;

namespace api_vendace.Models.Dtos;

public class ObjectValue
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Value> Value { get; set; } = new List<Value>();
}
[Owned]
public class Value
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}