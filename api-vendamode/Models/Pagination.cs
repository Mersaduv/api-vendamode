namespace api_vendamode.Models;

public class Pagination<T>
{
    public int CurrentPage { get; set; }
    public int NextPage { get; set; }
    public int PreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public int LastPage { get; set; }
    public List<T>? Data { get; set; }
    public int TotalCount { get; set; }
}
