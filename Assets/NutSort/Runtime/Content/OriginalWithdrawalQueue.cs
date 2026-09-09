using System;
using Newtonsoft.Json.Linq;
namespace NutSort.Content
{
    public static class OriginalWithdrawalQueue
    {
        // UserLSSInfo.RefreshQueueCount, original integer-exclusive Random.Range calls.
        public static void Refresh(JObject account,Func<int,int,int> random)
        {
            int count=(int?)Field(account,"QueueCount")??0;
            if(count<=0){Set(account,"QueueCount",random(8000,9000));Set(account,"PassRate",random(90,96));return;}
            if(count>=101)
            {
                count=unchecked(count-random(50,100));Set(account,"QueueCount",count);
                if(count<=99)Set(account,"QueueCount",random(90,100));
            }
            else if(count>=11)
            {
                count=unchecked(count-random(10,20));Set(account,"QueueCount",count);
                if(count<=9)Set(account,"QueueCount",random(5,10));
            }
        }
        public static JToken Field(JObject row,string name)
        {JToken value=null;foreach(var p in row.Properties())if(string.Equals(p.Name,name,StringComparison.OrdinalIgnoreCase))value=p.Value;return value;}
        public static void Set(JObject row,string name,JToken value)
        {JProperty found=null;foreach(var p in row.Properties())if(string.Equals(p.Name,name,StringComparison.OrdinalIgnoreCase))found=p;if(found==null)row[name]=value;else found.Value=value??JValue.CreateNull();}
    }
}
