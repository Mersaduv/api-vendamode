using System.Text.Json;
using System.Text.Json.Serialization;
using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class ColumnFooter : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Index { get; set; }
    public List<FooterArticleColumn> FooterArticleColumns { get; set; } = new List<FooterArticleColumn>();
}