using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalSimpleMarqueeValidation
    {
        public static void Validate()
        {
            var prefab=Resources.Load<GameObject>("Prefabs/PlayerInfo/PMDSimple");Check(prefab!=null,"Original PMDSimple loads");
            Check(prefab.GetComponentsInChildren<Transform>(true).Length==8,"Original eight-object hierarchy");
            foreach(var t in prefab.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No missing scripts");
            foreach(var t in prefab.GetComponentsInChildren<TMP_Text>(true))Check(t.font!=null&&t.fontSharedMaterial!=null,"Fonts/materials resolve");
            var obj=UnityEngine.Object.Instantiate(prefab);var view=obj.GetComponent<OriginalSimpleMarquee>();
            GameObject player=null;
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                var text=new OriginalMarqueeText(user,tables,()=>"Fixture",v=>"$7",(a,b)=>7);
                int reads=0,binds=0,updates=0;bool available=true;
                Func<OriginalMarqueeItem> select=()=>{reads++;return available?new OriginalMarqueeItem{IsGold=true,Minimum=7,Maximum=7}:null;};
                Action<OriginalMarqueeItemView> bind=item=>{binds++;item.Bind(text,tables.PayChannels,"en",()=>"US",(seconds,done)=>{updates++;Check(seconds==.01f,"Native item width delay");},path=>null,(a,b)=>0);};
                view.Bind(select,bind);view.Advance(3);Check(reads==0,"No scrolling before init");
                view.Init(3);
                Check(reads==4&&binds==3&&updates==3&&view.Items.Count==3&&obj.activeSelf,"Availability read plus three independent selected rows");
                Check(view.Items[0].Rect.anchoredPosition.y==-240&&view.Items[1].Rect.anchoredPosition.y==0&&view.Items[2].Rect.anchoredPosition.y==-120,"Native source-after-clone positioning");
                Check(view.Items[1].name=="2"&&view.Items[2].name=="3","Original clone names");
                view.Advance(1.5f);Check(view.Items[1].Rect.anchoredPosition.y==0&&view.Remaining==0,"Zero remaining still holds");
                view.Advance(.8f);
                Check(view.Items[0].Rect.anchoredPosition.y==-120&&view.Items[1].Rect.anchoredPosition.y==-240&&view.Items[2].Rect.anchoredPosition.y==0&&updates==4&&view.Remaining==1.5f,"Crossing frame uses full delta, wraps one row and continues moving following rows");
                view.Init(4);Check(reads==6&&binds==3&&updates==4&&view.Items[0].Rect.anchoredPosition.y==-120,"Repeated init updates level/hold only, retaining rows/text/positions");
                view.Advance(3);Check(updates==7&&view.Items[0].Rect.anchoredPosition.y==-240&&view.Items[1].Rect.anchoredPosition.y==-240&&view.Items[2].Rect.anchoredPosition.y==-240,"Large delta wraps each row once, discards overshoot and keeps iterating after hold reset");
                available=false;view.Init(4);Check(!obj.activeSelf&&updates==7,"Missing selection hides existing block without clearing text");
                available=true;
                user.ServerConfigData=JObject.Parse(@"{""LSS260820"":false}");
                player=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerInfo/PlayerInfo"));
                var info=player.GetComponent<OriginalPlayerInfo>();OriginalSimpleMarquee child=null;
                info.Init(3,user,tables,"en",()=>true,()=>"9/8/2026",()=>46,()=>7,v=>"$7",(parent,level)=>
                {child=UnityEngine.Object.Instantiate(prefab,parent,false).GetComponent<OriginalSimpleMarquee>();child.Bind(select,bind);child.Init(level);});
                Check(child!=null&&child.transform.parent==info.MarqueeParent&&child.Items.Count==3&&child.gameObject.activeSelf,"Actual PlayerInfo composes actual PMDSimple with three original row views");
                Debug.Log("NUT_SIMPLE_MARQUEE_VALIDATION_PASS original prefab and actual PlayerInfo composition, native clone ordering, live selection, hold boundary/full delta movement, reuse/reinit/large-delta behavior; production TXPanel owner pending.");
            }
            finally{UnityEngine.Object.DestroyImmediate(obj);if(player!=null)UnityEngine.Object.DestroyImmediate(player);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
