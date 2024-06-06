using api_vendace.Interfaces;

namespace api_vendace.Entities;

public class EntityImage<TEntityIdType, TEntity> : IThumbnail
    where TEntity : class
{
    public Guid Id { get; set; }
    public TEntityIdType? EntityId { get; set; } 
    public string? ImageUrl { get; set; }
    public string? Placeholder { get; set; }
    public TEntity? Entity { get; set; } 
}