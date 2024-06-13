using System.Text.Json;
using api_vendace.Models;
using Microsoft.EntityFrameworkCore;

namespace api_vendace.Entities.Users;

public class Address : BaseClass<Guid>
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;

    public Province? Province { get; set; }
    public City? City { get; set; }

    public string FullAddress { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}

[Owned]
public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int Province_Id { get; set; }
}

[Owned]
public class Province
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}