using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using UnityEngine;
namespace NutSort.Gameplay
{
    // SuccessPanel.GetReward and its response (9E8A6C / 9E8D2C).
    public sealed class OriginalSuccessRewardFlow
    {
        private readonly Action<bool,Action<JObject>> request;
        private readonly Action<int,OriginalItemGetInfo> showPanel;
        private readonly Action close;
        public OriginalSuccessRewardFlow(Action<bool,Action<JObject>> request,Action<int,OriginalItemGetInfo> showPanel,Action close)
        {this.request=request;this.showPanel=showPanel;this.close=close;}
        public void GetReward(bool isMore)
        {
            request(isMore,response=>Receive(response,isMore));
        }
        private void Receive(JObject response,bool isMore)
        {
            var info=new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>()};
            if(!string.IsNullOrEmpty(Text(Data(response),"hg_amt")))
            {
                var item=new OriginalItemInfo{ItemType=0};
                item.Count=OriginalRewardProgress.ToFloat(Text(Data(response),"hg_amt"));
                item.MoreCount=OriginalRewardProgress.ToFloat(Text(Data(response),"hg_amt"));
                item.CurrentCount=OriginalRewardProgress.ToFloat(Text(Data(response),"hg_psi"));
                info.ItemInfos.Add(item);
            }
            if(!string.IsNullOrEmpty(Text(Data(response),"hg_zs_amt")))
            {
                var item=new OriginalItemInfo{ItemType=1};
                item.Count=OriginalRewardProgress.ToFloat(Text(Data(response),"hg_zs_amt"));
                item.MoreCount=OriginalRewardProgress.ToFloat(Text(Data(response),"hg_zs_amt"));
                item.DoubleCurrentCount=ToDouble(Text(Data(response),"hg_zs_psi"));
                info.ItemInfos.Add(item);
            }
            info.IsMore=isMore;
            showPanel(8,info);
            close();
        }
        private static double ToDouble(string value)
        {
            if(double.TryParse(value,out double result))return result;
            Debug.LogError("ToDouble fail s:"+value);return 0;
        }
        private static JObject Data(JObject response)
        {
            JToken value=Field(response,"kinetic_data");
            if(value==null||value.Type==JTokenType.Null)throw new NullReferenceException("kinetic_data");
            return (JObject)value;
        }
        private static string Text(JObject obj,string name)=>(string)Field(obj,name);
        private static JToken Field(JObject obj,string name)
        {
            if(obj==null)throw new NullReferenceException("Reward document");
            JToken result=null;
            foreach(JProperty property in obj.Properties())
                if(string.Equals(property.Name,name,StringComparison.OrdinalIgnoreCase))result=property.Value;
            return result;
        }
    }
}
