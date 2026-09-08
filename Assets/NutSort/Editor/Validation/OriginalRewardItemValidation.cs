using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalRewardItemValidation
    {
        public static void Validate()
        {
            var settings=Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem");
            Check(settings!=null,"Reward settings load");
            var formatter=new OriginalGoldFormatter(()=>"en-US");
            string country="US";int countryReads=0;
            var factory=new OriginalRewardItemFactory(settings,formatter,()=>{countryReads++;return country;});
            var parent=new GameObject("Reward item validation",typeof(RectTransform));
            try
            {
                var infos=new OriginalItemGetInfo { IsMore=true,ItemInfos=new List<OriginalItemInfo>() };
                for(int type=0;type<=4;type++)infos.ItemInfos.Add(new OriginalItemInfo{ItemType=type,Count=type==0?12.5f:type==1?123.25f:2.5f,MoreCount=7.5f});
                infos.ItemInfos.Add(new OriginalItemInfo{ItemType=4,Count=0});
                foreach(bool big in new[]{false,true})
                {
                    int before=parent.transform.childCount;
                    var views=factory.GenerateItem(infos,parent.transform,big,true);
                    Check(views.Count==6&&parent.transform.childCount==before+6,"Native generation appends and retains zero-count entries");
                    for(int i=0;i<views.Count;i++)
                    {
                        var v=views[i];
                        Check(ReferenceEquals(v.ItemInfo,infos.ItemInfos[i]),"Original item reference retained");
                        Check(v.Icon.sprite!=null&&v.Count.font!=null&&v.Count.fontSharedMaterial!=null,"Sprite/font/material resolves");
                        Check(v.transform.parent==parent.transform,"Parent preserved");
                        Check(v.Count.text==(i==0?"$12.50":i==1?"123.25":i==5?"x0":"x2"),"Original type-specific formatting and midpoint-to-even; aggregate IsMore is not propagated");
                        foreach(var t in v.GetComponentsInChildren<Transform>(true))Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0,"No exported missing scripts");
                        Check(v.GetComponentsInChildren<OriginalLoopRotation>(true).Length==1,"Configured native replacement of rotation");
                        Check(v.Count.enableAutoSizing&&Mathf.Approximately(v.Count.fontSizeMax,big?94.1f:70f),"Original font sizing bounds");
                        Check(v.Icon.sprite==Resources.Load<Sprite>(settings.GetIconPath(infos.ItemInfos[i].ItemType,big,"US")),"Native size/type sprite choice");
                        var rect=v.Icon.sprite.rect;Check(Vector2.Distance(v.Icon.rectTransform.sizeDelta,rect.size)<.01f,"SetNativeSize applied");
                        if(big)Check(v.Max!=null&&!v.Max.activeSelf&&v.Icon.material.name=="FlowLightMaterial","Big item keeps flow material and hides Max even when requested");
                        else Check(v.Max==null,"Small item has no Max node");
                        infos.ItemInfos[i].IsMore=true;
                        if(v.Max!=null)v.Max.SetActive(true);
                        v.Init(infos.ItemInfos[i],big,false);
                        Check(v.Count.text==(i==0?"$7.50":i==1?"7.50":i==5?"x0":"x8"),"Per-item IsMore selects alternate float/coin/int values");
                        if(v.Max!=null)Check(!v.Max.activeSelf,"Max remains hidden for false too");
                        infos.ItemInfos[i].IsMore=false;
                    }
                    country="CA";views[0].Init(infos.ItemInfos[0],big);
                    Check(views[0].Icon.sprite==Resources.Load<Sprite>(settings.GetIconPath(0,big,"US")),"Canada cash uses US atlas");
                    country="FR";views[0].Init(infos.ItemInfos[0],big);
                    Check(views[0].Icon.sprite==Resources.Load<Sprite>(settings.GetIconPath(0,big,"DE")),"France cash uses DE atlas");
                    country="US";
                }
                var unknown=parent.GetComponentInChildren<OriginalRewardItemView>();
                unknown.Init(new OriginalItemInfo{ItemType=99,Count=3});
                Check(unknown.Icon.sprite==null&&unknown.Count.text=="x3","Missing numeric enum resource clears sprite but still formats count, with original diagnostic");
                string originalPath=settings.ItemPath;bool failed=false;
                try
                {
                    settings.ItemPath="Prefabs/Items/ValidationMissing";
                    try{factory.GenerateItem(infos,parent.transform);}catch(NullReferenceException){failed=true;}
                }
                finally{settings.ItemPath=originalPath;}
                Check(failed&&parent.transform.childCount==12,"Missing prefab logs then fails at component lookup without clearing earlier children");
                Check(countryReads==8,"Only cash touches country state");
                var fractional=new OriginalItemInfo {Count=.1f,MoreCount=.2f,CurrentCount=999,DoubleCurrentCount=999};
                Check(fractional.DoubleCount==(double).1f&&fractional.DoubleCount!=.1d,"Coin count widens float, not current balance or reparsed decimal");
                fractional.IsMore=true;Check(fractional.FloatCount==.2f&&fractional.DoubleCount==(double).2f,"Alternate count uses native float storage");
                Check(factory.GenerateItem(new OriginalItemGetInfo{ItemInfos=new List<OriginalItemInfo>()},parent.transform).Count==0&&parent.transform.childCount==12,"Empty list preserves existing children");
            }
            finally {UnityEngine.Object.DestroyImmediate(parent);}
            Debug.Log("NUT_REWARD_ITEM_VALIDATION_PASS source small/big prefabs, official UI/TMP, type/country icons and sizing, shared formatter, MoreCount precision, unconditional Max hiding and ordered append-only generation; production settlement host and visual parity pending.");
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    }
}
