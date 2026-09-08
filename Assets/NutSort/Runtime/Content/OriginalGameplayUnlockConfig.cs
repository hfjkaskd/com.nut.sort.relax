using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    public sealed class OriginalGameplayUnlockConfig
    {
        private readonly OriginalUserLocalData user;
        private readonly int[] defaults;
        public OriginalGameplayUnlockConfig(OriginalUserLocalData user,int[] defaults)
        {
            this.user=user ?? throw new ArgumentNullException(nameof(user));
            this.defaults=(int[])(defaults ?? throw new ArgumentNullException(nameof(defaults))).Clone();
        }
        public int[] Read()
        {
            JObject config=user.ServerConfigData ?? throw new NullReferenceException("ServerConfigData");
            JToken token=null;
            foreach(JProperty field in config.Properties())
                if(string.Equals(field.Name,"LSSGPUL",StringComparison.OrdinalIgnoreCase))token=field.Value;
            if(token==null)return defaults;
            if(token.Type==JTokenType.Null)return null;
            var array=(JArray)token;
            var result=new int[array.Count];
            for(int i=0;i<result.Length;i++)result[i]=(int)array[i];
            return result;
        }
    }
}
