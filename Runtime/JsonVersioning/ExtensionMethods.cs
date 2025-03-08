using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public static class JsonVersioningExtensionMethods
{
    public static JToken ToJToken(this int value)
    {
        return JToken.FromObject(value);
    }


    public static int FromJToken(this int value, JToken token)
    {
        return token.Value<int>();
    }
}
