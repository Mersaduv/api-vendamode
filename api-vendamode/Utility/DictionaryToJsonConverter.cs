using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
namespace api_vendamode.Utility;
public class DictionaryToJsonConverter : ValueConverter<Dictionary<string, object>, string>
{
    public DictionaryToJsonConverter() : base(
        v => JsonConvert.SerializeObject(v),
        v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v) ?? new Dictionary<string, object>())
    { }
}