using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    public sealed class OriginalServerConfigRequests
    {
        public static JObject Cached;
        private readonly OriginalServerConfigDefaults defaults;
        private readonly Func<OriginalCountryInfo> country;
        private readonly OriginalServerConfigInitialization initialization;
        private readonly Action<string,Action<string,byte[]>> request;
        public OriginalServerConfigRequests(OriginalServerConfigDefaults defaults,Func<OriginalCountryInfo> country,
            OriginalServerConfigInitialization initialization,Action<string,Action<string,byte[]>> request)
        {this.defaults=defaults;this.country=country;this.initialization=initialization;this.request=request;}
        public void Config(Action<bool> callback)=>Request(value=>callback?.Invoke(value!=null));
        public void Request(Action<JObject> callback=null)
        {
            if(Cached!=null){callback?.Invoke(Cached);return;}
            request(country().Area,(json,bytes)=>Receive(json,callback));
        }
        private void Receive(string json,Action<JObject> callback)
        {
            if(string.IsNullOrEmpty(json)){callback?.Invoke(null);return;}
            Cached=OriginalServerConfigJson.Read(json,defaults);
            if(Cached==null)throw new NullReferenceException("ServerConfigData");
            initialization.Initialize(Cached);
            callback?.Invoke(Cached);
        }
    }
}
