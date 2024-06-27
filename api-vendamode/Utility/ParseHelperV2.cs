using Newtonsoft.Json;

namespace api_vendamode.Utility;

public class ParseHelperV2
{
    public static List<T> ParseData<T>(List<string?> productAttributeData) where T : class
    {
        List<T> myObjectsList = [];
        try
        {
            foreach (var item in productAttributeData)
            {

                T[] myObjectsArray = JsonConvert.DeserializeObject<T[]>(item!)!;
                myObjectsList = new List<T>(myObjectsArray);
            }

            return myObjectsList;
        }
        catch (JsonException ex)
        {
            throw new Exception(ex.Message);
        }
    }

}