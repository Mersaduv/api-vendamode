namespace api_vendamode.Entities.Products;

public class SizeTable
{
    public List<string>? Columns { get; set; }
    public List<ModelTable>? Rows { get; set; }
}
public class ModelTable
{
    public string Name { get; set; } = string.Empty;
    public List<double>? Values { get; set; }
}
