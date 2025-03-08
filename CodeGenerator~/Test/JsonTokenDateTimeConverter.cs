using Newtonsoft.Json.Linq;
using System;

public class JsonTokenDateTimeConverter
{
    public static DateTime FromJToken(JToken token, DateTime existingValue)
    {
        if (token != null && token.Type == JTokenType.Integer)
        {
            return new DateTime(token.Value<int>(),1,1);
        }
        return existingValue;
    }

    public static JToken ToJToken(DateTime value)
    {
        return new JValue(value.Year); // ISO 8601 format
    }
}