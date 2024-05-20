using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace api_vendamode.Utility;

public class ParseHelper
{
    public static List<T> ParseData<T>(List<string?> productAttributeData) where T : class
    {
        List<T> myObjectsList = [];
        try
        {
            foreach (var item in productAttributeData)
            {
                string jsonString = $"[{item}]";

                T[] myObjectsArray = JsonConvert.DeserializeObject<T[]>(jsonString)!;
                myObjectsList = new List<T>(myObjectsArray);
            }

            return myObjectsList;
        }
        catch (JsonException)
        {
            return new List<T>();
        }
    }

}