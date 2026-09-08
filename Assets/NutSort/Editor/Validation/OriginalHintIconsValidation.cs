using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalHintIconsValidation
    {
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            JArray source=(JArray)tables.ReadTable("pay.json")["channelInfos"];
            foreach(JToken row in source)
            {
                var info=tables.ChannelInfos.GetChannelInfo((string)row["channel"]);
                Check(info.Channel==(string)row["channel"] && info.Info.Count==((JArray)row["info"]).Count,"Original channel info records");
            }
            var selected=tables.ChannelInfos.GetSelfChannel(tables.PayChannels,"US",JObject.Parse("{\"GetType\":2}"));
            Check(selected.Channel=="PayPal" && ReferenceEquals(selected,tables.ChannelInfos.GetChannelInfo("PayPal")),"Saved selection uses shared channel row");
            Check(tables.ChannelInfos.GetSelfChannel(tables.PayChannels,"US",new JObject()).Channel=="Venmo","Missing integer field uses source zero default");
            bool failed=false;try{tables.ChannelInfos.GetSelfChannel(tables.PayChannels,"US",JObject.Parse("{\"GetType\":9}"));}catch(ArgumentOutOfRangeException){failed=true;}
            Check(failed,"Invalid channel index not clamped");
            Check(OriginalChannelInfos.IconPath("PayPal")=="Atlas/PaySimple/PayPal" && OriginalChannelInfos.IconPath("PayPal",true)=="Atlas/Pay/PayPal","Original resource variants");
            var go=new GameObject("Hint icon validation",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));
            try
            {
                Image head=go.GetComponent<Image>();
                Sprite original=Resources.Load<Sprite>("Atlas/PaySimple/PayPal");Check(original!=null,"Current icon resource");head.sprite=original;
                int draws=0,loads=0;
                var icons=new OriginalPlayerGoldHintIcons(tables.PayChannels,()=>"US",path=>{loads++;Check(path=="Atlas/PaySimple/PayPal","Original image path");return null;},(a,b)=>{draws++;Check(a==0 && b==3,"Country channel draw");return 2;});
                icons.ApplyPush(head);Check(head.sprite==original && draws==1,"Push keeps old icon on missing load");
                icons.RefreshRandom(head);Check(head.sprite==null && draws==2,"Random helper assigns missing load");
                head.sprite=original;icons.ApplySelf(head,selected);Check(head.sprite==null && draws==2 && loads==3,"Self icon assigns missing load without random draw");
                Debug.Log("NUT_HINT_ICONS_VALIDATION_PASS original channel metadata, saved index, resource paths and distinct missing-sprite assignment rules.");
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
