using System.Reflection;
using api_vendace.Entities;
using api_vendace.Models;

namespace api_vendamode.Models.Dtos;

public class DescriptionEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EntityImage<Guid, DescriptionEntity> Thumbnail { get; set; }
}