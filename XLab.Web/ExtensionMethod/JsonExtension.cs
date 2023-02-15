using JsonNet.ContractResolvers;
using Newtonsoft.Json;

namespace XLab.Web.ExtensionMethod
{
    public static class JsonExtension
    {
        public static string ToJson<TSource>(this TSource data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static TResult FromJson<TResult>(this string json)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };

            return JsonConvert.DeserializeObject<TResult>(json, settings);
        }
    }
}