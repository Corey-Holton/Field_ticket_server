using Newtonsoft.Json;

namespace CA.Ticketing.Common.Extensions
{
    public static class JsonHelpers
    {
        public static string ToJson(this object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public static T? FromJson<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
