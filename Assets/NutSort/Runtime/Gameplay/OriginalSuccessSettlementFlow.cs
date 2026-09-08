using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.Gameplay
{
    // Success b__2 0xA008B8 builds presentation data, never grants inventory here.
    public sealed class OriginalSuccessSettlementFlow
    {
        private readonly OriginalUserLocalData user;
        private readonly Action save,closePanels;
        private readonly Func<int,bool> isPanelOpen;
        private readonly Action<Action> enqueue;
        private readonly Action<int,OriginalItemGetInfo> showPanel;
        public OriginalSuccessSettlementFlow(OriginalUserLocalData user,Action save,Func<int,bool> isPanelOpen,
            Action<Action> enqueue,Action closePanels,Action<int,OriginalItemGetInfo> showPanel)
        {this.user=user;this.save=save;this.isPanelOpen=isPanelOpen;this.enqueue=enqueue;this.closePanels=closePanels;this.showPanel=showPanel;}
        public void Run(JObject response)
        {
            var info=new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>()};
            var data=Object(Field(response,"kinetic_data"));
            if(string.IsNullOrEmpty(Text(data,"hg_amt")))
            {
                if(user.Level==2||user.Level==3)
                {
                    int index=user.Level==2?0:1;
                    var rewards=Field(user.GoldRewardTargetS2CData,"bear_list");
                    if(rewards==null||rewards.Type==JTokenType.Null)throw new NullReferenceException("bear_list");
                    float value=OriginalRewardProgress.ToFloat(Text(Object(((JArray)rewards)[index]),"psi_value"));
                    if(index==0)user.Level1Gold=value;else user.Level2Gold=value;
                    info.ItemInfos.Add(new OriginalItemInfo{ItemType=0,Count=value,MoreCount=value});
                    save();
                }
            }
            else
            {
                var item=new OriginalItemInfo{ItemType=0};
                item.Count=OriginalRewardProgress.ToFloat(Text(data,"hg_amt"));
                item.MoreCount=OriginalRewardProgress.ToFloat(Text(data,"hg_amt"))*OriginalRewardProgress.ToFloat(Text(data,"bear_video"));
                info.ItemInfos.Add(item);
            }
            // Re-read after the early-level save, as the source does.
            data=Object(Field(response,"kinetic_data"));
            string coin=Text(data,"hg_zs_amt");
            if(!string.IsNullOrEmpty(coin)&&coin!="0")
                info.ItemInfos.Add(new OriginalItemInfo{ItemType=1,Count=OriginalRewardProgress.ToFloat(coin)});
            Action display=()=>{closePanels();showPanel(9,info);};
            if(isPanelOpen(11))enqueue(display);else display();
        }
        private static JObject Object(JToken value)
        {if(value==null||value.Type==JTokenType.Null)throw new NullReferenceException("Reward object");return (JObject)value;}
        private static string Text(JObject obj,string name)=> (string)Field(obj,name);
        private static JToken Field(JObject obj,string name)
        {
            if(obj==null)throw new NullReferenceException("Reward document");
            JToken result=null;
            foreach(var property in obj.Properties())
                if(string.Equals(property.Name,name,StringComparison.OrdinalIgnoreCase))result=property.Value;
            return result;
        }
    }
}
