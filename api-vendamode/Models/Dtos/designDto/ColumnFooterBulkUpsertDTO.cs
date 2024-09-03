using api_vendamode.Entities.Designs;

namespace api_vendamode.Models.Dtos.designDto;

public class ColumnFooterBulkUpsertDTO
{
    public List<ColumnFooter> ColumnFooters { get; set; } = new List<ColumnFooter>();
}