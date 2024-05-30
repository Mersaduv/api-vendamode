namespace api_vendamode.Models;

public class PaginatedList<T>
{
    public List<T> Data { get; set; } = [];
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
