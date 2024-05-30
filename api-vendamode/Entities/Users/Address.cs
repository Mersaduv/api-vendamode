using api_vendamode.Models;

namespace api_vendamode.Entities.Users;

public class Address : BaseClass<Guid>
{
    public Guid UserId { get; set; }
    public string FirstAddress { get; set; } = string.Empty;
    public string SecondAddress { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}